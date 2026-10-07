FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release


WORKDIR /src



COPY ["src/Fcg.Users.API/Fcg.User.API.csproj", "Fcg.Users.API/"]
COPY ["src/Fcg.Users.Application/Fcg.Users.Application.csproj", "Fcg.Users.Application/"]
COPY ["src/Fcg.Users.Domain/Fcg.Users.Domain.csproj", "Fcg.Users.Domain/"]
COPY ["src/Fcg.Users.Infrastructure/Fcg.Users.Infrastructure.csproj", "Fcg.Users.Infrastructure/"]

RUN dotnet restore "./Fcg.Users.API/Fcg.User.API.csproj"

COPY src/ .
WORKDIR "/src/Fcg.Users.API"
RUN dotnet build "./Fcg.User.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet ef migrations bundle --self-contained -r linux-x64 -o /app/build/efbundle

FROM build AS publish
RUN dotnet publish "./Fcg.User.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root

COPY --from=build /app/build/efbundle ./efbundle
RUN chmod +x ./efbundle
USER app

ENTRYPOINT []
CMD ["dotnet", "Fcg.User.API.dll"]