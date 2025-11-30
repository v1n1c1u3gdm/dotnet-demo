# .NET Demo (ASP.NET Core + NHibernate)

Repositório reescrito com **ASP.NET Core 10** sobre Kestrel, NHibernate e MySQL containerizado. A raiz continua dividida em dois módulos:

- `api/` – solução .NET com `DotnetDemo.Api` (Web API) e `DotnetDemo.Tests` (xUnit + WebApplicationFactory).
- `ui/` – front-end Vue 2 + BootstrapVue, consumindo os mesmos endpoints e com suíte Jest ≥85% de cobertura.

## Arquitetura & Tecnologias

- **API (`api/`)**:
  - ASP.NET Core (Kestrel) + `Startup.cs` tradicional que executa FluentMigrator + seeding logo no boot.
  - NHibernate + FluentNHibernate mappings para `Author`, `Article` e `Social`, com `StringListJsonType` para o campo `tags`.
  - Migrations FluentMigrator equivalentes às do projeto Ruby (criação das tabelas e remoção de `url`), rodando automaticamente.
  - Seeds idempotentes lendo o mesmo dataset (`Seeds/article_seed_data.json`) e respeitando `VINICIUS_PUBLIC_KEY`.
  - Swagger em `/api-docs`, redirect padrão em `/`, e HTML "tabelaço" em `/tech`.
  - Observabilidade com OpenTelemetry Metrics + Prometheus exporter (`GET /metrics`) e middleware customizado para contadores/histogramas.
  - Logging via Serilog (console + arquivos) com `api/logs/app.log` e `api/logs/nhibernate.log`.
- **UI (`ui/`)**: Vue 2 (Vue CLI) + BootstrapVue, build multi-stage (Node → NGINX) compartilhado no mesmo `Dockerfile`. As variáveis `VUE_APP_*` são injetadas na build para apontar para `http://localhost:3000`.
- **Banco (`db/`)**: MySQL 8.4 em container dedicado com volume `mysql_data` e credenciais fixas (`ruby-demo` / `2u8y-c0d3`).
- **Orquestração**: `docker-compose.yml` agora referencia o `Dockerfile` oficial da Microsoft (sample `aspnetapp`), publica a API em `:3000` (container `8080`) e monta o diretório de logs.

## Stack

- .NET SDK 9 localmente (o container já usa `mcr.microsoft.com/dotnet/sdk:10.0`/`aspnet:10.0`).
- ASP.NET Core + Kestrel + Swagger/Swashbuckle.
- NHibernate + FluentNHibernate + FluentMigrator.
- Serilog (console + file) com interceptador de SQL.
- OpenTelemetry Metrics + Prometheus scraping endpoint.
- xUnit + FluentAssertions + WebApplicationFactory + SQLite para integração.
- Vue 2, BootstrapVue, Jest, Vue Test Utils.
- Docker 24 + Docker Compose v2.

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

- **API**: `cd api && dotnet test` – utiliza WebApplicationFactory com SQLite on-disk (`Data Source=...db`) + migrations + seeds reais. Cobre CRUD, `/count_by_author`, `/up`, `/metrics` e `/tech`.
- **UI**: `cd ui && npm install && npm run test:unit`.

## UI (Vue + BootstrapVue)

```bash
cd ui
npm install
npm run serve        # http://localhost:8080 (envs VUE_APP_* apontam para http://localhost:3000)
npm run build        # gera dist/ usada pelo estágio NGINX
```

Os services (`articlesService`, `authorsService`, `socialsService`) continuam exatamente iguais ao projeto Ruby – os endpoints e payloads permanecem compatíveis.*** End Patch
