namespace FamTec.Server.Services
{
    public class StartupTask : IHostedService
    {
        private readonly WorksSetting _settings;

        public StartupTask(WorksSetting settings)
        {
            _settings = settings;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _settings.DefaultSetting();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
