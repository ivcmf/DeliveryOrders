using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        var cs = Environment.GetEnvironmentVariable("Cosmos__ConnectionString")
                 ?? throw new InvalidOperationException("Cosmos__ConnectionString missing");
        var db = Environment.GetEnvironmentVariable("Cosmos__Database") ?? "delivery";
        var ctr = Environment.GetEnvironmentVariable("Cosmos__Container") ?? "orders";

        s.AddSingleton(new CosmosClient(cs));
        s.AddSingleton(sp =>
            sp.GetRequiredService<CosmosClient>().GetDatabase(db).GetContainer(ctr));
    })
    .Build();

host.Run();
