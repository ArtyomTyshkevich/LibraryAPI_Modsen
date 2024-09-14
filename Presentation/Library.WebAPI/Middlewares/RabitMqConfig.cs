using Library.Data.Consumers;
using Library.WebAPI.Middlewares;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure.Setup
{
    public static class RabbitMqSetup
    {
        public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<BookRentConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMqSettings:Host"], c =>
                    {
                        c.Username(configuration["RabbitMqSettings:Username"]!);
                        c.Password(configuration["RabbitMqSettings:Password"]!);
                    });

                    cfg.ReceiveEndpoint(configuration["RabbitMqSettings:QueueName"]!, e =>
                    {
                        e.ConfigureConsumer<BookRentConsumer>(context);
                    });

                    cfg.ClearSerialization();
                    cfg.UseRawJsonSerializer();
                    cfg.ConfigureEndpoints(context);
                    cfg.UseFilter(new DelayMiddleware());
                });
            });
        }
    }
}