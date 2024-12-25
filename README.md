# 🏦 **Reactive Finance API** 🚀

Welcome to the **Reactive Finance** API! This is a powerful API for managing and querying currency resources with cutting-edge features built using **.NET Core 8**.

The API allows you to perform CRUD operations on currency resources, including retrieving currency details, exchanging rates, and more! 🌍💸

## Features 🌟

- **WriteCurrencyResource**: Create a new currency resource.
- **QueryCurrencyResource**: Retrieve currency resources by various filters.
- **Exchange Rates**: Get the latest exchange rates for different currencies.
- **Swagger Spec**: Full OpenAPI 3.0 specification to help you integrate smoothly.

## 📜 API Documentation

The API exposes several endpoints that you can use to interact with the currency resources:

### 📝 **Swagger Spec (OpenAPI 3.0)**

You can find the complete Swagger spec for the API in the `spec.yaml` file. Here's a preview:

```yaml
openapi: 3.0.3
info:
  title: reactive-finance
  version: 1.0.0
  contact: {}
servers:
- url: https://localhost:7248
- url: https://proxy.rest.localhost.com/WSF
paths:
  /v1/WriteCurrencyResource:
    post:
      tags:
      - V1
      summary: /v1/WriteCurrencyResource
      description: /v1/WriteCurrencyResource
      operationId: v1Writecurrencyresource
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                currency_code:
                  type: string
                  example: CHF
                currency_flag:
                  type: string
                  example: 🇨🇭
                currency_id:
                  type: number
                  example: 756
                currency_name:
                  type: string
                  example: Swiss Franc
                currency_symbol:
                  type: string
                  example: CHF
            examples:
              /v1/WriteCurrencyResource:
                value:
                  currency_code: CHF
                  currency_flag: 🇨🇭
                  currency_id: 756
                  currency_name: Swiss Franc
                  currency_symbol: CHF
      responses:
        '200':
          description: ''
  /v1/QueryCurrencyResource:
    get:
      tags:
      - V1
      summary: /v1/QueryCurrencyResource
      description: /v1/QueryCurrencyResource
      operationId: v1Querycurrencyresource
      responses:
        '200':
          description: ''
  /v1/QueryCurrencyResource/exchanges:
    get:
      tags:
      - V1
      summary: /v1/QueryCurrencyResource/exchanges
      description: /v1/QueryCurrencyResource/exchanges
      operationId: v1QuerycurrencyresourceExchanges
      responses:
        '200':
          description: ''
  /v1/QueryCurrencyResource/{currencyCode}:
    get:
      tags:
      - V1
      summary: /v1/QueryCurrencyResource/:currencyCode
      description: /v1/QueryCurrencyResource/:currencyCode
      operationId: v1QuerycurrencyresourceCurrencycode
      responses:
        '200':
          description: ''
    parameters:
    - name: currencyCode
      in: path
      required: true
      schema:
        type: string
        example: ZAR
      description: South African Rands
tags:
- name: V1
```

## 🚀 Getting Started

To get started with **Reactive Finance API**, follow the steps below to set up the project locally or in a containerized environment.

### 🖥️ Running the API on Localhost

1. **Clone the Repository:**

   Clone the repository to your local machine:

   ```bash
   git clone git@github.com:MarkMngoma/reactive-finance.git
   cd reactive-finance
   ```

2. **Restore Dependencies:**

   Make sure to restore all necessary NuGet packages:

   ```bash
   dotnet restore
   ```

3. **Run the API:**

   Start the API locally:

   ```bash
   dotnet run --verbose --project ./Server/Server.csproj
   ```

   The API will now be available at [https://localhost:7248](https://localhost:7248).

---

### 🐳 Spinning Up the testing Container

The project uses **Docker** for containerization. To run the application in a container, you need to spin up the Docker container using the provided `docker-compose.yml` file.

1. **Spin Up the localhost Containers:**

   Run the following command to start the containers:

   ```bash
   docker compose -f ./Server.IntegrationTests/Src/Main/Infrastructure/TestContainers/docker-compose.yml up
   ```

   Run the following command to start the containers in detached mode:

   ```bash
   docker compose -f ./Server.IntegrationTests/Src/Main/Infrastructure/TestContainers/docker-compose.yml up -d
   ```

2. **Shut Down the Containers:**

   Run the following command to stop the containers:

   ```bash
   docker compose -f ./Server.IntegrationTests/Src/Main/Infrastructure/TestContainers/docker-compose.yml down
   ```

   This will start all the necessary services defined in the `docker-compose.yml` file, including the Reactive Finance API.

---

### 🐳 Spinning Up the Cloud Container

1. **Spinning up Cloud Ready Container:**

    Execute this command to create an image that'll be provisioned on k8s

    ```bash
    docker build --platform linux/amd64 -t ${containerRegistry}/${containerRepository}/reactive-finance:${version} -f ./Server/Src/Main/Infrastructure/Cloud/Dockerfile .
    ```

2. **Pushing and versioning via container registry:**

   Push the image to a secure container registry
    ```bash
    docker push ${containerRegistry}/${containerRepository}/reactive-finance:${version}
    ```

3. **Spinning up Cloud Ready Container:**

   Execute this command to create an image that'll be provisioned on k8s
   ```bash
    kubectl apply -f ./Server/Src/Main/Infrastructure/Cloud/deployment.yml
   ```
---

### 🔬 Running Integration Tests

The project includes integration tests to ensure that everything is functioning as expected.

1. **Run Tests Using .NET CLI:**

   To run the integration tests, use the following command:

   ```bash
   dotnet test --logger "console;verbosity=detailed"
   ```

   This will execute all the integration tests and display the results in the terminal.

2. **Test Environment Setup:**

   Make sure the API is running before executing the tests, especially when working with containers. You can also run the tests in isolation or as part of your CI/CD pipeline.

---

## 🧑‍💻 API Endpoints

### 1. **POST /v1/WriteCurrencyResource**

- **Description**: Create a new currency resource.
- **Request Body**:
  ```json
  {
    "currency_code": "CHF",
    "currency_flag": "🇨🇭",
    "currency_id": 756,
    "currency_name": "Swiss Franc",
    "currency_symbol": "CHF"
  }
  ```

### 2. **GET /v1/QueryCurrencyResource**

- **Description**: Retrieve all currency resources.
- **Response**: List of all currencies.

### 3. **GET /v1/QueryCurrencyResource/exchanges**

- **Description**: Retrieve the latest exchange rates for all currencies.

### 4. **GET /v1/QueryCurrencyResource/{currencyCode}**

- **Description**: Retrieve a specific currency resource by currency code.
- **Path Parameter**: `currencyCode` (e.g., `ZAR` for South African Rand).

---

Happy coding! 🎉🚀
