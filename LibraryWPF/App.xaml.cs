using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SyspevPKSKR1;
using SyspevPKSKR1.Models;
using System;
using System.Windows;

namespace LibraryWPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddDbContext<LibraryDbContext>(options =>
                options.UseNpgsql("Host=localhost;Port=5432;Database=sysoevpks1;Username=postgres;Password=RussianGrime65"),
                ServiceLifetime.Transient);

            services.AddSingleton<DialogService>();
            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}