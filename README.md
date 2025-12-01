# .NET Demo (ASP.NET Core + NHibernate)

Repositório reescrito com **ASP.NET Core 9** sobre Kestrel, NHibernate e MySQL containerizado. O objetivo é reconstruir o blog gerado via Publii, evoluindo-o para um CMS de alta performance e totalmente observável. A raiz continua dividida em dois módulos:

- `api/` – solução .NET com `DotnetDemo.Api` (Web API) e `DotnetDemo.Tests` (xUnit + WebApplicationFactory).
- `ui/` – front-end Vue 2 + BootstrapVue, consumindo os mesmos endpoints (a mesma estrutura do blog Publii original), agora servindo também como base para o painel administrativo do futuro CMS.

## Arquitetura & Tecnologias

- **API (`api/`)**:
  - ASP.NET Core 9 (Kestrel) + `Startup.cs` tradicional que executa FluentMigrator + seeding logo no boot.
  - NHibernate + FluentNHibernate mappings para `Author`, `Article` e `Social`, com `StringListJsonType` para o campo `tags`.
  - Migrations FluentMigrator equivalentes às do projeto Ruby (criação das tabelas e remoção de `url`), rodando automaticamente.
  - Seeds idempotentes lendo o mesmo dataset (`Seeds/article_seed_data.json`) e respeitando `VINICIUS_PUBLIC_KEY`.
  - Swagger em `/api-docs`, redirect padrão em `/`, e HTML "tabelaço" em `/tech`.
  - Observabilidade com OpenTelemetry Metrics + Prometheus exporter (`GET /metrics`) e middleware customizado para contadores/histogramas.
  - Logging via Serilog (console + arquivos) com `api/logs/app.log` e `api/logs/nhibernate.log`.
- **UI (`ui/`)**: Vue 2 (Vue CLI) + BootstrapVue, build multi-stage (Node → NGINX) compartilhado no mesmo `Dockerfile`. As variáveis `VUE_APP_*` são injetadas na build para apontar para `http://localhost:3000`.
- **Banco (`db/`)**: MySQL 8.4 em container dedicado com volume `mysql_data` e credenciais fixas (`ruby-demo` / `2u8y-c0d3`).
- **Orquestração**: `docker-compose.yml` agora referencia o `Dockerfile` oficial da Microsoft (sample `aspnetapp`), publica a API em `:3000` (container `8080`) e monta o diretório de logs.

## Tecnologias

### Back-end (API)

#### Framework & Runtime
- **ASP.NET Core 9** – Framework web moderno e de alta performance
- **Kestrel** – Servidor web cross-platform integrado
- **.NET SDK 9** – SDK para desenvolvimento local

#### ORM & Banco de Dados
- **NHibernate 5.5.1** – ORM robusto e maduro
- **FluentNHibernate 3.3.0** – Mapeamento fluente para NHibernate
- **FluentMigrator 6.0.0** – Migrations versionadas e automatizadas
- **MySqlConnector 2.5.0** – Driver MySQL de alta performance
- **MySql.Data 9.0.0** – Driver MySQL oficial
- **Microsoft.Data.Sqlite 9.0.0** – SQLite para testes de integração

#### Autenticação & Segurança
- **Microsoft.AspNetCore.Authentication.JwtBearer 9.0.0** – Autenticação JWT para API

#### Documentação & Validação
- **Swashbuckle.AspNetCore 6.6.2** – Geração automática de documentação Swagger/OpenAPI
- **Microsoft.AspNetCore.OpenApi 9.0.11** – Suporte OpenAPI
- **FluentValidation.AspNetCore 11.3.1** – Validação fluente de modelos

#### Observabilidade & Logging
- **Serilog.AspNetCore 8.0.1** – Framework de logging estruturado
- **Serilog.Sinks.Console 6.0.0** – Sink para console
- **Serilog.Sinks.File 5.0.0** – Sink para arquivos
- **Serilog.Settings.Configuration 8.0.0** – Configuração via appsettings.json
- **OpenTelemetry.Extensions.Hosting 1.9.0** – Extensões OpenTelemetry
- **OpenTelemetry.Instrumentation.AspNetCore 1.9.0** – Instrumentação ASP.NET Core
- **OpenTelemetry.Instrumentation.Http 1.9.0** – Instrumentação HTTP
- **OpenTelemetry.Instrumentation.Runtime 1.9.0** – Instrumentação de runtime
- **OpenTelemetry.Exporter.Prometheus.AspNetCore 1.11.0-beta.1** – Exportador Prometheus

#### Testes
- **xUnit 2.9.2** – Framework de testes
- **FluentAssertions 6.12.2** – Assertions legíveis e expressivas
- **Microsoft.AspNetCore.Mvc.Testing 9.0.0** – Testes de integração com WebApplicationFactory
- **coverlet.collector 6.0.2** – Coleta de cobertura de código

### Front-end (UI)

#### Framework & Core
- **Vue.js 2.6.14** – Framework JavaScript progressivo
- **Vue Router 3.6.5** – Roteamento oficial do Vue.js
- **core-js 3.8.3** – Polyfills para compatibilidade

#### UI & Estilização
- **Bootstrap 4.6.2** – Framework CSS responsivo
- **BootstrapVue 2.23.1** – Componentes Vue baseados em Bootstrap
- **@fontsource/ubuntu 5.2.8** – Fonte Ubuntu self-hosted
- **@fortawesome/fontawesome-free 7.1.0** – Ícones Font Awesome

#### Bibliotecas de Terceiros
- **jQuery 3.7.1** – Biblioteca JavaScript (requerida por DataTables e Summernote)
- **DataTables.net 1.13.8** – Tabelas interativas com paginação, busca e ordenação
- **datatables.net-bs4 1.13.8** – Integração DataTables com Bootstrap 4
- **Summernote 0.9.1** – Editor WYSIWYG para conteúdo rico

#### Ferramentas de Desenvolvimento
- **Vue CLI 5.0.0** – Ferramentas de linha de comando para Vue.js
- **@vue/cli-service 5.0.0** – Serviços de build e desenvolvimento
- **@vue/cli-plugin-babel 5.0.0** – Plugin Babel para transpilação
- **@vue/cli-plugin-eslint 5.0.0** – Plugin ESLint para linting
- **@vue/cli-plugin-unit-jest 5.0.0** – Plugin Jest para testes unitários
- **@babel/core 7.12.16** – Compilador JavaScript
- **@babel/eslint-parser 7.12.16** – Parser Babel para ESLint
- **eslint 7.32.0** – Linter JavaScript
- **eslint-plugin-vue 8.0.3** – Plugin ESLint para Vue.js

#### Testes
- **Jest** (via @vue/cli-plugin-unit-jest) – Framework de testes JavaScript
- **@vue/test-utils 1.3.4** – Utilitários para testes de componentes Vue
- **@vue/vue2-jest 27.0.0** – Transformador Jest para Vue 2
- **babel-jest 27.5.1** – Transformador Babel para Jest
- **jest-environment-jsdom 27.5.1** – Ambiente DOM para Jest
- **vue-template-compiler 2.6.14** – Compilador de templates Vue
- **flush-promises 1.0.2** – Utilitário para testes assíncronos

### Infraestrutura & DevOps

- **Docker 24+** – Containerização
- **Docker Compose v2** – Orquestração de containers
- **MySQL 8.4** – Banco de dados relacional
- **NGINX** – Servidor web para servir o front-end em produção

## Endpoints principais

- `GET /api-docs` – Swagger UI (redirecionado automaticamente a partir de `/`).
- `GET /articles`, `/articles/{id}`, `/articles/count_by_author` – com payloads idênticos aos consumidos pelo `ui/`.
- `GET /authors`, `/authors/{id}` – inclui `socials` e `articles`.
- `GET /socials`.
- `GET /up` – healthcheck JSON (`status`, `service`, `timestamp`).
- `GET /metrics` – OpenTelemetry/Prometheus com counters/histogramas + gauge de liveness.
- `GET /tech` – relatório HTML replicando o "tabelaço" do Rails.

## Como iniciar com Docker

### Pré-requisitos

- Docker 24+ + Compose v2.
- Portas livres: 3000 (API), 3306 (MySQL) e 8080 (UI).

### Passo rápido

```bash
docker compose build
docker compose up
```

- O estágio `api-app` usa o sample oficial (`mcr.microsoft.com/dotnet/aspnet:10.0`) e publica em `http://localhost:3000`.
- FluentMigrator + seeds executam durante o boot da API.
- Logs ficam em `api/logs/app.log` e `api/logs/nhibernate.log` (montados no container).
- UI em `http://localhost:8080`, consumindo `http://localhost:3000`.

### Variáveis relevantes

| Variável | Padrão | Descrição |
| --- | --- | --- |
| `Database__ConnectionString` | `Server=db;Port=3306;Database=ruby_demo_development;Uid=ruby-demo;Pwd=2u8y-c0d3;...` | DSN utilizado pelo NHibernate/FluentMigrator. |
| `VINICIUS_PUBLIC_KEY` | chave fake usada no seed | Substituível para regenerar a seed. |
| `Logging:Directory` | `../logs` | Diretório onde Serilog grava `app.log` e `nhibernate.log`. |

## Desenvolvimento fora do Docker

```bash
cd api
dotnet restore
dotnet tool restore
dotnet build
dotnet run --project DotnetDemo.Api
```

- Configure um MySQL local com as mesmas credenciais ou aponte `Database__ConnectionString` para outro host.
- Os logs ficarão em `api/logs/`.

## Testes automatizados

- **API**: `cd api && dotnet test` – utiliza WebApplicationFactory com SQLite on-disk (`Data Source=...db`) + migrations + seeds reais. Cobre CRUD, `/count_by_author`, `/up`, `/metrics` e `/tech`, garantindo que a plataforma continue evoluindo como CMS de alta performance.
- **UI**: `cd ui && npm install && npm run test:unit`.

## UI (Vue + BootstrapVue)

```bash
cd ui
npm install
npm run serve        # http://localhost:8080 (envs VUE_APP_* apontam para http://localhost:3000)
npm run build        # gera dist/ usada pelo estágio NGINX
```

Os services (`articlesService`, `authorsService`, `socialsService`) continuam exatamente iguais ao projeto Ruby – os endpoints e payloads permanecem compatíveis.

## Área administrativa (/admin)

- Login JWT dedicado em `http://localhost:8080/admin/login`, inspirado no layout Bootstrap Dashboard, mas reaproveitando o `SiteLayout`.
- **Credenciais seed:**
  - Docker/produção (`ASPNETCORE_ENVIRONMENT=Production`): `admin` / `change-me-please`
  - Desenvolvimento (`ASPNETCORE_ENVIRONMENT=Development`): `admin` / `change-me-in-dev`
- Funcionalidades principais:
  - Listagem com DataTable local (paginação, busca e ordenação) + ações por ícones.
  - Botão “Novo” para abrir o mesmo formulário em modo criação.
  - Editor Summernote (servido/localmente) para o campo de conteúdo, sem dependência de CDN.
  - Slug pode ser informado manualmente; se deixado em branco, é gerado automaticamente no backend com verificação de unicidade.
- Para personalizar as credenciais seed, ajuste `Seeds:AdminUser` no `appsettings*.json` antes de subir a API.
