# Fiap Cloud Games (FCG) - Usuário API

Este microsserviço atua como o motor de **identidade, cadastro e autenticação** da plataforma **Fiap Cloud Games (FCG)**. Desenvolvido com **.NET 9** e estruturado seguindo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, o projeto garante robustez, segurança e controle total sobre o ciclo de vida das contas de usuário.

A principal responsabilidade deste serviço é cadastrar e autenticar os usuários da plataforma, orquestrando eventos e gerenciando o controle de acesso baseado em permissões (Roles).

---

## 🔄 Fluxos de Integração e Autenticação

A **Usuários API** centraliza as regras de segurança e integração inicial de identidade na plataforma:

### 1. Cadastro e Produção de Eventos
*   Ao realizar o cadastro de um novo usuário na plataforma, o serviço salva os dados com segurança (hash de senha) e **produz** o evento `UserCreatedEvent`.
*   Este evento é publicado no **RabbitMQ** e consumido nativamente pela *Notifications-API*, que se encarregará de enviar o e-mail de boas-vindas.

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

## 🛠️ Tecnologias e Bibliotecas

A API faz uso das seguintes tecnologias e pacotes:

- **.NET 9.0**: Plataforma de desenvolvimento principal.
- **Entity Framework Core 9.0**: ORM para persistência dos dados no **SQL Server**.
- **MediatR (v14)**: Implementação do padrão Mediator para suporte a **CQRS** (Command Query Responsibility Segregation).
- **Dapper**: Micro-ORM de alta performance utilizado na camada de leitura (Queries).
- **BCrypt.Net-Next**: Hashing seguro e criptografia de senhas dos usuários.
- **System.IdentityModel.Tokens.Jwt**: Criação e manipulação de JSON Web Tokens (JWT) para autenticação.
- **MassTransit**: Biblioteca de mensageria para simplificar a comunicação assíncrona (envio do `UserCreatedEvent`).
- **xUnit & Moq**: Frameworks para execução de testes de unidade e integração.

### 📦 Shared Kernels (Pacotes Compartilhados da FCG)
Projetos criados internamente e utilizados para padronizar e compartilhar recursos base:
- **Fcg.Core.Abstractions**: Biblioteca de blocos de construção para Domain-Driven Design (DDD) em projetos .NET.
- **Fcg.Core.SharedContracts**: Contratos de eventos compartilhados para comunicação assíncrona entre os microsserviços da plataforma FCG (ex: RabbitMQ / MassTransit).
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
- **Padrão Outbox Transacional**: O envio do `UserCreatedEvent` para o RabbitMQ é persistido na mesma transação atômica do Entity Framework (Outbox Pattern nativo do MassTransit). Garante que nenhuma mensagem seja perdida na integração com as Notificações, mesmo se o RabbitMQ cair no momento do cadastro.
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
  "RabbitMqSettings": {
    "Host": "localhost",
    "Port": "5672",
    "Username": "guest",
    "Password": "guest"
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
| `RabbitMqSettings__Host` | `rabbitmq` | Host do RabbitMQ para consumo e publicação de eventos. |
| `RabbitMqSettings__Port` | `5672` | Porta TCP do RabbitMQ. |
| `RabbitMqSettings__Username` | `guest` | Usuário de acesso ao RabbitMQ. |
| `RabbitMqSettings__Password` | `${RABBITMQ_PASS}` | Senha do RabbitMQ injetada. |
| `JwtSettings__Secret` | `${JWT_SECRET}` | Chave de assinatura e validação dos tokens JWT injetada do `.env`. |

No **Kubernetes**, estas variáveis são providas via ConfigMaps e Secrets contidos nos manifestos de implantação.

---

## 🚀 Como Executar Localmente

### Pré-requisitos
- SDK do [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
- Banco de dados SQL Server e RabbitMQ acessíveis.

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
- **Readiness Probe**: Verifica conectividade com SQL Server e RabbitMQ antes de aceitar requisições externas e enviar tráfego para os pods (`/health/readiness`).

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
