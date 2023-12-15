# API de Gestão de Conta Bancária

Bem-vindo à API de gestão de contas bancárias, uma aplicação ASP.NET Core 8 que permite a administração de transações bancárias. Abaixo estão as instruções para configurar e executar a API em seu ambiente local.



## Pré-requisitos

Certifique-se de ter os seguintes componentes instalados em sua máquina:

- [.NET Core 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

---

## Configuração do Banco de Dados

1. Crie um banco de dados no PostgreSQL para a aplicação.

2. Atualize a string de conexão no arquivo `appsettings.json` com as informações do seu banco de dados:

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=SeuBancoDeDados;Username=SeuUsuario;Password=SuaSenha"
    }
    ```

---

## Execução da API

1. Abra um terminal e navegue até o diretório do projeto:

    ```bash
    cd Caminho/Para/Seu/Projeto
    ```

2. Restaure as dependências e inicie a aplicação:

    ```bash
    dotnet restore
    dotnet run
    ```

3. Acesse a API em [https://localhost:5001](https://localhost:5001) no seu navegador.

---

## Documentação da API

A API utiliza o Swagger para documentação. Após iniciar a aplicação, acesse:

- [https://localhost:5001/swagger](https://localhost:5001/swagger)

---

## Tecnologias Utilizadas

- ASP.NET Core 8
- PostgreSQL
- Entity Framework Core

---

## Contato

Para quaisquer dúvidas ou problemas, sinta-se à vontade para entrar em contato.
