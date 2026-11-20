# Fiap Cloud Games (FCG) - Usuário API

Este microsserviço atua como o motor de **identidade, cadastro e autenticação** da plataforma **Fiap Cloud Games (FCG)**. Desenvolvido com **.NET 9** e estruturado seguindo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, o projeto garante robustez, segurança e controle total sobre o ciclo de vida das contas de usuário.

A principal responsabilidade deste serviço é cadastrar e autenticar os usuários da plataforma, orquestrando eventos e gerenciando o controle de acesso baseado em permissões (Roles).

---

## 🔄 Fluxos de Integração e Autenticação

A **Usuários API** centraliza as regras de segurança e integração inicial de identidade na plataforma:

### 1. Cadastro e Produção de Eventos
*   Ao realizar o cadastro de um novo usuário na plataforma, o serviço salva os dados com segurança (hash de senha) e **produz** o evento `UserCreatedEvent`.
*   Este evento é publicado no **Azure Service Bus** (tópico `user-created`) e consumido pela *Notification Function*, que se encarrega de enviar o e-mail de boas-vindas, e pela *Order API*, que cria o *snapshot* local do cliente.

### 🚌 Mensageria com Azure Service Bus

A publicação dos eventos usa o **MassTransit** com `UsingAzureServiceBus`. Cada contrato vai para um **tópico com nome fixo** (`SetEntityName` com as constantes da classe `Topics` do `Fcg.Core.SharedContracts`), e cada serviço interessado lê o tópico pela sua própria *subscription*. A Usuários API é **publicadora pura**: não possui *consumers* e, por isso, a configuração exige apenas a connection string do namespace.

| Evento | Tópico | Publicado quando | Consumido por |
| :--- | :--- | :--- | :--- |
| `IUserCreatedIntegrationEvent` | `user-created` | Um usuário se cadastra. | Notification Function (e-mail de boas-vindas), Order API (*snapshot* do cliente) |
| `IUserDeactivatedIntegrationEvent` | `user-deactivated` | Uma conta é desativada (Soft Delete). | User Library API (suspende o acervo) |
| `IUserReactivatedIntegrationEvent` | `user-reactivated` | Uma conta é reativada. | User Library API (reativa o acervo) |
| `IRepublishUserIntegrationEvent` | `user-republished` | Um Admin solicita a republicação dos usuários. | Notification Function (sincroniza os dados de contato) |

A publicação passa pelo **Outbox Transacional** do MassTransit (tabelas no SQL Server) e conta com retry de 3 tentativas com intervalo de 5 segundos.

### 2. Autenticação e Perfis (Roles)
*   A API é responsável por validar as credenciais e **gerar um Token JWT** customizado de acordo com o perfil (Role) do usuário autenticado (ex: `Jogador`, `Administrador`).
*   Um usuário com permissões de **Admin** é capaz de executar operações completas de **CRUD** sobre as contas da plataforma, incluindo a exclusão de usuários (que é realizada na forma de **Soft Delete**, inativando o registro em vez de removê-lo fisicamente do banco de dados).

---

## 🔐 Credenciais de Teste (Admin)

Para facilitar a avaliação das funcionalidades administrativas (CRUD completo) protegidas pelas rotas com perfil *Administrador*, utilize o usuário pré-configurado da inicialização do banco:

*   **E-mail:** `admin@fiapcloudgames.com.br`
*   **Senha:** `SenhaAdmin@123`

> [!TIP]
> Realize o login no endpoint `/api/User/login` pelo Swagger para obter o Token JWT. Em seguida, utilize o botão **Authorize** (cadeado) para injetar o header de autorização nas chamadas que exigem perfil de Admin.

---

## 📊 Métricas (Prometheus)

A API expõe métricas no formato Prometheus em `/metrics`, habilitadas por `app.UseHttpMetrics()` (duração, contagem e status code de cada rota, coletados automaticamente) e `app.MapMetrics()`.

Além das métricas HTTP padrão, há métricas de negócio customizadas, definidas em `AuthMetrics` e incrementadas nos respectivos Command Handlers:

| Métrica | Tipo | Labels | Descrição |
| :--- | :--- | :--- | :--- |
| `auth_login_attempts_total` | Counter | `result` (`success` \| `failure`) | Tentativas de autenticação (`AuthenticateUserCommandHandler`). |
| `auth_register_attempts_total` | Counter | `result` (`success` \| `failure`) | Tentativas de cadastro de usuário (`RegisterUserCommandHandler`). |
| `auth_deactive_attempts_total` | Counter | `result` (`success` \| `failure`) | Tentativas de desativação da própria conta (`DeactivateAccountCommandHandler`). |
| `auth_update_attempts_total` | Counter | `result` (`success` \| `failure`) | Tentativas de atualização de dados cadastrais (`UpdateUserCommandHandler`). |
| `user_admin_actions_total` | Counter | `action` (`promote` \| `demote` \| `deactivate` \| `reactivate`), `result` (`success` \| `failure`) | Ações administrativas executadas sobre contas de outros usuários. |

---

## 🛠️ Tecnologias e Bibliotecas

A API faz uso das seguintes tecnologias e pacotes:

- **.NET 9.0**: Plataforma de desenvolvimento principal.
- **Entity Framework Core 9.0**: ORM para persistência dos dados no **SQL Server**.
- **MediatR (v14)**: Implementação do padrão Mediator para suporte a **CQRS** (Command Query Responsibility Segregation).
- **Dapper**: Micro-ORM de alta performance utilizado na camada de leitura (Queries).
- **BCrypt.Net-Next**: Hashing seguro e criptografia de senhas dos usuários.
- **System.IdentityModel.Tokens.Jwt**: Criação e manipulação de JSON Web Tokens (JWT) para autenticação.
- **MassTransit & Azure Service Bus**: Mensageria para a publicação assíncrona dos eventos de usuário em tópicos do **Azure Service Bus**, com Outbox Transacional baseado em EF Core.
- **Prometheus (prometheus-net.AspNetCore)**: Exposição de métricas HTTP e de negócio no formato Prometheus.
- **xUnit & Moq**: Frameworks para execução de testes de unidade e integração.

### 📦 Shared Kernels (Pacotes Compartilhados da FCG)
Projetos criados internamente e utilizados para padronizar e compartilhar recursos base:
- **Fcg.Core.Abstractions**: Biblioteca de blocos de construção para Domain-Driven Design (DDD) em projetos .NET.
- **Fcg.Core.SharedContracts**: Contratos de eventos compartilhados para comunicação assíncrona entre os microsserviços da plataforma FCG via Azure Service Bus / MassTransit, incluindo a classe `Topics` com os nomes dos tópicos.
- **Fcg.Core.WebApi**: Utilitários prontos para uso em ASP.NET Core Web APIs, incluindo autenticação JWT e tratamento global de exceções.

---

## 🏗️ Arquitetura da Solução

O projeto está estruturado em camadas para separar responsabilidades de forma clara e evitar acoplamento:

```
src/
├── Fcg.Users.Domain         # Regras de Negócio, Entidades de Domínio e Invariantes
├── Fcg.Users.Application    # Casos de Uso (Commands/Queries), DTOs e Handlers (MediatR)
├── Fcg.Users.Infrastructure # Persistência de Dados, Configuração de Segurança e Serviços Externos
└── Fcg.Users.API            # Host da API HTTP, Middlewares e Ponto de Entrada (Program.cs)
```

### Detalhamento das Camadas

#### 1. `Fcg.Users.Domain` (Domínio)
Contém o coração da regra de negócio:
- **Entidades**: O agregado raiz `User`, que encapsula o comportamento de desativação (soft delete), reativação e troca de senhas.
- **Objetos de Valor**: Validações de domínio robustas nativas.
- **Enums**: Definição de perfis de acesso (`Administrador`, `Jogador`).

#### 2. `Fcg.Users.Application` (Aplicação)
Orquestra os fluxos de dados usando **CQRS**:
- **Commands & Queries**: Lógica segregada entre leitura (Dapper) e gravação (EF Core). 

#### 3. `Fcg.Users.Infrastructure` (Infraestrutura)
- **Persistência**: `UserDbContext` com mapeamento fluente.
- **Padrão Outbox Transacional**: O envio dos eventos de usuário para o Azure Service Bus é persistido na mesma transação atômica do Entity Framework (Outbox Pattern nativo do MassTransit). Garante que nenhuma mensagem seja perdida na integração com os demais serviços, mesmo se o Azure Service Bus estiver indisponível no momento do cadastro.
- **Mensageria (`MessageBrokerExtensions`)**: MassTransit com `UsingAzureServiceBus`, mapeamento dos contratos para os tópicos (`SetEntityName(Topics.*)`) e retry. A connection string vem de `AzureServiceBusSettings`, validada na subida (`ValidateOnStart`).
- **Segurança**: Serviço para gerar JWT e encriptar senhas.

#### 4. `Fcg.Users.API` (Apresentação)
O ponto de partida da aplicação responsável pelo bootstrap e exposição dos serviços HTTP via Swagger.

---

## 🚀 Funcionalidades Principais (CRUD e Controles)

O microsserviço gerencia todo o ciclo de vida do usuário:

1. **Cadastro de Usuário**: Permite que novos jogadores se cadastrem na plataforma.
2. **Autenticação**: Valida credenciais do usuário e retorna um JWT ativo contendo as claims da Role associada.
3. **Gestão de Perfis (Admin)**: Promover jogadores a administradores e vice-versa.
4. **CRUD de Usuários (Admin)**: Atualização de dados cadastrais e visualização global de usuários.
5. **Deleção Lógica (Soft Delete)**: O Administrador (ou o próprio usuário logado) pode excluir a conta. Em vez de deletar o registro permanentemente do SQL Server, a plataforma realiza um **Soft Delete**, inativando a conta e registrando o motivo e a data da desativação.

---

## ⚙️ Configuração e Variáveis de Ambiente

Para o funcionamento local stand-alone, configure o arquivo `appsettings.json`:

```json
{
  "DatabaseSettings": {
    "Host": "localhost",
    "Port": "1433",
    "DatabaseName": "Fcg_Users",
    "Username": "sa",
    "Password": "SuaSenhaSegura"
  },
  "AzureServiceBus": {
    "ConnectionString": "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<policy>;SharedAccessKey=<chave>"
  },
  "JwtSettings": {
    "Secret": "SUA_CHAVE_SUPER_SECRETA_E_LONGA_DE_EXEMPLO",
    "ExpirationHours": 2,
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGamesAudience"
  }
}
```

---

### 2. Execução via Docker Compose
Ao rodar através do contêiner Docker configurado no repositório de orquestração (`fcg-infrastructure`), as seguintes variáveis de ambiente são injetadas no contêiner para sobrescrever os valores do `appsettings.json`:

| Variável | Valor Padrão/Exemplo | Descrição |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | `Development` | Define o ambiente de execução da aplicação. |
| `DatabaseSettings__Host` | `fcg-db-central` | Host do banco SQL Server centralizado. |
| `DatabaseSettings__DatabaseName` | `Fcg_Users` | Nome do banco de dados de Usuários. |
| `DatabaseSettings__Port` | `1433` | Porta TCP do SQL Server. |
| `DatabaseSettings__Username` | `sa` | Usuário de acesso ao banco de dados. |
| `DatabaseSettings__Password` | `${DB_PASS}` | Senha do banco injetada. |
| `AzureServiceBus__ConnectionString` | `${SERVICE_BUS_CONNECTION_STRING}` | Connection string do namespace do Azure Service Bus (tier Standard ou superior), usada para publicar os eventos nos tópicos. |
| `JwtSettings__Secret` | `${JWT_SECRET}` | Chave de assinatura e validação dos tokens JWT injetada do `.env`. |

No **Kubernetes**, estas variáveis são providas via ConfigMap (`k8s/user-configmap.yaml`) e segredos do **Azure Key Vault** (`fcg-secrets`), incluindo `AzureServiceBus__ConnectionString`.

---

## 🚀 Como Executar Localmente

### Pré-requisitos
- SDK do [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
- Banco de dados SQL Server acessível.
- Um namespace do **Azure Service Bus** (tier Standard ou superior) e a sua connection string.

1. **Restaurar e Compilar:**
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Aplicar Migrações do Banco de Dados (EF Core):**
   ```bash
   dotnet ef database update --project src/Fcg.Users.Infrastructure/ --startup-project src/Fcg.Users.API/
   ```

3. **Executar a API:**
   ```bash
   dotnet run --project src/Fcg.Users.API/
   ```
   Acesse o Swagger interativo em: `http://localhost:8081/swagger`

---

## ☸️ Implantação no Kubernetes (k8s)

A orquestração está presente no repositório de infraestrutura central, mas os manifestos primordiais residem em `/k8s`. A API está exposta nativamente através de NodePort na porta `30000` (ex.: `http://localhost:30000/swagger`).

### 🏥 Resiliência e Probes de Saúde
- **Liveness Probe**: Verifica se a aplicação travou (`/health/liveness`).
- **Readiness Probe**: Verifica conectividade com o SQL Server antes de aceitar requisições externas e enviar tráfego para os pods (`/health/readiness`).

---

## 🧪 Testes Automatizados

O projeto utiliza **xUnit** para garantir a estabilidade:
- **Fcg.Users.Domain.Tests:** Testes unitários focados nas invariantes, criação de entidades e restrições de contas.
- **Fcg.Users.Application.Tests:** Validação dos manipuladores (Handlers) isolados via mocks.
- **Fcg.Users.Infrastructure.Integration:** Testes de persistência transacional com banco de dados in-memory ou real.

Para executar todos os testes da solução de uma vez, rode o comando na raiz:
```bash
dotnet test
```
