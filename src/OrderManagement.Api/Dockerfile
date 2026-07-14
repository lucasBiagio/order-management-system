FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/OrderManagement.Api/OrderManagement.Api.csproj", "src/OrderManagement.Api/"]
COPY ["src/OrderManagement.Application/OrderManagement.Application.csproj", "src/OrderManagement.Application/"]
COPY ["src/OrderManagement.Domain/OrderManagement.Domain.csproj", "src/OrderManagement.Domain/"]
COPY ["src/OrderManagement.Infrastructure/OrderManagement.Infrastructure.csproj", "src/OrderManagement.Infrastructure/"]

RUN dotnet restore "src/OrderManagement.Api/OrderManagement.Api.csproj"

COPY . .

RUN dotnet restore "src/OrderManagement.Api/OrderManagement.Api.csproj" --force

RUN dotnet publish "src/OrderManagement.Api/OrderManagement.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/Data

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "OrderManagement.Api.dll"]