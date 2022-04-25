#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000 

ENV ASPNETCORE_URLS=http://+:5000
ENV RABBITMQ_PORT 5672
ENV RABBITMQ_HOST localhost

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CredentialValidation.csproj", "."]
RUN dotnet restore "./CredentialValidation.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CredentialValidation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CredentialValidation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CredentialValidation.dll"]