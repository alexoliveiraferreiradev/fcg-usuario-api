# Fiap Cloud Games (FCG) - Usuário API

Este microsserviço é responsável por toda a **gestão de identidade, autenticação e autorização** da plataforma **Fiap Cloud Games (FCG)**. Desenvolvido com **.NET 9** e estruturado seguindo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, o projeto garante robustez, segurança e manutenibilidade para as operações de cadastro de usuários, autenticação via tokens JWT e controle de perfis.

---

## 🔐 Credenciais de Teste (Admin)

Para facilitar a avaliação das funcionalidades administrativas (como a criação de jogos, jogadores e promoções), utilize o usuário pré-configurado:

*   E-mail: admin@fiapcloudgames.com.br

*   Senha: SenhaAdmin@123

Nota: Realize o login no endpoint /login para obter o Token JWT e utilize o botão Authorize do Swagger para enviar o cabeçalho de autorização.


---

## 🛠️ Tecnologias e Bibliotecas

A API faz uso das seguintes tecnologias e pacotes:

- **.NET 9.0**: Plataforma de desenvolvimento principal.
- **Entity Framework Core 9.0**: ORM para persistência dos dados no **SQL Server**.
- **MediatR (v14)**: Implementação do padrão Mediator para suporte a **CQRS** (Command Query Responsibility Segregation).
- **Dapper**: Micro-ORM de alta performance utilizado na camada de leitura (Queries) para otimização de consultas complexas.
- **BCrypt.Net-Next**: Hashing seguro e criptografia de senhas dos usuários.
- **System.IdentityModel.Tokens.Jwt**: Criação e manipulação de JSON Web Tokens (JWT) para autenticação.
- **MassTransit**: Biblioteca/Framework de mensageria para simplificar a comunicação assíncrona com RabbitMQ.
- **xUnit**: Framework para execução de testes de unidade e integração.

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
Contém o coração da regra de negócio, livre de dependências de frameworks externos:
- **Entidades**: O agregado raiz `User`, que encapsula o comportamento do usuário e do ciclo de vida da conta.
- **Objetos de Valor (Value Objects)**: `Nome`, `Email` e `Senha`, com auto-validação das suas respectivas regras no construtor.
- **Repositórios**: A interface `IUserRepository`, que define os contratos de banco de dados a serem implementados pela infraestrutura.
- **Enums**: Definição de perfis (`Administrador`, `Jogador`) e motivos de desativação (`MotivoDesativacao`).

#### 2. `Fcg.Users.Application` (Aplicação)
Orquestra os fluxos de dados e implementa os casos de uso usando **CQRS**:
- **Commands & Queries**: Separados por escopo (ex.: `CadastrarUserCommand`, `ObterUserPorIdQuery`), convertidos em **C# records** para imutabilidade.
- **Handlers**: Processam os comandos e consultas via `IRequestHandler` do MediatR.
- **Interfaces**: Definição de serviços utilitários como `ITokenService`.

#### 3. `Fcg.Users.Infrastructure` (Infraestrutura)
Lida com preocupações transversais, frameworks e infraestrutura técnica:
- **Persistência**: Implementação do `UserDbContext` configurado via EF Core, utilizando mapeamento fluente (`UserConfiguration`).
- **Padrão Outbox Transacional (Outbox Pattern)**: Configuração do Outbox transacional via EF Core e MassTransit (`AddEntityFrameworkOutbox`). Isso assegura que a publicação de mensagens no RabbitMQ (como o evento `UserCreatedEvent`) seja transacionada atomicamente junto às alterações do banco de dados, garantindo consistência eventual e confiabilidade mesmo em momentos de indisponibilidade parcial da rede ou broker.
- **Segurança**:
  - `PasswordHasher`: Responsável por gerar hashes seguros usando BCrypt e comparar senhas.
  - `TokenService`: Gera tokens JWT com claims de identidade e perfis (`AdminRole`, `JogadorRole`).
- **Repositórios Concretos**: Implementação da persistência de dados em `UserRepository`.

#### 4. `Fcg.Users.API` (Apresentação)
O ponto de partida da aplicação responsável pelo bootstrap e exposição dos serviços HTTP.

---

## 🚀 Funcionalidades Principais (Casos de Uso)

O microsserviço gerencia todo o ciclo de vida do usuário:

1. **Cadastro de Usuário (`CadastrarUserCommand`)**: Permite que novos jogadores se cadastrem com e-mail, nome e senha.
2. **Autenticação (`AutenticarUserCommand`)**: Valida credenciais do usuário e retorna um token JWT ativo.
3. **Gerenciamento de Perfis**:
   - Promover Jogador a Administrador (`PromoverUserParaAdminCommand`).
   - Rebaixar Administrador a Jogador (`RebaixarUserParaJogadorCommand`).
4. **Atualização de Cadastro (`AtualizarUserCommand`)**: Altera nome e/ou senha do usuário logado.
5. **Ciclo de Conta (Ativo / Inativo)**:
   - Desativação de conta pelo próprio usuário (`DesativarContaCommand`).
   - Desativação de conta por um Administrador, informando o motivo (`DesativarUserCommand`).
   - Reativação de conta previamente desativada (`ReativarContaCommand`).
6. **Consultas (Queries)**:
   - Obter detalhes de um usuário por ID (`ObterUserPorIdQuery`).
   - Listar todos os usuários cadastrados (`ObterTodosUsersQuery`).

---

## ⚙️ Configuração e Variáveis de Ambiente

Para o funcionamento correto do microsserviço de Usuários, certas variáveis de ambiente de banco de dados, mensageria e autenticação devem ser fornecidas dependendo do ambiente de execução.

### 1. Execução Local Standalone (Desenvolvimento)
Quando executada diretamente pela IDE ou linha de comando `dotnet run`, a API consome as configurações definidas no arquivo [appsettings.json](src/Fcg.Users.API/appsettings.json) ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "UserConnection": "Server=localhost;Database=Fcg_Users;User Id=sa;Password=SuaSenhaSegura;TrustServerCertificate=True;",
    "RabbitMq": "amqp://guest:guest@localhost:5672",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "Secret": "SUA_CHAVE_SUPER_SECRETA_E_LONGA_DE_EXEMPLO",
    "Emissor": "Fcg.Users.API",
    "ValidoEm": "FiapCloudGames",
    "ExpiracaoHoras": 2
  }
}
```

---

### 2. Execução via Docker Compose
Ao rodar através do contêiner Docker configurado no repositório de orquestração (`fcg-infrastructure`), as seguintes variáveis de ambiente são injetadas no contêiner:

| Variável | Valor Padrão/Exemplo | Descrição |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | `Development` | Define o ambiente de execução da aplicação. |
| `ConnectionStrings__UserConnection` | `Server=fcg-db-central;Database=Fcg_Users;...` | String de conexão para o banco SQL Server centralizado. |
| `ConnectionStrings__RabbitMq` | `rabbitmq` | Host do RabbitMQ para publicação do `UserCreatedEvent`. |
| `ConnectionStrings__Redis` | `redis:6379` | Host do cache Redis (usado para token blacklist ou sessões). |
| `JwtSettings__Secret` | `${JWT_SECRET}` | Chave de assinatura dos tokens JWT injetada do `.env`. |

---

### 3. Execução no Kubernetes (ConfigMaps e Secrets)
No Kubernetes, as configurações são abstraídas em manifestos separados ConfigMaps e Secrets:

#### **ConfigMap: `user-config`**
Armazena dados não sensíveis configurados no arquivo [configmap.yaml](k8s/configmap.yaml):
- `DB_SERVER`: Nome do serviço DNS do banco de dados no cluster.
- `DB_PORT`: Porta TCP do SQL Server.
- `DB_NAME`: Nome lógico do banco de dados.
- `DB_TRUST_CERT`: Permitir certificados autoassinados.
- `RABBITMQ_SERVER`: Nome do serviço DNS do RabbitMQ no cluster.
- `RABBITMQ_PORT`: Porta TCP do RabbitMQ.
- `ENVIRONMENT`: Variável `ASPNETCORE_ENVIRONMENT`.

#### **Secret: `user-opaque`**
Armazena credenciais confidenciais codificadas em Base64 configuradas no arquivo [secrets.yaml](k8s/secrets.yaml):
- `DB_USER`: Usuário de acesso ao banco.
- `DB_PASS`: Senha do banco.
- `JWT_SECRET`: Chave simétrica do token.
- `RABBITMQ_USER`: Usuário do RabbitMQ.
- `RABBITMQ_PASS`: Senha do RabbitMQ.

---

## 🚀 Como Executar Localmente (Standalone)

### Pré-requisitos
- SDK do [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
- Banco de dados SQL Server e RabbitMQ acessíveis.

### Comandos de Terminal

1. **Restaurar Dependências e Compilar:**
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Aplicar Migrações do Banco de Dados (EF Core):**
   Certifique-se de que a Connection String do SQL Server esteja acessível no seu `appsettings.json` local e execute:
   ```bash
   dotnet ef database update --project src/Fcg.Users.Infrastructure/ --startup-project src/Fcg.Users.API/
   ```

3. **Executar a API:**
   ```bash
   dotnet run --project src/Fcg.Users.API/
   ```
   A API ficará acessível localmente e o painel de documentação Swagger poderá ser acessado em: `http://localhost:8081/swagger` (ou a porta definida no seu arquivo de perfil `launchSettings.json`).

---

## 🐳 Construção da Imagem Docker

Para validar e construir a imagem Docker do microsserviço de forma isolada, provando o funcionamento de sua otimização de múltiplos estágios (multi-stage build) e injetando as credenciais do GitHub Packages para restauração de pacotes, execute o seguinte comando a partir da raiz deste repositório:

```bash
docker build --secret id=github_token,env=GITHUB_TOKEN -t fcg-users-api .
```

---

## ☸️ Como Implantar no Kubernetes (k8s)

Os manifestos na pasta `/k8s` estão prontos para implantação local. Para aplicar todas as configurações, serviços de rede e deployments do microsserviço de usuários no cluster local configurado, execute:

```bash
kubectl apply -f k8s/
```

### Validação dos Recursos
Para validar se a implantação foi bem-sucedida, você pode executar:
```bash
# Verificar status dos pods (deve constar como Running)
kubectl get pods

# Verificar serviços expostos e a porta do NodePort
kubectl get services
```

*Nota: Por padrão, o serviço `svc-fcg-users-api` é do tipo **NodePort**, expondo a API fisicamente no localhost na porta **`30000`** para fácil acesso do avaliador.*

### 🏥 Resiliência e Probes de Saúde (Health Checks)

A API possui suporte nativo para verificação de integridade operacional através dos seguintes endpoints expostos:
- **Liveness Probe (`/health/liveness`)**: Utilizada pelo Kubernetes para detectar se a aplicação travou ou precisa ser reiniciada.
- **Readiness Probe (`/health/readiness`)**: Utilizada pelo Kubernetes para verificar se todas as conexões de infraestrutura necessárias (Banco de Dados SQL Server e RabbitMQ) estão ativas e prontas para receber requisições antes de direcionar tráfego de rede para os pods.

Esses endpoints estão totalmente integrados nos manifestos do Kubernetes (`k8s/deployment.yaml`), elevando o nível de resiliência e estabilidade do cluster.

---

## 🧪 Testes Automatizados

O repositório conta com suítes de testes robustas organizadas na pasta `/tests`:
- **Fcg.Users.Domain.Tests:** Testes unitários focados nas entidades e Value Objects.
- **Fcg.Users.Application.Tests:** Testes unitários para casos de uso, validando comandos, consultas e handlers com mocks.
- **Fcg.Users.Infrastructure.Integration:** Testes de integração cobrindo persistência de dados.

Para executar todos os testes da solução, rode o comando a partir do diretório raiz:
```bash
dotnet test
```
