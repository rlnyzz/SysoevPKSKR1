using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SyspevPKSKR1;
using System;
using System.Linq;  

namespace SyspevPKSKR1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Это консольное приложение для проверки БД");
            Console.WriteLine("Основное приложение - LibraryWPF\n");

            try
            {
                // Создаем service collection и регистрируем DbContext
                var services = new ServiceCollection();
                
                services.AddDbContext<LibraryDbContext>(options =>
                    options.UseNpgsql("Host=localhost;Port=5432;Database=sysoevpks1;Username=postgres;Password=RussianGrime65"));
                
                var serviceProvider = services.BuildServiceProvider();
                
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                    
                    if (context.Database.CanConnect())
                    {
                        Console.WriteLine("✅ Подключение к БД успешно!\n");
                        
                        var authorsCount = context.Authors.Count();
                        var genresCount = context.Genres.Count();
                        var booksCount = context.Books.Count();
                        
                        Console.WriteLine("📊 СТАТИСТИКА БАЗЫ ДАННЫХ:");
                        Console.WriteLine($"   👤 Авторов: {authorsCount}");
                        Console.WriteLine($"   🎭 Жанров: {genresCount}");
                        Console.WriteLine($"   📚 Книг: {booksCount}");
                        
                        var booksWithAuthors = context.Books
                            .Include(b => b.AuthorNavigation)
                            .Include(b => b.GenreNavigation)
                            .ToList();
                        
                        Console.WriteLine($"\n🔗 СВЯЗИ:");
                        Console.WriteLine($"   Книг с авторами: {booksWithAuthors.Count(b => b.AuthorNavigation != null)}");
                        Console.WriteLine($"   Книг с жанрами: {booksWithAuthors.Count(b => b.GenreNavigation != null)}");
                    }
                    else
                    {
                        Console.WriteLine("❌ Не удалось подключиться к БД");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка: {ex.Message}");
                Console.WriteLine($"\n📋 Детали: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}