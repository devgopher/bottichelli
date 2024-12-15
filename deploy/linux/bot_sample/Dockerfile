FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy everything
RUN git clone https://github.com/devgopher/botticelli.git
WORKDIR /app/botticelli/Botticelli
RUN git fetch --all
RUN git switch release/0.5
RUN git pull

COPY . ./

# Restore as distinct layers
# RUN dotnet restore

# Build and publish a release
RUN dotnet publish -c Release -o /app/out
RUN ls .

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE $http_port
EXPOSE $https_port

ENV ASPNETCORE_URLS https://0.0.0.0:$https_port;http://0.0.0.0:$http_port
ENV ASPNETCORE_ENVIRONMENT=Release
ENV ServerSettings__ServerUri $server_uri
ENV AnalyticsSettings__ServerUri $analytics_uri

ENTRYPOINT ["dotnet", "<Entrypoint>.dll"]