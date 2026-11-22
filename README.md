# CSharpLab

Multi-lab C# workspace (plus related experiments).

## Layout

```
CSharpLab/
  UnitTests/              SalesSystem API + xUnit unit tests
  ArchitectureTests/      Future architecture tests
  LoadTests/              Future load / performance tests
  QueueLab/               RabbitMQ tax-queue experiment
  experiments/            Ad-hoc experiments
  projects/               Related apps (e.g. fila-imposto)
```

## SalesSystem unit-testing lab

```bash
dotnet build UnitTests/UnitTests.sln
dotnet test UnitTests/SalesSystem.Tests/SalesSystem.Tests.csproj
```

## Queue lab

```bash
dotnet build QueueLab/QueueLab.sln
```
