# Atividade — AULA 06

## Síncrono ou Assíncrono?

*Análise de fluxos de comunicação entre serviços — Arquitetura de Aplicações Web*[cite: 2]

## 🎯 MISSÃO

Vocês são os arquitetos dos 4 fluxos abaixo. Para CADA cenário:

- Decidam o estilo de comunicação: síncrono (request/response), assíncrono (fila/evento) ou API Gateway/BFF[cite: 2]
- Desenhem o fluxo com caixas (serviços) e setas (chamadas/mensagens) no espaço indicado[cite: 2]
- Justifiquem com pelo menos 2 fatores (urgência da resposta, tolerância a atraso, picos, falhas...)[cite: 2]
- Apontem o principal risco da escolha de vocês[cite: 2]

*⏱️ Tempo: 25 minutos  |  👥 Formato: em duplas  |  Não existe resposta única — o que vale é a justificativa.*[cite: 2]

> **Nome:** Cauã Rheris Papa Ribeiro   **Turma:** Análise e Desenvolvimento de Sistemas   **Data:** 14 / 09 / 2026

---

## CENÁRIO 01 — PagFácil — aprovar ou negar AGORA

No checkout do PagFácil, ao clicar em “Pagar”, o serviço de Pagamentos precisa consultar o saldo/limite do cliente no serviço de Contas — e a resposta define se a venda acontece neste exato momento[cite: 2].

- O cliente está na tela, esperando o resultado da compra[cite: 2]
- Sem a resposta de Contas, não há decisão possível: aprovar às cegas é proibido[cite: 2]
- Tempo de resposta do serviço de Contas: ~80 ms em condições normais[cite: 2]

**Sua análise:**

1. Estilo recomendado:   [X] Síncrono      [ ] Assíncrono (fila/evento)      [ ] API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

| Checkout | ➔ (HTTP Síncrono) ➔ | Serviço de Pagamentos | ➔ (HTTP Síncrono) ➔ | Serviço de Contas |
| --- | --- | --- | --- | --- |

3. Justificativa (mínimo 2 fatores):
- **Urgência da resposta:** O cliente está ativamente aguardando na tela o resultado da transação para saber se a compra foi finalizada.
- **Consistência imediata:** A regra de negócio proíbe aprovar sem limite (às cegas); a resposta define estritamente se a venda pode ou não ocorrer no momento exato.

4. Principal risco da escolha:
- **Baixa resiliência (Falha em Cascata):** Se o serviço de Contas cair ou ficar muito lento, o serviço de Pagamentos sofre a indisponibilidade simultaneamente, bloqueando completamente o checkout da loja.

---

## CENÁRIO 02 — CadastraJá — o e-mail de boas-vindas

Após criar a conta no CadastraJá, o sistema envia um e-mail de boas-vindas. O provedor de e-mail às vezes demora 8 segundos para responder e falha em 2% das tentativas[cite: 2].

- O usuário quer começar a usar o app imediatamente após o cadastro[cite: 2]
- O e-mail chegar 1 minuto depois não incomoda ninguém[cite: 2]
- Se o provedor falhar, o envio deve ser tentado de novo — sem o usuário perceber[cite: 2]

**Sua análise:**

1. Estilo recomendado:   [ ] Síncrono      [X] Assíncrono (fila/evento)      [ ] API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

| Serviço de Cadastro | ➔ (Publica Evento) ➔ | Fila (Message Broker) | ➔ (Consome Evento) ➔ | Serviço de E-mail | ➔ Provedor Externo |
| --- | --- | --- | --- | --- | --- |

3. Justificativa (mínimo 2 fatores):
- **Tolerância a atrasos:** O usuário quer usar o app imediatamente. Como o e-mail não é um requisito bloqueante, o envio pode atrasar minutos sem afetar a experiência.
- **Resiliência a falhas:** A fila permite isolar os 2% de falha do provedor. Se o envio falhar, a mensagem volta para a fila e ocorre uma nova tentativa (Retry) transparente para o usuário.

4. Principal risco da escolha:
- **Acúmulo de mensagens:** Em caso de indisponibilidade prolongada (horas fora do ar) do provedor de e-mail, a fila pode acumular um número excessivo de eventos de boas-vindas, gerando risco de sobrecarga de armazenamento no broker ou rajada excessiva quando o serviço retornar.

---

## CENÁRIO 03 — MegaMarket — baixa de estoque nos picos

No marketplace MegaMarket, cada venda gera uma baixa no serviço de Estoque. Nas grandes promoções o tráfego sobe 10x e o Estoque não dá conta de responder na velocidade das vendas[cite: 2].

- Atraso de alguns segundos na baixa é aceitável[cite: 2]
- PERDER uma baixa de estoque não é aceitável (gera venda sem produto)[cite: 2]
- O checkout não pode ficar lento nem cair porque o Estoque está sobrecarregado[cite: 2]

**Sua análise:**

1. Estilo recomendado:   [ ] Síncrono      [X] Assíncrono (fila/evento)      [ ] API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

| Serviço de Checkout | ➔ (Publica Evento) ➔ | Fila (Message Broker) | ➔ (Consome Evento no seu próprio ritmo) ➔ | Serviço de Estoque |
| --- | --- | --- | --- | --- |

3. Justificativa (mínimo 2 fatores):
- **Absorção de picos (Buffering):** A fila age como um amortecedor para as requisições, evitando que a sobrecarga do Estoque afete a velocidade do checkout do MegaMarket durante a promoção.
- **Garantia de entrega:** Uma fila persistente assegura que a baixa de estoque não será perdida caso o Serviço de Estoque caia ou reinicie; ele processará os itens pendentes assim que voltar ao ar.

4. Principal risco da escolha:
- **Consistência Eventual:** Devido ao atraso na atualização do estoque, o sistema corre o risco de processar a venda de um produto que já se esgotou nos segundos anteriores de tráfego intenso, gerando compras de produtos que fisicamente não existem mais.

---

## CENÁRIO 04 — AppBanco — uma tela, cinco serviços

A tela inicial do AppBanco mostra saldo, fatura do cartão, investimentos, empréstimos e cashback — dados de 5 serviços diferentes. O time mobile reclama: são 5 chamadas, 5 formatos de resposta e 5 pontos de falha em cada abertura do app[cite: 2].

- A tela precisa abrir rápido, inclusive em redes móveis ruins[cite: 2]
- Cada serviço tem equipe, formato e autenticação próprios[cite: 2]
- Amanhã nasce a versão web, que precisa de MAIS dados que a mobile[cite: 2]

**Sua análise:**

1. Estilo recomendado:   [ ] Síncrono      [ ] Assíncrono (fila/evento)      [X] API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

| App Mobile | ➔ (1 Req) ➔ | BFF (Mobile Gateway) | ➔ (N Reqs Síncronas Paralelas) ➔ | Microserviços (Saldo, Fatura, Invest., Emp., Cash.) |
| --- | --- | --- | --- | --- |

3. Justificativa (mínimo 2 fatores):
- **Otimização de rede:** Centraliza a comunicação em uma única requisição partindo do cliente, minimizando o impacto da latência (que é alta em redes móveis ruins).
- **Adequação por cliente (Backend For Frontend):** Cada interface (Mobile vs. Web) pode ter um BFF próprio para traduzir, agrupar e formatar o payload perfeitamente sob medida para o que aquela tela específica necessita, sem sobrecarregar o cliente com dados desnecessários.

4. Principal risco da escolha:
- **Ponto Único de Falha (SPOF):** O BFF / API Gateway se torna o gargalo da aplicação. Se ele ficar indisponível, todo o AppBanco ficará fora do ar simultaneamente, já que a comunicação com todos os microsserviços dependerá exclusivamente dele.

---

## DESAFIO

1. Escolha um cenário em que vocês indicaram ASSÍNCRONO. Os brokers de mensagens costumam garantir entrega “pelo menos uma vez” — ou seja, a MESMA mensagem pode chegar duas vezes. O que aconteceria no seu fluxo? Como o consumidor deveria se proteger?[cite: 2]

**Resposta:**
- **O que aconteceria:** No cenário do "MegaMarket", se a mensagem de venda for entregue duas vezes, o serviço de Estoque daria baixa indevida em dois itens quando apenas um foi efetivamente vendido, bagunçando o inventário e causando prejuízos ou cancelamentos falsos.
- **Como se proteger:** O serviço consumidor (Estoque) precisa ser desenvolvido de forma **Idempotente**. Cada evento publicado na fila deve possuir um `ID único`. Antes de processar a baixa, o consumidor deve checar em seu banco de dados se esse `ID` já foi processado com sucesso. Caso já tenha sido, ele simplesmente descarta a mensagem recebida. Caso contrário, ele processa a baixa e registra o `ID` no banco.