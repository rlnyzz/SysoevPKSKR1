using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SyspevPKSKR1;
using LibraryWPF.Views;
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
                    options.UseNpgsql("Host=localhost;Port=5432;Database=sysoevpks1;Username=postgres;Password=RussianGrime65"),
                    ServiceLifetime.Transient);

                services.AddSingleton<DialogService>();
                services.AddTransient<MainWindow>();

                ServiceProvider = services.BuildServiceProvider();

                if (ServiceProvider == null)
                {
                    throw new InvalidOperationException("Не удалось создать ServiceProvider");
                }

                var mainWindow = ServiceProvider.GetService<MainWindow>();
                
                if (mainWindow == null)
                {
                    throw new InvalidOperationException("Не удалось создать главное окно");
                }

                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при запуске приложения:\n\n{ex.Message}\n\n" +
                    $"Проверьте:\n" +
                    $"1. Запущен ли PostgreSQL\n" +
                    $"2. Правильный ли пароль в строке подключения\n" +
                    $"3. Существует ли база данных sysoevpks1",
                    "Критическая ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                Shutdown();
            }
        }

        public static T? GetService<T>() where T : class
        {
            return ServiceProvider?.GetService<T>();
        }
    }
}