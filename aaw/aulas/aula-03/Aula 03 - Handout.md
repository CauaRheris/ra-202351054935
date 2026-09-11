# HANDOUT — AULA 03

## Consultoria de Design: a API da EscolaTech

*Identifique os anti-padrões e proponha o redesenho — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

A EscolaTech contratou a consultoria de vocês para auditar a API do sistema escolar. Todos os endpoints abaixo FUNCIONAM e estão em produção — mas o time novo se recusa a mexer neles. Para CADA endpoint:

- Identifiquem o(s) problema(s) de design (pode haver mais de um!)
- Proponham o redesenho: método HTTP + rota + status codes corretos

*⏱️ Tempo: 25 minutos  |  👥 Formato: em duplas  |  Dica: se a rota conta o que faz em português, algo está errado.*

> **Nomes:** _Cauã Papa e Gabriel Otone_   **Turma:** _Arquitetura App Web_    **Data:** _20_ / _08_ / _2026_

## ENDPOINT 01 — POST /api/getAlunos

**Documentação atual (extraída da wiki da EscolaTech):**

```text
POST /api/getAlunos
Retorna TODOS os alunos cadastrados (hoje: 12.482 registros).
Resposta: 200 OK + array JSON completo (~9 MB).
Obs. da wiki: "usar POST porque GET não estava funcionando".
```

1. Qual(is) problema(s) de design vocês identificam?

    _R: Os problemas são, o verbo "get" presente no nome, o método POST utilizado e o response muito grande._

2. Seu redesenho (método + rota + status codes):

    ```text 
    R: GET /api/Alunos
    Retorna TODOS os alunos cadastrados (hoje: 12.482 registros).
    Resposta: 200 OK + array JSON completo (~9 MB).
    Obs. da wiki: "usar POST porque GET não estava funcionando".
    ```


## ENDPOINT 02 — GET /deletarAluno?id=7

**Documentação atual (extraída da wiki da EscolaTech):**

```text
GET /deletarAluno?id=7
Remove o aluno do banco de dados.
Resposta: 200 OK + "OK" (mesmo se o aluno não existir).
Obs. da wiki: "dá pra deletar pelo navegador, bem prático".
```

1. Qual(is) problema(s) de design vocês identificam?
    
    _R: O verbo no endpoint,o nome no singular, a barra no lugar do "?" na rota ao identificar e o retorno consta que o get deletou._

2. Seu redesenho (método + rota + status codes):

    ```text
    DELETE /API/Alunos/7
    Remove o aluno do banco de dados.
    Resposta: 200 OK + "OK" (somente se o aluno existir).
    Obs. da wiki: "dá pra deletar pelo navegador, bem prático".
    ```

## ENDPOINT 03 — POST /api/alunos (criação)

**Documentação atual (extraída da wiki da EscolaTech):**

```text
POST /api/alunos
Body: { "nome": "...", "curso": "..." }
Cria o aluno e responde: 200 OK + body "OK".
O app precisa buscar a lista inteira de novo para descobrir o ID gerado.
```

1. Qual(is) problema(s) de design vocês identificam?

    _R: O retorno deveria ser 400 e a dica está errada, não é necessário buscar a lista pois vamos criar e não atualizar._
2. Seu redesenho (método + rota + status codes):

    ```text
    POST /api/alunos
    Body: { "nome": "Jorge", "curso": "Sistemas de Informação" }
    Cria o aluno e responde: 400 Bad Request.
    Aluno não criado por falta de informações.
    ```

## ENDPOINT 04 — GET /escolas/1/turmas/3/alunos/25/matriculas/88/disciplinas/12

**Documentação atual (extraída da wiki da EscolaTech):**

```text
GET /escolas/1/turmas/3/alunos/25/matriculas/88/disciplinas/12
Retorna os dados da disciplina 12 da matrícula 88.
Para montar a URL o app precisa conhecer 5 IDs diferentes.
Resposta: 200 OK + JSON da disciplina.
```

1. Qual(is) problema(s) de design vocês identificam?

    _R: Anti padrão do alinhamento._

2. Seu redesenho (método + rota + status codes):

    ```text
    GET /escolas/1/turmas/3/alunos/25/matriculas/88/disciplinas/12
    Retorna os dados da disciplina 12 da matrícula 88.
    Para montar a URL o app precisa conhecer 5 IDs diferentes.
    Resposta: 200 OK + JSON da disciplina.
    ```

## ENDPOINT 05 — GET /api/alunos/7/matriculas (erro)

**Documentação atual (extraída da wiki da EscolaTech):**

```text
GET /api/alunos/7/matriculas
Se o aluno 7 não existe, responde:
200 OK + "<html><b>Erro: aluno nao existe!</b></html>"
O app mobile quebra tentando fazer parse do JSON.
```

1. Qual(is) problema(s) de design vocês identificam?

    _R: O status code está incorreto. Deveria ser 404 Not Found._
2. Seu redesenho (método + rota + status codes):

    ```text
    GET /api/alunos/7/matriculas
    Se o aluno 7 não existe, responde:
    404 Not Found + "<html><b>Erro: aluno nao existe!</b></html>"
    O app mobile quebra tentando fazer parse do JSON.
    ```

## ENDPOINT 06 — PUT /api/atualizarNotaParcial?aluno=7&disc=12&nota=8.5

**Documentação atual (extraída da wiki da EscolaTech):**

```text
PUT /api/atualizarNotaParcial?aluno=7&disc=12&nota=8.5
Atualiza SÓ a nota parcial da disciplina, sem body.
Todos os dados vão na query string.
Resposta: 200 OK + "OK".
```

1. Qual(is) problema(s) de design vocês identificam?

    _R: A rota está usando query string para passar parâmetros que deveriam ser parte da URL, o que não é ideal para APIs RESTful._
2. Seu redesenho (método + rota + status codes):

    ```text
    PUT /api/alunos/7/disciplinas/12/notas
    Body: { "nota": 8.5 }
    Atualiza a nota parcial da disciplina.
    Resposta: 200 OK + "OK".
    ```

## DESAFIO

1. A EscolaTech quer lançar mudanças na API sem quebrar o app mobile antigo, que não recebe atualização há 2 anos. Que decisão de design — que falta na API INTEIRA — resolve esse problema? Como ficariam as rotas?

    _R: A decisão de design que resolve esse problema é a versão da API. As rotas ficariam assim:_

    ```text
    GET /v1/api/alunos
    POST /v1/api/alunos
    DELETE /v1/api/alunos/7
    GET /v1/escolas/1/turmas/3/alunos/25/matriculas/88/disciplinas/12
    GET /v1/api/alunos/7/matriculas
    PUT /v1/api/alunos/7/disciplinas/12/notas
    ```
