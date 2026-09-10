```markdown
# HANDOUT — AULA 02

## Dissecando o HTTP

*6 requisições sob o microscópio — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês interceptaram 6 conversas entre um app e a API de uma biblioteca. Para CADA card:

- Descrevam o que o cliente pediu (verbo + recurso na URI)
- Expliquem o que o status code da resposta informa
- Respondam: repetindo a MESMA requisição 3 vezes seguidas, o estado do servidor muda?

Ao final, preencham juntos a TABELA-SÍNTESE dos verbos na última página.

*⏱️ Tempo: 30 minutos  |  👥 Formato: em duplas  |  Dica: o card 6 esconde uma pegadinha de quem é a culpa.*

> **Nomes:** ____________________   **Turma:** ____________________   **Data:** ___ / ___ / ______

## REQUISIÇÃO 01 — A prateleira inteira

```text
→ REQUISIÇÃO
GET /api/livros HTTP/1.1
Host: biblioteca.newton.br
Accept: application/json

```

```text
← RESPOSTA
HTTP/1.1 200 OK
Content-Type: application/json

[ { "id": 1, "titulo": "Clean Code", "autor": "Robert C. Martin" },
  { "id": 7, "titulo": "O Programador Pragmático", "autor": "Hunt & Thomas" } ]

```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
O cliente utilizou o verbo GET no recurso /api/livros.
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
O status 200 OK informa que a requisição deu certo e os dados foram retornados.
3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
O estado do servidor não muda, pois o verbo GET apenas lê os dados (é seguro). A resposta será a mesma lista nas três vezes.

## REQUISIÇÃO 02 — O livro fantasma

```text
→ REQUISIÇÃO
GET /api/livros/99 HTTP/1.1
Host: biblioteca.newton.br
Accept: application/json

```

```text
← RESPOSTA
HTTP/1.1 404 Not Found
Content-Type: application/problem+json

{ "title": "Not Found", "status": 404 }

```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
O cliente utilizou o verbo GET no recurso /api/livros/99.
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
O status 404 Not Found informa que o recurso não foi encontrado. A culpa é do cliente, que solicitou um ID que não existe.
3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
O estado do servidor não muda. A resposta continuará sendo 404 Not Found em todas as tentativas.

## REQUISIÇÃO 03 — Livro novo na estante

```text
→ REQUISIÇÃO
POST /api/livros HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "titulo": "Domain-Driven Design", "autor": "Eric Evans" }

```

```text
← RESPOSTA
HTTP/1.1 201 Created
Location: /api/livros/8
Content-Type: application/json

{ "id": 8, "titulo": "Domain-Driven Design", "autor": "Eric Evans" }

```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
O cliente utilizou o verbo POST no recurso /api/livros.
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
O status 201 Created informa que a requisição deu certo e um novo recurso foi criado com sucesso.
3. Enviando este POST 3 vezes seguidas, o que acontece na estante? Para que serve o header Location?
O servidor criará 3 livros novos (repetidos), cada um com um ID diferente. O header Location serve para informar a URL exata de onde o recurso recém-criado pode ser acessado.

## REQUISIÇÃO 04 — Corrigindo a ficha completa

```text
→ REQUISIÇÃO
PUT /api/livros/7 HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "id": 7, "titulo": "O Programador Pragmático", "autor": "D. Hunt; D. Thomas" }

```

```text
← RESPOSTA
HTTP/1.1 200 OK
Content-Type: application/json

{ "id": 7, "titulo": "O Programador Pragmático", "autor": "D. Hunt; D. Thomas" }

```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
O cliente utilizou o verbo PUT no recurso /api/livros/7.
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
O status 200 OK informa que a atualização do recurso deu certo.
3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
O estado do servidor muda apenas na primeira requisição. Repetir deixará o recurso exatamente no mesmo estado (idempotência). A resposta será sempre 200 OK.

## REQUISIÇÃO 05 — Fora do catálogo

```text
→ REQUISIÇÃO
DELETE /api/livros/7 HTTP/1.1
Host: biblioteca.newton.br

```

```text
← RESPOSTA
HTTP/1.1 204 No Content

```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
O cliente utilizou o verbo DELETE no recurso /api/livros/7.
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
O status 204 No Content informa que a requisição deu certo e o recurso foi removido com sucesso (sem retornar um corpo de resposta).
3. Repetindo o DELETE, o estado do servidor muda? Que resposta você ESPERA na segunda vez?
O estado do servidor não muda a partir da segunda tentativa, pois o recurso já foi apagado. A resposta esperada na segunda vez é um 404 Not Found.

## REQUISIÇÃO 06 — O cadastro capenga

```text
→ REQUISIÇÃO
POST /api/livros HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "autor": "Anônimo" }

```

```text
← RESPOSTA
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json

{ "title": "Bad Request", "status": 400,
  "errors": { "Titulo": [ "O campo Titulo é obrigatório" ] } }

```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
O cliente utilizou o verbo POST no recurso /api/livros.
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
O status 400 Bad Request informa que a requisição estava malformada. A culpa é do cliente, que não enviou o campo obrigatório "Titulo".
3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
O estado do servidor não muda, pois requisições inválidas são rejeitadas. A resposta continuará sendo o erro 400 Bad Request.

## TABELA-SÍNTESE — Os verbos do HTTP

*Preencham com base nos 6 cards. “Seguro” = não altera nada no servidor. “Idempotente” = repetir N vezes deixa o servidor no mesmo estado que 1 vez.*

| **Verbo** | **Para que serve** | **Seguro?** | **Idempotente?** | **Status típicos** |
| --- | --- | --- | --- | --- |
| **`GET`** | Ler ou recuperar dados | Sim | Sim | 200, 404 |
| **`POST`** | Criar um novo recurso | Não | Não | 201, 400 |
| **`PUT`** | Atualizar/substituir o recurso completo | Não | Sim | 200, 404 |
| **`PATCH`** | Atualizar o recurso parcialmente | Não | Não (tecnicamente) | 200, 404 |
| **`DELETE`** | Remover um recurso | Não | Sim | 204, 404 |

## DESAFIO

1. O verbo PATCH não apareceu em nenhum card. Qual a diferença entre PATCH e PUT? Um app de banco quer alterar SÓ o apelido do usuário, entre dezenas de campos do perfil — qual dos dois você usaria e por quê?

**Resposta:**
A diferença principal é o escopo: o PUT substitui o recurso inteiro (exige o envio de todos os campos), enquanto o PATCH aplica modificações parciais (envia apenas o que vai mudar). Para alterar SÓ o apelido, deve-se usar o PATCH, pois é mais eficiente do que trafegar todos os dados do perfil pela rede para modificar um único campo.

```

```