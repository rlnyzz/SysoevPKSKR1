using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SyspevPKSKR1;
using LibraryWPF.Views;
using LibraryWPF.ViewModels;  
using System;
using System.Windows;

namespace LibraryWPF
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var services = new ServiceCollection();

                services.AddDbContext<LibraryDbContext>(options =>
                    options.UseNpgsql("Host=localhost;Port=5433;Database=sysoevpks1;Username=postgres;Password=RussianGrime65"),
                    ServiceLifetime.Transient);

                services.AddSingleton<DialogService>();
                
                services.AddTransient<MainViewModel>();
                
                services.AddTransient<MainWindow>();

                ServiceProvider = services.BuildServiceProvider();

                using (var scope = ServiceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                    if (!context.Database.CanConnect())
                    {
                        throw new Exception("Не удалось подключиться к базе данных");
                    }
                    
                    Console.WriteLine("✅ Подключение к БД успешно!");
                }

                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при запуске приложения:\n\n{ex.Message}\n\n" +
                    $"Stack: {ex.StackTrace}",
                    "Критическая ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                Shutdown();
            }
        }
    }
}