# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Shared/OrderCase.Contracts/OrderCase.Contracts.csproj src/Shared/OrderCase.Contracts/
COPY src/Services/Ordering/OrderCase.Ordering.Domain/OrderCase.Ordering.Domain.csproj src/Services/Ordering/OrderCase.Ordering.Domain/
COPY src/Services/Ordering/OrderCase.Ordering.Persistence/OrderCase.Ordering.Persistence.csproj src/Services/Ordering/OrderCase.Ordering.Persistence/
COPY src/Services/Ordering/OrderCase.Ordering.Application/OrderCase.Ordering.Application.csproj src/Services/Ordering/OrderCase.Ordering.Application/
COPY src/Services/Ordering/OrderCase.Ordering.Infrastructure/OrderCase.Ordering.Infrastructure.csproj src/Services/Ordering/OrderCase.Ordering.Infrastructure/
COPY src/Services/Ordering/OrderCase.Ordering.Api/OrderCase.Ordering.Api.csproj src/Services/Ordering/OrderCase.Ordering.Api/

RUN dotnet restore src/Services/Ordering/OrderCase.Ordering.Api/OrderCase.Ordering.Api.csproj

COPY src/ src/

RUN dotnet publish src/Services/Ordering/OrderCase.Ordering.Api/OrderCase.Ordering.Api.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "OrderCase.Ordering.Api.dll"]
