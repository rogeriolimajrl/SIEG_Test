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