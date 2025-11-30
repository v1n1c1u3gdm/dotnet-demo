# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [2025-11-30]

### Added
- Área administrativa `/admin` com autenticação JWT, DataTable local, botões de ação por ícones e editor Summernote servido localmente.
- Fluxo de criação de artigos diretamente pelo dashboard (“Novo”) reutilizando o mesmo modal.
- Geração automática de slugs no backend quando o campo é omitido ou entra em conflito (incluindo utilitário `SlugGenerator`).
- Autenticação JWT na API (controller dedicado, serviço de login, seeding de usuário admin com hash PBKDF2 e configuração de `JwtOptions`).
- Documentação no README com credenciais padrão para o admin e instruções específicas.
- `CHANGELOG.md` seguindo o modelo Keep a Changelog.

### Changed
- Layout do `SiteLayout`/`AdminLayout` para permitir cabeçalho em largura total e reaproveitar o espaço principal quando o dashboard está logado.
- Serviço de artigos no backend passou a normalizar e validar slugs únicos antes de persistir ou atualizar.
- String de conexão MySQL passou a forçar `utf8mb4` e o serviço `db` no Docker Compose foi configurado com `character-set-server`/`collation-server`.
- Serialização JSON da API agora utiliza snake_case (`JsonSerializerOptions`) e os testes passaram a usar helpers para requests com esse formato.
- Projeto reorganizado para ASP.NET Core + NHibernate + Vue 2 (commit `6710b16`), removendo arquivos antigos, ajustando README e aplicando MIT License.

### Fixed
- Evitada colisão de slugs e falha 500 durante criação/atualização simultânea de artigos.
- Ajustado seed de datas/tags no `DataSeeder`, tentando interpretar datas brasileiras e limpando tags duplicadas.

## [2025-03-14]
### Added
- Componente de erro dedicado no frontend (`Criando componente de erro`).

## [2025-02-28]
### Added
- Interceptor global de erros, componente de erros e controller de stubs para cenários de falha.

### Changed
- Tratamento global de exceções no backend para padronizar respostas (commit `2793a7b`).

## [2025-02-27]
### Added
- Middleware simples para tratamento global de exceções (commit `2793a7b`).

## [2025-02-26]
### Added
- Bootswatch integrado ao frontend, novos componentes/rotas e melhorias nos guards.
- Ajustes de CSS para seguir BS5, enxugando estilos e atualizando imagens/fluxos (commits `d94b150`, `d4bc749`, `a7b939f`, `8d5863c`, `81d3361`).
- Tentativa de obtenção do nome do usuário via signal e persistência de login com localStorage.

## [2025-02-21]
### Added
- Interface e integração de login na navbar, além de componentes de navegação.
- Pipeline completo de JWT básico: infraestrutura, serviço, testes de login e collection/DTOs atualizados.
- CORS mais permissivo e suporte a bearer tokens nos testes do controller.

### Changed
- Limpeza de estilos pendentes e refactors DRY nas controllers coleções de usuário.
- Ajustes no template, route guards e pipes.

## [2025-02-20]
### Added
- Debug configs, controllers generalizados, migrações para senha/hash e DTOs para autobinding.

## [2025-02-18]
### Added
- Bootstrap configurado no frontend e propriedades de senha no domínio.

## [2025-02-13]
### Added
- Base da API com controllers acessando banco, dados seed e introdução do Angular no frontend.

### Fixed
- Remoção de controllers de exemplo e ajustes no comportamento do conteúdo conforme documentação BS5.

## [2025-02-11]
### Added
- Commit inicial do projeto (estrutura básica).

