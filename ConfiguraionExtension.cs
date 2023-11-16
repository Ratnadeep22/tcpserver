using Microsoft.Extensions.DependencyInjection;

namespace tcpserver
{
    public static class ConfiguraionExtension
    {
        public static void AddVMServices(this IServiceCollection services)
        {
            services.AddTransient<MainWindowVM, MainWindowVM>();
        }

        public static void AddViewServices(this IServiceCollection services)
        {
            services.AddTransient<MainWindow, MainWindow>();
        }

    }
}
