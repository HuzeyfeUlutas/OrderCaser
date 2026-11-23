# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Shared/OrderCase.Contracts/OrderCase.Contracts.csproj src/Shared/OrderCase.Contracts/
COPY src/Services/Processing/OrderCase.Processing.Domain/OrderCase.Processing.Domain.csproj src/Services/Processing/OrderCase.Processing.Domain/
COPY src/Services/Processing/OrderCase.Processing.Persistence/OrderCase.Processing.Persistence.csproj src/Services/Processing/OrderCase.Processing.Persistence/
COPY src/Services/Processing/OrderCase.Processing.Application/OrderCase.Processing.Application.csproj src/Services/Processing/OrderCase.Processing.Application/
COPY src/Services/Processing/OrderCase.Processing.Infrastructure/OrderCase.Processing.Infrastructure.csproj src/Services/Processing/OrderCase.Processing.Infrastructure/
COPY src/Services/Processing/OrderCase.Processing.Worker/OrderCase.Processing.Worker.csproj src/Services/Processing/OrderCase.Processing.Worker/

RUN dotnet restore src/Services/Processing/OrderCase.Processing.Worker/OrderCase.Processing.Worker.csproj

COPY src/ src/

RUN dotnet publish src/Services/Processing/OrderCase.Processing.Worker/OrderCase.Processing.Worker.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "OrderCase.Processing.Worker.dll"]
