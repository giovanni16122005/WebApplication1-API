# WebApplication1 API

API REST desenvolvida em **ASP.NET Core** com integração ao **PostgreSQL** e autenticação utilizando **JWT**.

O projeto foi desenvolvido durante a graduação em Ciência da Computação com o objetivo de praticar o desenvolvimento de APIs, persistência de dados e autenticação de usuários.

## Tecnologias

- C#
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT (JSON Web Token)
- Swagger / OpenAPI
- Git e GitHub

## Funcionalidades

- Autenticação de usuários
- Login com geração de token JWT
- Criação de contas
- Alteração de senha
- Recuperação de senha
- Persistência de usuários utilizando PostgreSQL
- API REST com endpoints organizados em Controllers
- Documentação e testes dos endpoints através do Swagger

## Estrutura do projeto

```text
WebApplication1/
├── Controllers/
│   ├── AuthController.cs
│   └── WeatherForecastController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   ├── ChangePasswordRequest.cs
│   ├── CreateAccountRequest.cs
│   ├── ForgotPasswordRequest.cs
│   ├── LoginRequest.cs
│   └── Usuario.cs
│
├── Properties/
│   └── launchSettings.json
│
├── Program.cs
├── ServiceLog.cs
├── WeatherForecast.cs
└── WebApplication1.csproj
```

## Autenticação

A API utiliza **JWT (JSON Web Token)** para autenticação.

Após realizar o login com credenciais válidas, a API gera um token que pode ser utilizado para acessar endpoints protegidos.

As credenciais e chaves de autenticação são mantidas fora do repositório público por meio do `.gitignore`.

## Banco de dados

O projeto utiliza **PostgreSQL** para armazenamento dos dados dos usuários.

A comunicação com o banco é realizada através do **Entity Framework Core**, utilizando o `AppDbContext` para gerenciamento da persistência.

## Como executar

### 1. Pré-requisitos

- .NET SDK
- PostgreSQL
- Visual Studio ou Visual Studio Code

### 2. Configurar o banco

Crie um banco PostgreSQL local e configure a string de conexão no arquivo:

```text
appsettings.json
```

> O arquivo `appsettings.json` não é versionado neste repositório por conter informações de configuração sensíveis.

### 3. Executar a aplicação

Na pasta do projeto:

```bash
dotnet restore
dotnet run
```

### 4. Acessar o Swagger

Após iniciar a aplicação, acesse a URL apresentada no terminal para abrir a documentação interativa da API.

## Objetivo do projeto

Este projeto foi desenvolvido como prática de desenvolvimento backend, envolvendo:

- Desenvolvimento de APIs REST;
- Integração com banco de dados;
- Autenticação e autorização;
- Manipulação de requisições HTTP;
- Organização de código em Controllers, Models e Data;
- Utilização de ferramentas do ecossistema .NET.

## Projeto acadêmico

Projeto desenvolvido durante a graduação em **Ciência da Computação**.
