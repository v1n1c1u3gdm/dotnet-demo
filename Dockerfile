FROM mcr.microsoft.com/dotnet/sdk:9.0 AS api-build
WORKDIR /src

COPY api/DotnetDemo.sln .
COPY api/Directory.Build.props ./Directory.Build.props
COPY api/DotnetDemo.Api/DotnetDemo.Api.csproj DotnetDemo.Api/
COPY api/DotnetDemo.Tests/DotnetDemo.Tests.csproj DotnetDemo.Tests/
RUN dotnet restore DotnetDemo.Api/DotnetDemo.Api.csproj

COPY api/ .
RUN dotnet publish DotnetDemo.Api/DotnetDemo.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS api-app
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=api-build /app/publish .
RUN mkdir -p /app/logs
EXPOSE 8080
ENTRYPOINT ["dotnet", "DotnetDemo.Api.dll"]

# -------- Vue build stage --------
FROM node:lts-alpine AS ui-build
WORKDIR /app
COPY ui/package*.json ./
RUN npm install
COPY ui/ .
ARG VUE_APP_API_BASE=http://localhost:3000
ARG VUE_APP_ARTICLE_PUBLIC_BASE_URL=https://viniciusmenezes.com
ENV VUE_APP_ARTICLES_URL=${VUE_APP_API_BASE}/articles
ENV VUE_APP_AUTHORS_URL=${VUE_APP_API_BASE}/authors
ENV VUE_APP_ARTICLES_COUNT_URL=${VUE_APP_API_BASE}/articles/count_by_author
ENV VUE_APP_SOCIALS_URL=${VUE_APP_API_BASE}/socials
ENV VUE_APP_ARTICLE_PUBLIC_BASE_URL=${VUE_APP_ARTICLE_PUBLIC_BASE_URL}
RUN npm run build

# -------- Vue runtime stage --------
FROM nginx:stable-alpine AS ui-app
COPY --from=ui-build /app/dist /usr/share/nginx/html
COPY ui/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
