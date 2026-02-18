using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7112/notifications") 
    .Build();

connection.On<string, object>("EntityCreated", (entity, data) =>
{
    Console.WriteLine($"CREATO: {entity} -> {data}");
});

connection.On<string, object>("EntityUpdated", (entity, data) =>
{
    Console.WriteLine($"AGGIORNATO: {entity} -> {data}");
});

connection.On<string, object>("EntityDeleted", (entity, data) =>
{
    Console.WriteLine($"ELIMINATO: {entity} -> {data}");
});

await connection.StartAsync();
Console.WriteLine("Connesso a notifications. Premi INVIO per chiudere.");
Console.ReadLine();
