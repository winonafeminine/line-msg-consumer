FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
# RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
# USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS publish
WORKDIR /src
# COPY ["./api/article-sv/article-sv.csproj", "./"]
# RUN dotnet restore "article-sv.csproj"
# RUN apk add --update nodejs npm
ARG DEBIAN_FRONTEND=noninteractive
ENV TZ=Asia/Bangkok

RUN curl -fsSL https://deb.nodesource.com/setup_20.x | sh - && \
    apt-get install -y nodejs tzdata
# RUN apk add --update nodejs tzdata npm

COPY ./package*.json ./
RUN npm install

WORKDIR "/src/."

COPY ./ ./

RUN npx nx run Api.UserSv:publish
# generate a dev certs
# RUN dotnet dev-certs https --trust && \
#     dotnet dev-certs https -ep "/src/dist/apps/Api.UserSv/net6.0/publish/aspnetapp.pfx" -p "SecurePwdGoesHere"

FROM base AS final
WORKDIR /app
COPY --from=publish /src/dist/apps/Api.UserSv/net6.0/publish .
ENTRYPOINT ["dotnet", "ApiUserSv.dll"]