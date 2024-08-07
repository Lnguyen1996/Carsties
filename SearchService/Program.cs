using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Comsumers;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionSvcHttpClient>()
                    .AddPolicyHandler(GetPolicy());

builder.Services.AddMassTransit(i =>
{
    i.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    i.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    i.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));

            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();


app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);

        throw;
    }
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy() => HttpPolicyExtensions
                                                            .HandleTransientHttpError()
                                                            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                                                            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));