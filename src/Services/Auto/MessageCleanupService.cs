using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System;
using PetProject.Repositories;
using Microsoft.Extensions.Configuration;

namespace PetProject.Services
{
    public class MessageCleanupService : IHostedService, IDisposable
    {
        private readonly IPrivateMessageRepository _messageService;
        private readonly IConfiguration _configuration;
        private Timer _timer;

        public MessageCleanupService(IPrivateMessageRepository messageService, IConfiguration configuration)
        {
            _messageService = messageService;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            double.TryParse(_configuration["AppSettings:LoopCronMinutes"], out var loopTime); 
            _timer = new Timer(async state => await DoWork(), null, TimeSpan.Zero, TimeSpan.FromMinutes(loopTime));
            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            await _messageService.DeleteMessageWithTime();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
