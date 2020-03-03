using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Consumer.Handler
{
    public class MessageHandler : IHostedService
    {
        private readonly ILogger _logger;
        public MessageHandler(ILogger<MessageHandler> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe("fila_pedido");
                var cts = new CancellationTokenSource();

                try
                {
                    while (true)
                    {
                        var message = c.Consume(cts.Token);
                        _logger.LogInformation($"Mensagem: {message.Value} recebida de {message.TopicPartitionOffset}");
                    }
                }
                catch (OperationCanceledException)
                {
                    c.Close();
                }
            }

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}