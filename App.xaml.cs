using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;

namespace tcpserver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IConfiguration _configuration;
        public IHost _host;
        public App()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional:false, reloadOnChange:true)
                .Build();

            _host = new HostBuilder()
                .ConfigureServices(services => {
                    services.AddSingleton(_configuration);
                    services.AddViewServices();
                    services.AddVMServices();
                })
                .Build();

            _host.StartAsync().Wait();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainwindow = _host.Services.GetRequiredService<MainWindow>();
            mainwindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _host.StopAsync().Wait();
        }
    }
}
