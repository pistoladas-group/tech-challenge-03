<h1 align="center">Tech News</h1>
<h4 align="center">Um blog de notícias para a galera tech.</h4>

<p align="center">
  <a href="">
    <img src="https://img.shields.io/badge/version-1.0.0-blue"
         alt="version">
  </a>
  <a href="">
    <img src="https://img.shields.io/badge/license-MIT-green"
         alt="license">
  </a>
</p>

<p align="center">
  <a href="">
    <img src=".github\images\website-demo.png" alt="website-demo">
  </a>
</p>


## Sumário

- [Sobre](#sobre)
- [Tecnologias](#tecnologias)
- [Suporte ao Browser](#suporte-ao-browser)
- [Arquitetura](#arquitetura)
    - [Web App](#web-app)
    - [Core API](#core-api)
    - [Auth API (Authorization Server)](#auth-api-authorization-server)
- [Segurança](#segurança)
    - [Rotação das Chaves](#rotação-das-chaves)
    - [Prevenção contra possíveis ataques](#prevenção-contra-possíveis-ataques)
- [Testes](#testes)
    - [Testes Unitários](#testes-unitários)
    - [Testes de Integração](#testes-de-integração)
    - [Testes de UI/UAT (Interface/Aceitação do Usuário)](#testes-de-uiuat-interfaceaceitação-do-usuário)
- [CI / CD](#ci--cd)
- [Executando a aplicação](#executando-a-aplicação)
    - [Docker](#docker)


# Sobre
Este projeto foi criado para atender os requisitos do projeto Tech Challenge da [Faculdade de Tecnologia - FIAP](https://postech.fiap.com.br/?gclid=Cj0KCQjwnf-kBhCnARIsAFlg49228y9z3y6lf_mWZEekgcxZRZBDavxtRT-zAUNs33TZOJtXpGVMNlAaAue5EALw_wcB).<br>
O sistema do TechNews consiste em três aplicações: 
- Uma aplicação Web App MVC que exibe as notícias do blog.
- Uma API de gerenciamento das notícias (requer autenticação OAuth2).
- Uma API dedicada ao gerenciamento, autenticação e autorização dos usuários.

# Tecnologias

| Web App | API's | ORM | Database
| --- | --- | --- | --- |
| [![bootstrap-version](https://img.shields.io/badge/Bootstrap-5.3.1-purple)](https://getbootstrap.com/)<br>[![fontawesome-version](https://img.shields.io/badge/Font_Awesome-6.4.0-yellow)](https://fontawesome.com/)<br>[![aspnetcore-version](https://img.shields.io/badge/ASP.NET_Core_MVC-7.0-blue)](https://learn.microsoft.com/pt-br/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0)| [![aspnetcore-version](https://img.shields.io/badge/ASP.NET_Core-7.0-blue)](https://learn.microsoft.com/pt-br/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0) | [![dapper-version](https://img.shields.io/badge/EF_Core-7.0-red)](https://learn.microsoft.com/en-us/ef/core/) | ![database](https://img.shields.io/badge/SQL_Server-gray)

# Suporte ao Browser

| <img src="https://user-images.githubusercontent.com/1215767/34348387-a2e64588-ea4d-11e7-8267-a43365103afe.png" alt="Chrome" width="16px" height="16px" /> Chrome | <img src="https://user-images.githubusercontent.com/1215767/34348590-250b3ca2-ea4f-11e7-9efb-da953359321f.png" alt="IE" width="16px" height="16px" /> Internet Explorer | <img src="https://user-images.githubusercontent.com/1215767/34348380-93e77ae8-ea4d-11e7-8696-9a989ddbbbf5.png" alt="Edge" width="16px" height="16px" /> Edge | <img src="https://user-images.githubusercontent.com/1215767/34348394-a981f892-ea4d-11e7-9156-d128d58386b9.png" alt="Safari" width="16px" height="16px" /> Safari | <img src="https://user-images.githubusercontent.com/1215767/34348383-9e7ed492-ea4d-11e7-910c-03b39d52f496.png" alt="Firefox" width="16px" height="16px" /> Firefox |
| :---------: | :---------: | :---------: | :---------: | :---------: |
| Yes | 11+ | Yes | Yes | Yes |


# Arquitetura
Esta é uma visão geral da arquitetura do TechNews.

<p align="center">
  <a href="">
    <img src=".github\images\architecture-overview.png" alt="overview-architecture">
  </a>
</p>

## Web App

A concepção da aplicação foi fundamentada no padrão arquitetural MVC (Model View Controller), sendo implementada por meio do ASP.NET Core.

No âmbito do negócio, sua responsabilidade é requisitar os recursos restritos disponibilizados pela API de notícias e, posteriormente, apresentando esses elementos aos usuários.

Quanto à segurança, a aplicação assume a responsabilidade de transmitir os dados referentes à criação de usuários ou as credenciais de acesso ao Authorization Server. Mais detalhes em [Segurança](#segurança)


Caso as credenciais estiverem corretas, a aplicação irá receber um JWT assinado (JWS - Json Web Signature) e irá autenticar o usuário via cookie. Então, para cada requisição feita à API de notícias o mesmo JWT será enviado pelo header.

<p align="center">
  <a href="">
    <img src=".github\images\webapp-architecture.png" alt="webapp-architecture">
  </a>
</p>

## Core API

Escolhemos uma arquitetura mais simples para a API de notícias, adotado um estilo arquitetural de CRUD.

## Auth API (Authorization Server)

Foi adotado o estilo arquitetural REST (Representational State Transfer) com camadas, utilizando ASP.NET Core.

A camada de <b>Filtros</b> lidam com exceções e tratamento de Model State inválidos.

A camada de <b>Controllers</b> direciona o fluxo das requisições. É responsável por expor os parâmetros públicos da chave assimétrica, realizar chamadas ao Identity para autenticação/autorização do usuário e criar o JWT utilizando as classes de Serviços.

A camada de <b>Data</b> se integra com classes do Identity (User e Role) e com o Entity Framework para mapeamento de dados.

A camada de <b>Services</b> possui serviços com responsabilidades diversas como: gerenciar (buscar ou persistir) a chave privada no Azure Key Vault, assinar um token digitalmente com criptografia RSA ou ECC e a criação da chave assimétrica através de criptografia RSA ou ECC.

O <b>Background Service</b> que vemos abaixo é uma parte da camada de serviços. Ele constitui uma solução simples para a rotação da chave privada que gera os tokens. O ideal é possuir uma solução mais robusta, consistindo em uma aplicação que gerencia a rotação da chave para todas as instâncias de aplicações que a utilizam.

<p align="center">
  <a href="">
    <img src=".github\images\architecture-auth.png" alt="api-architecture">
  </a>
</p>

# Segurança

A orquestração do fluxo de autenticação do Tech News foi fundamentada na documentação do [OAuth 2.0](https://datatracker.ietf.org/doc/html/rfc6749) bem como na documentação do [JWT para Access Tokens OAuth 2.0](https://datatracker.ietf.org/doc/html/rfc9068).

<p align="center">
  <a href="">
    <img src=".github\images\auth-flow-01.png" alt="api-architecture">
  </a>
</p>

<p align="center">
  <a href="">
    <img src=".github\images\auth-flow-02.png" alt="api-architecture">
  </a>
</p>

<p align="center">
  <a href="">
    <img src=".github\images\auth-flow-0201.png" alt="api-architecture">
  </a>
</p>

<p align="center">
  <a href="">
    <img src=".github\images\auth-flow-03.png" alt="api-architecture">
  </a>
</p>

<p align="center">
  <a href="">
    <img src=".github\images\auth-flow-04.png" alt="api-architecture">
  </a>
</p>



## Rotação / Gerenciamento das Chaves
Para a rotação da chave privada optamos por uma solução simples para o tech challenge, um <b>background service</b>. O ideal seria uma solução mais robusta, consistindo em uma aplicação que gerencia a rotação da chave para todas as instâncias de aplicações que a utilizam. 

O serviço rotaciona a chave privada a cada X dias (parametrizado por variável). Utiliza-se o algoritmo de criptografia assimétrica [RSA](https://pt.wikipedia.org/wiki/RSA_(sistema_criptogr%C3%A1fico)) ou [ECC (Elliptic Curve Cryptography)](https://pt.wikipedia.org/wiki/Criptografia_de_curva_el%C3%ADptica) para a criação de uma nova chave. 

Os parâmetros privados da chave são persistidos no Azure Key Vault, enquanto os parâmetros públicos são encapsulados em um JWK (Json Web Key) e expostos em uma URL com uma lista de JWKS (Json Web Key Set). Por exemplo <b>url-api/jwks</b>. 

São esses parâmetros públicos disponíveis nessa URL que as API's de recursos protegidos irão validar o JWT recebido.

## Prevenção contra possíveis ataques

Algumas camadas adicionais de segurança foram implementadas para evitar alguns dos ataques mais comuns.

| Nome | Prevenção Implementada|
| :---------: | :---------: |
| <p style="width:260px; text-align: left;">SQL Injection</p> | <p style="text-align: left;">Qualquer acesso aos dados é feito através de procedures parametrizadas e do ORM.</p> |
| <p style="width:260px; text-align: left;">Brute Force</p> | <p style="text-align: left;">Lockout após X tentativas erradas de autenticação, Hash de senhas utilizando algoritmo Bcrypt e formato rígido de senha (mínimo: 8 caracteres, 1 dígito, 1 minúscula, 1 maiúscula e 1 caracter especial)</p> |
| <p style="width:260px; text-align: left;">Cross Site Scripting (XSS)</p> | <p style="text-align: left;">Validações server-side do que recebemos do browser, cookies de autenticação como HTTP Only e criptografado para evitar acessá-los por script.</p> |
| <p style="width:260px; text-align: left;">Cross Site Request Forgery (CSRF)</p> | <p style="text-align: left;">Validação de Anti Forgery Token e CORS (habilitado por padrão pelo ASP .NET Core).</p> |
| <p style="width:260px; text-align: left;">Man in the Middle</p> | <p style="text-align: left;">Habilitado HSTS para informar ao cliente que somente requisições HTTPS são aceitas e redirecionamento de protocolos HTTP para HTTPS.</p> |

# Testes
Para este tech challenge o projeto inclui testes em diferentes níveis para garantir a qualidade e o funcionamento correto do software.

<p align="center">
  <a href="">
    <img src=".github\images\tests-diagram.png" alt="api-architecture">
  </a>
</p>

## Testes Unitários
Os testes unitários visam validar a funcionalidade de unidades individuais de código, como métodos ou funções.

- <b>Frameworks Utilizados:</b> xUnit, FakeItEasy (para mocks) e Bogus (para geração automática de dados fake)
- <b>Localização dos Testes:</b> tests/unit/

## Testes de Integração
Os testes de integração validam a interação entre diferentes partes do sistema para garantir que elas funcionem corretamente juntas. Para este teste está configurado subir o banco de dados em um container ao executar o teste.

- <b>Frameworks Utilizado:</b> xUnit e Bogus
- <b>Localização dos Testes:</b> tests/integration/

## Testes de UI/UAT (Interface/Aceitação do Usuário)
Os testes de UI/UAT (User Acceptance Testing) são realizados para validar o aplicativo quanto à usabilidade, experiência do usuário e para garantir que atende aos requisitos do usuário final. Para este teste todo um ambiente é criado e depois descartado após execução do teste.

- <b>Frameworks Utilizado:</b> xUnit, Bogus, Specflow e Selenium
- <b>Localização dos Testes:</b> tests/user-interface/

# CI / CD

O CI / CD desse Tech Challenge consiste nos pipelines: <b>Create Azure Resources</b>, <b>Main</b> e <b>UI Test</b>.

O pipeline de <b>Create Azure Resources</b> cria todos os recursos descritos na [Arquitetura](#arquitetura), fazendo uso dos ARM Templates disponíveis na pasta "azure". Os recursos criados são: Key Vault, Container Registry, SQL Databases e Blob Storage.

O pipeline <b>Main</b> assume o papel de Integração Contínua (CI) e Entrega Contínua (CD), realizando os seguintes passos:
- A compilação das aplicações juntamente com suas dependências; 
- A execução dos testes Unitários, de Integração e UI/UAT;
- A compilação das imagens com base os dockerfiles, gerando os artefatos que são publicados no Container Registry;
- Criação das instâncias dos containers no Azure Container Instance com base as imagens;

Já o pipeline <b>UI Test</b> serve para preparar o ambiente de teste e executar os testes de interface e aceitação do usuário (UI/UAT). Os passos são:
- Criação do Resource Group na Azure de teste para facilitar o gerenciamento do ambiente;
- Deploy das bases de testes;
- Compilação das imagens com base os dockerfiles, gerando os artefatos que são publicados no Container Registry com a tag de teste;
- Criação das instâncias dos containers no Azure Container Instance com base as imagens de teste;
- Execução do teste de UI/UAT;
- Descarte do ambiente de teste deletando o Resource Group;

Abaixo um diagrama que demonstra como o pipeline Main e UI Test se integram.

<p align="center">
  <a href="">
    <img src="docs\pipelines-diagram.jpg" alt="pipeline-diagram">
  </a>
</p>

As migrations do banco são realizadas por cada aplicação (Auth API e Core API), no momento em que a aplicação é executada no container. Isso garante que as bases estão atualizadas automaticamente através das migrations do Entity Framework. Também existe a opção de executar os scripts gerados manualmente. Eles se encontram na pasta "sql".


# Executando a aplicação
É possível executar a aplicação realizando a configuração manualmente, ou utilizando Docker (recomendado).

## Docker
Para rodar localmente, é possível utilizar o Docker.  
Abaixo o passo a passo para executar a aplicação localmente:
- Realizar o clone do projeto na pasta desejada:
    ```bash
        git clone https://github.com/pistoladas-group/tech-challenge-02.git
    ```
- Configurar certificados para habilitar conexão via https:
    ```bash
        dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\technews.pfx"  -p "OVmTv9lykb0)>m=wWcQaJ"
        dotnet dev-certs https --trust
    ```
- Utilizar o comando abaixo para subir a aplicação utilizando docker-compose:
    ```bash
        docker-compose -f docker-compose.debug.yml up --build
    ```