From mcr.microsoft.com/dotnet/sdk:10.0-preview@sha256:46e0cca8e6693abfe4a596eab89a00bdf05602ce7d4d77fbe5676c686cf7f700 AS build
WORKDIR /App

Copy . ./
RUN dotnet restore
RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview@sha256:50da860c10c541ea114b8db0282b845baab4264d4bef9e9e2ab566d905b3ad68 AS runtime
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "TransactionApi.dll"]
