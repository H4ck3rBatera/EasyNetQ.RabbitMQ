#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EasyNetQ.RabbitMQ.Api/EasyNetQ.RabbitMQ.Api.csproj", "EasyNetQ.RabbitMQ.Api/"]
COPY ["EasyNetQ.RabbitMQ.Worker/EasyNetQ.RabbitMQ.Worker.csproj", "EasyNetQ.RabbitMQ.Worker/"]
COPY ["EasyNetQ.RabbitMQ.Domain/EasyNetQ.RabbitMQ.Domain.csproj", "EasyNetQ.RabbitMQ.Domain/"]
RUN dotnet restore "EasyNetQ.RabbitMQ.Api/EasyNetQ.RabbitMQ.Api.csproj"
COPY . .
WORKDIR "/src/EasyNetQ.RabbitMQ.Api"
RUN dotnet build "EasyNetQ.RabbitMQ.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyNetQ.RabbitMQ.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyNetQ.RabbitMQ.Api.dll"]
