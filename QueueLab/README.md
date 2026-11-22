# QueueLab

RabbitMQ lab: the consumer calculates tax on shopping-cart messages.

Reuses `TaxCalculationService` from `UnitTests/SalesSystem`.

## Run

```bash
# Terminal 1 — consumer
cd QueueLab
dotnet run --project TaxQueue -- consume

# Terminal 2 — sample publish (optional; the Svelte UI can publish too)
dotnet run --project TaxQueue -- publish
```

Defaults: `localhost:5672`, guest/guest, queue `tax.calculate`.
