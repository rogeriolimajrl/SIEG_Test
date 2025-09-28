# SIEG_Test API

API para salvar, consultar, atualizar e excluir documentos XML (ex: NFe, CTe, etc.), construída em .NET 8, com persistência em MongoDB e publicação de eventos em RabbitMQ.

## Endpoints

1. Upload de documento

POST /api/documents/upload

Envia um XML para ser armazenado e processado.

Request (JSON):

{
  "xml": "<NFe><Id>123</Id></NFe>",
  "type": "NFe"
}


Response (201 Created):

{
  "id": "650f3e1a2f5d5c1f44a1c123",
  "type": "NFe",
  "createdAt": "2025-09-27T12:00:00Z"
}

2. Consultar documentos

GET /api/documents?page=1&pageSize=10

Retorna lista paginada de documentos.

Response (200 OK):

{
  "page": 1,
  "pageSize": 10,
  "total": 2,
  "items": [
    {
      "id": "650f3e1a2f5d5c1f44a1c123",
      "type": "NFe",
      "createdAt": "2025-09-27T12:00:00Z"
    },
    {
      "id": "650f3e1a2f5d5c1f44a1c124",
      "type": "CTe",
      "createdAt": "2025-09-27T13:15:00Z"
    }
  ]
}

3. Consultar documento por ID

GET /api/documents/{id}

Exemplo:
GET /api/documents/650f3e1a2f5d5c1f44a1c123

Response (200 OK):

{
  "id": "650f3e1a2f5d5c1f44a1c123",
  "type": "NFe",
  "xml": "<NFe><Id>123</Id></NFe>",
  "createdAt": "2025-09-27T12:00:00Z"
}


Se não encontrado (404):

{
  "error": "Document not found"
}

4. Atualizar documento

PUT /api/documents/{id}

Request (JSON):

{
  "xml": "<NFe><Id>123</Id><Updated>true</Updated></NFe>",
  "type": "NFe"
}


Response (200 OK):

{
  "id": "650f3e1a2f5d5c1f44a1c123",
  "type": "NFe",
  "updatedAt": "2025-09-27T14:00:00Z"
}

5. Deletar documento

DELETE /api/documents/{id}

Response (204 No Content):

(no body)


Se não encontrado (404):

{
  "error": "Document not found"
}

## Como rodar a API

Configure MongoDB e RabbitMQ no appsettings.json.

## Rode o projeto:

dotnet run --project SIEG_Test.Api


## A API ficará disponível em:

http://localhost:5154


## Como rodar os testes

Entre no projeto de testes:

cd SIEG_Test.Testes


Rode todos os testes unitários e de integração:

dotnet test



## Decisões de arquitetura e modelagem

Camadas da aplicação:

Controllers: endpoints HTTP (REST)

DTOs: objetos de entrada/saída da API

Models: entidades persistidas no MongoDB

Services: lógica de negócio, incluindo persistência e publicação em RabbitMQ

Tests: testes unitários e de integração com Mongo2Go (MongoDB in-memory)

MongoDB foi escolhido pela flexibilidade em lidar com XML armazenado em formato raw + hash para deduplicação.

RabbitMQ foi integrado para cenários de mensageria/eventos (ex: disparo de notificações ou workflows assíncronos).

DTO separado do Model: mantém a API desacoplada do modelo de persistência, evitando vazamento de detalhes internos.


## Tratamento de dados sensíveis

ConnectionStrings do MongoDB e credenciais do RabbitMQ não são hardcoded: ficam em appsettings.json e podem ser sobrepostas por variáveis de ambiente.

Nunca armazenamos senhas ou tokens diretamente no código-fonte.

XMLs são armazenados no banco com hash calculado (SHA256), para facilitar deduplicação sem expor o conteúdo completo em logs.



## Possíveis melhorias (se tivesse mais tempo)

Autenticação e Autorização
Implementar autenticação (JWT) para proteger os endpoints.

Validação de XMLs
Usar XSD ou esquemas oficiais da SEFAZ para validar NFes, CTes etc.

Escalabilidade
Colocar API em containers (Docker) e configurar réplicas do MongoDB e RabbitMQ.

Testes mais robustos
Ampliar cobertura de testes de carga e de integração, incluindo cenários de falha de conexão com MongoDB ou RabbitMQ.