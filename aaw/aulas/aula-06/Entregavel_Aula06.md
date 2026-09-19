# Entregável — Prática Aula 06 (Comunicação entre Serviços)
**Aluno:** Cauã Rheris Papa Ribeiro


## 2. Tabela comparativa preenchida (Fase 5, pergunta 5)

| Fluxo | Tempo de resposta | Notificação chega? |
|---|---|---|
| **Síncrono** | ~4000 a 5250 ms | [ ] Imediata / [X] Pode falhar (e causar erro 500 para o usuário) |
| **Assíncrono** | ~1000 a 1120 ms | [X] Em background / [ ] Garantida? (Depende da fila não reiniciar) |

---

## 3. Respostas das perguntas 1-4

**1. Por que o pagamento é síncrono nos dois fluxos? Poderia ser assíncrono?**
O pagamento deve se manter síncrono porque a regra central de negócios exige essa validação imediata. Ele não poderia ser assíncrono pois o sistema é proibido de aprovar um pedido "às cegas" para verificar o saldo do cliente depois.

**2. O que acontece se a fila perder um evento?**
Como a fila atual do projeto (`Channel<T>`) opera inteiramente na memória RAM da aplicação, qualquer reinicialização, queda de energia ou travamento do servidor fará com que todos os eventos pendentes sejam perdidos para sempre.

**3. Em produção, o que substituiria o `Channel<T>`?**
Para garantir que as mensagens não sejam perdidas (persistência), o `Channel<T>` seria substituído por um *Message Broker* dedicado. Exemplos de tecnologias incluem RabbitMQ, Apache Kafka, AWS SQS ou Azure Service Bus.

**4. Se a NotificacaoApi falhar, como o consumidor da fila deveria reagir?**
O consumidor deveria implementar uma política de retentativas automáticas (*Retry*), preferencialmente com recuo exponencial (*Exponential Backoff*). Caso a mensagem atinja o limite máximo de tentativas de envio sem sucesso, ela deve ser movida para uma *Dead-Letter Queue* (DLQ), que é uma fila separada para mensagens com erro, permitindo uma análise manual posterior sem travar o processamento dos novos pedidos.