using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SyspevPKSKR1.Models; 

namespace SyspevPKSKR1  
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=".PadRight(60, '='));
            Console.WriteLine("     ПРОЕКТ: SyspevPKSKR1");
            Console.WriteLine("     РЕШЕНИЕ: SysoevPKSKR1");
            Console.WriteLine("     БАЗА ДАННЫХ: sysoevpks1");
            Console.WriteLine("=".PadRight(60, '='));
            
            using (var context = new LibraryDbContext())
            {
                try
                {
                    Console.Write("\n📡 Подключение к PostgreSQL... ");
                    
                    if (context.Database.CanConnect())
                    {
                        Console.WriteLine("✅ УСПЕШНО\n");
                        
                        var books = context.Books
                            .Include(b => b.AuthorNavigation)
                            .Include(b => b.GenreNavigation)
                            .ToList();
                        
                        Console.WriteLine($"📊 Книг в базе: {books.Count}");
                        Console.WriteLine($"👤 Авторов: {context.Authors.Count()}");
                        Console.WriteLine($"🎭 Жанров: {context.Genres.Count()}");
                        
                        if (books.Any())
                        {
                            Console.WriteLine("\n📖 ПЕРВЫЕ 5 КНИГ:\n");
                            foreach (var book in books.Take(5))
                            {
                                Console.WriteLine($"ID: {book.Id}");
                                Console.WriteLine($"Название: {book.Title}");
                                Console.WriteLine($"Автор: {book.AuthorNavigation?.Firstname} {book.AuthorNavigation?.Lastname ?? book.Author}");
                                Console.WriteLine($"Жанр: {book.GenreNavigation?.Name_genre?.Trim() ?? book.Genre}");
                                Console.WriteLine($"Год: {book.Publishyear}");
                                Console.WriteLine("-".PadRight(40, '-'));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ ОШИБКА ПОДКЛЮЧЕНИЯ");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ ОШИБКА: {ex.Message}");
                }
            }
            
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}