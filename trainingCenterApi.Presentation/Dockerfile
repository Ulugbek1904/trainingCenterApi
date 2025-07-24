# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["trainingCenterApi.Presentation/trainingCenterApi.Presentation.csproj", "trainingCenterApi.Presentation/"]
COPY ["trainingCenter.Infrastructure/trainingCenter.Infrastructure.csproj", "trainingCenter.Infrastructure/"]
COPY ["trainingCenter.Domain/trainingCenter.Domain.csproj", "trainingCenter.Domain/"]
COPY ["trainingCenterApi.Common/trainingCenterApi.Common.csproj", "trainingCenterApi.Common/"]
COPY ["trainingCenter.Services/trainingCenter.Services.csproj", "trainingCenter.Services/"]
RUN dotnet restore "./trainingCenterApi.Presentation/trainingCenterApi.Presentation.csproj"
COPY . .
WORKDIR "/src/trainingCenterApi.Presentation"
RUN dotnet build "./trainingCenterApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./trainingCenterApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "trainingCenterApi.Presentation.dll"]