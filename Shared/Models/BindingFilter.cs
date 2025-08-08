using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TransactionApi;

public class ModelBindingFilter : IEndpointFilter
{
    private readonly IAppLogger _logger;

    public ModelBindingFilter(IAppLogger logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
            )
    {
        var modelState = context.HttpContext.Items["__ModelState"] as ModelStateDictionary;

        try
        {
            return await next(context);
        }
        catch (BadHttpRequestException ex) when (ex.Message.Contains("Failed to read parameter"))
        {
            _logger.Warning("Model bind failed", ex);

            var errors = modelState?
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return Results.ValidationProblem(
                errors ?? new Dictionary<string, string[]>(),
                title: "Invalid request data",
                detail: "One or more fields contain invalid value",
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
