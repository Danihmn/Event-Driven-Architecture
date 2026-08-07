# EventDrivenOrder

> **Projeto de estudo pessoal.** Não é um sistema em produção nem comercial. O objetivo é aprender e fixar, na prática, conceitos de arquitetura orientada a eventos (Event-Driven Architecture), mensageria com Apache Kafka, bounded contexts, agregados e comunicação assíncrona entre serviços.

## Objetivo

Este repositório é um laboratório para experimentar:

- Separação de um domínio em **bounded contexts** independentes (Order e Inventory), cada um com seu próprio banco de dados.
- Comunicação assíncrona entre esses contextos via **eventos publicados no Kafka**, em vez de chamadas HTTP diretas.
- Organização interna de cada serviço em camadas (Domain, Application, Infrastructure, Api), com CQRS simples via MediatR.
- Orquestração do ambiente local (APIs, Postgres, Kafka) com **.NET Aspire**.

## Arquitetura geral

O código-fonte fica em `src/` e é dividido em três bounded contexts / módulos:

```
src/
├── Order/                 # Bounded context de Pedidos
│   ├── Order.Api           # Minimal API + consumers de Kafka
│   ├── Order.Application   # Commands/Handlers (MediatR)
│   ├── Order.Domain        # Entidade Order, enum de status
│   └── Order.Infrastructure # EF Core + Postgres (orderdb)
├── Inventory/              # Bounded context de Estoque
│   ├── Inventory.Api           # Minimal API host + consumer de Kafka
│   ├── Inventory.Application   # Commands/Handlers (MediatR)
│   ├── Inventory.Domain        # Entidade Product
│   └── Inventory.Infrastructure # EF Core + Postgres (inventorydb)
├── Shared/
│   ├── Shared.Contracts        # Contratos comuns: Event (base), Topics, IEventPublisher, IRepository
│   └── Shared.Implementations  # KafkaEventPublisher e os eventos concretos (records)
└── EventDrivenOrder.AppHost    # Orquestração via .NET Aspire (Kafka + Postgres + as duas APIs)
```

Cada bounded context tem seu próprio banco (`orderdb` para Order, `inventorydb` para Inventory) e não acessa o banco do outro diretamente — a única forma de comunicação entre eles é a publicação/consumo de eventos no Kafka.

## Tecnologias e pacotes utilizados

Extraído diretamente dos arquivos `.csproj` do repositório:

- **.NET 10** (`net10.0`) em todos os projetos.
- **.NET Aspire** — `Aspire.Hosting.Kafka`, `Aspire.Hosting.PostgreSQL` (no AppHost) e `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` (nas APIs), versão `13.4.6`.
- **Confluent.Kafka** (`2.15.0`) — produtor/consumidor Kafka, usado em `Order.Api`, `Inventory.Api` e `Shared.Implementations`.
- **MediatR** (`14.2.0`) — commands/handlers em `Order.Application` e `Inventory.Application`.
- **FluentResults** (`4.0.0`) — retorno de resultados (`Result`/`Result<T>`) nas camadas de Domain.
- **Entity Framework Core** (`10.0.10`) + **Npgsql.EntityFrameworkCore.PostgreSQL** (`10.0.3`) — persistência em Postgres nas camadas de Infrastructure.
- **Microsoft.AspNetCore.OpenApi** (`10.0.10`) — exposição de OpenAPI nas APIs (habilitado em ambiente de Development).
- **Microsoft.Extensions.Configuration.UserSecrets** — usado nas camadas de Infrastructure.

## Como o fluxo de eventos funciona hoje

Fluxo ponta a ponta, com base no que está implementado nos consumers e handlers:

1. **Criação do pedido** — `POST orders/order` (`OrderEndpoints.cs`) envia `CreateOrderCommand` via MediatR. O `CreateOrderCommandHandler` cria a entidade `Order` (status `Pending`), persiste no `orderdb` e publica `OrderCreatedEvent` no tópico `order.created`.

2. **Reserva de estoque** — `Inventory.Api` tem um `OrderCreatedConsumer` (`BackgroundService`) inscrito no tópico `order.created`. Ao consumir a mensagem, dispara `ReserveStockCommand`. O `ReserveStockCommandHandler` busca o `Product` no `inventorydb` e tenta reservar a quantidade:
   - Se a reserva **falhar** (produto não encontrado ou estoque insuficiente), publica `InventoryReserveFailEvent` no tópico `stock.reservation-failed`.
   - Se a reserva **for bem-sucedida**, atualiza o produto e publica `InventoryReservedEvent` no tópico `stock.reserved`.

3. **Confirmação do pedido** — `Order.Api` tem um `InventoryReservedConsumer` inscrito no tópico `stock.reserved`. Ao consumir a mensagem, dispara `ConfirmOrderCommand`, que muda o status do pedido para `Confirmed`.

4. **Cancelamento do pedido** — `Order.Api` também tem um `InventoryReserveFailedConsumer` inscrito no tópico `stock.reservation-failed`. Ao consumir a mensagem, dispara `CancelOrderCommand`, que muda o status do pedido para `Cancelled`.

Os tópicos usados (`Shared.Contracts.Topics`) são:

| Tópico | Publicado por | Consumido por |
|---|---|---|
| `order.created` | `CreateOrderCommandHandler` (Order) | `OrderCreatedConsumer` (Inventory) |
| `stock.reserved` | `ReserveStockCommandHandler` (Inventory) | `InventoryReservedConsumer` (Order) |
| `stock.reservation-failed` | `ReserveStockCommandHandler` (Inventory) | `InventoryReserveFailedConsumer` (Order) |

A publicação é feita por `KafkaEventPublisher`, que implementa `IEventPublisher` (definido em `Shared.Contracts`) usando `IProducer<string, string>` do Confluent.Kafka, serializando o evento em JSON. Os consumers são `BackgroundService`s que assinam o tópico, desserializam a mensagem e enviam o comando correspondente via `IMediator`.

## Como rodar o projeto localmente

A orquestração local é feita pelo `EventDrivenOrder.AppHost`, usando .NET Aspire. O `AppHost.cs` sobe:

- Um container **Kafka** (com Kafka UI habilitada).
- Um Postgres para o Order (`order-postgres`, porta `5432`, com PgWeb e volume de dados), com o banco `orderdb`.
- Um Postgres para o Inventory (`inventory-postgres`, porta `5433`, com PgWeb e volume de dados), com o banco `inventorydb`.
- O projeto `Order.Api`, referenciando `orderdb` e o Kafka.
- O projeto `Inventory.Api`, referenciando `inventorydb` e o Kafka.

Para rodar (requer .NET 10 SDK e Docker, já que Aspire sobe Kafka/Postgres em containers):

```bash
cd src/EventDrivenOrder.AppHost
dotnet run
```

Isso inicia o dashboard do Aspire, a partir do qual é possível acompanhar os dois serviços, o Kafka e os bancos Postgres.
