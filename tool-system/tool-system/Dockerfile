FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["tool-system.csproj", "./"]
RUN dotnet restore "tool-system.csproj"

COPY . .
RUN dotnet publish "tool-system.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

USER root

ENTRYPOINT ["dotnet", "tool-system.dll"]