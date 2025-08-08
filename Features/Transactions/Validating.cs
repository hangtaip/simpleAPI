using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;

namespace TransactionApi;

public class Validating
{
    private readonly RequestDelegate _next;
    private readonly IAppLogger _logger;

    public Validating(RequestDelegate next, IAppLogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IValidator<TransactionDetailDTO> validator)
    {
        if (!ShouldValidate(context))
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();

        try
        {
            var transaction = await ReadAndValidateJson(context, validator);

            if (transaction == null) return;

            context.Items["ValidatedTransaction"] = transaction;
            context.Request.Body.Position = 0;

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error($"transaction last: {context.Items["ValidatedTransaction"]}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task<TransactionDetailDTO?> ReadAndValidateJson(HttpContext context, IValidator<TransactionDetailDTO> validator)
    {
        try
        {
            var transaction = await context.Request.ReadFromJsonAsync<TransactionDetailDTO>();
            _logger.Info($"Received transaction: {JsonSerializer.Serialize(transaction)}");


            if (transaction == null)
            {
                _logger.Error($"transaction first: {transaction}");
                await RespondWithError(context, "Request body is required");
                return null;
            }

            var validationResult = await validator.ValidateAsync(transaction);
            if (!validationResult.IsValid)
            {
                await RespondWithValidationErrors(context, validationResult);
                return null;
            }

            return transaction;
        }
        catch (JsonException ex)
        {
            await RespondWithError(context, $"Invalid JSON: {GetJsonErrorMessage(ex)}");
            return null;
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var rootException = exception is BadHttpRequestException badReqEx &&
                            badReqEx.InnerException != null
                            ? badReqEx.InnerException
                            : exception;

        switch (rootException)
        {
            case JsonException ex:
                break;
            default:
                _logger.Error("Validation error", rootException);
                await RespondWithError(context, "Invalid request");
                break;
        }
    }

    private string GetJsonErrorMessage(JsonException ex)
    {
        return ex switch
        {
            { Path: null } => "Invalid JSON format",
            { Path: not null } when ex.Message.Contains("could not be converted") => $"Invalid value at '{ex.Path}'",
            _ => ex.Message
        };
    }

    private async Task RespondWithValidationErrors(HttpContext context, ValidationResult result)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errorMessages = result.Errors
            // .GroupBy(e => e.PropertyName)
            // .ToDictionary(
            //     g => g.Key,
            //     g => g.Select(e => e.ErrorMessage).ToArray()
            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
            .Distinct();

        await context.Response.WriteAsJsonAsync(new FailedResponseDetailDTO
        {
            Result = 0,
            Resultmessage = "Validation failed: " +
                string.Join(", ", errorMessages)

        });
    }

    private async Task RespondWithError(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new FailedResponseDetailDTO
        {
            Result = 0,
            Resultmessage = message
        });
    }

    private bool ShouldValidate(HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/api/submittrxmessage") &&
            context.Request.Method == HttpMethods.Post;
    }
}
