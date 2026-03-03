using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SyspevPKSKR1;
using SyspevPKSKR1.Models; 
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

                // Проверка подключения к БД при запуске
                using (var scope = ServiceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<LibraryDbContext>();
                    if (dbContext != null && dbContext.Database.CanConnect())
                    {
                        Console.WriteLine("✅ Подключение к БД успешно!");
                        
                        var authorsCount = dbContext.Authors.Count();
                        var genresCount = dbContext.Genres.Count();
                        var booksCount = dbContext.Books.Count();
                        
                        Console.WriteLine($"📊 Статистика: Авторов: {authorsCount}, Жанров: {genresCount}, Книг: {booksCount}");
                    }
                    else
                    {
                        throw new Exception("Не удалось подключиться к базе данных");
                    }
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
                    $"2. Правильный ли пароль в строке подключения (сейчас: '1')\n" +
                    $"3. Существует ли база данных sysoevpks1\n" +
                    $"4. Созданы ли таблицы (author, book, genre)",
                    "Критическая ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                Shutdown();
            }
        }
    }
}