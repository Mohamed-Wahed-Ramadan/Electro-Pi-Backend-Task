FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ProjectManagement.sln ./
COPY src/ProjectManagement.Domain/ src/ProjectManagement.Domain/
COPY src/ProjectManagement.Application/ src/ProjectManagement.Application/
COPY src/ProjectManagement.Persistence/ src/ProjectManagement.Persistence/
COPY src/ProjectManagement.Infrastructure/ src/ProjectManagement.Infrastructure/
COPY src/ProjectManagement.API/ src/ProjectManagement.API/

RUN dotnet restore src/ProjectManagement.API/ProjectManagement.API.csproj
RUN dotnet publish src/ProjectManagement.API/ProjectManagement.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProjectManagement.API.dll"]
