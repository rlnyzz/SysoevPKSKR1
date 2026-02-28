using Microsoft.EntityFrameworkCore;
using SyspevPKSKR1;
using SyspevPKSKR1.Models;
using System;
using System.Linq;
using System.Windows;

namespace LibraryWPF.Views
{
    public partial class AuthorsWindow : Window
    {
        private readonly LibraryDbContext _context;
        private readonly DialogService _dialogService;

        public AuthorsWindow(LibraryDbContext context, DialogService dialogService)
        {
            InitializeComponent();
            _context = context;
            _dialogService = dialogService;
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            var authors = _context.Authors
                .OrderBy(a => a.Lastname)
                .ThenBy(a => a.Firstname)
                .ToList();
            
            AuthorsGrid.ItemsSource = authors;
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AuthorDialogWindow(_context, new Author(), _dialogService);
            if (dialog.ShowDialog() == true)
            {
                LoadAuthors();
                _dialogService.ShowInfo("Автор успешно добавлен!");
            }
        }

        private void EditAuthor_Click(object sender, RoutedEventArgs e)
        {
            var author = AuthorsGrid.SelectedItem as Author;
            if (author == null)
            {
                _dialogService.ShowWarning("Выберите автора для редактирования");
                return;
            }

            var dialog = new AuthorDialogWindow(_context, author, _dialogService);
            if (dialog.ShowDialog() == true)
            {
                LoadAuthors();
                _dialogService.ShowInfo("Автор успешно обновлен!");
            }
        }

        private async void DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            var author = AuthorsGrid.SelectedItem as Author;
            if (author == null)
            {
                _dialogService.ShowWarning("Выберите автора для удаления");
                return;
            }

            var result = _dialogService.ShowConfirmation(
                "Удаление автора",
                $"Вы уверены, что хотите удалить автора {author.Firstname?.Trim()} {author.Lastname?.Trim()}?\n\nПри удалении автора, у всех его книг будет снята связь (author_id = NULL).");

            if (result == true)
            {
                try
                {
                    _context.Authors.Remove(author);
                    await _context.SaveChangesAsync();
                    LoadAuthors();
                    _dialogService.ShowInfo("Автор успешно удален!");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"Ошибка при удалении: {ex.Message}");
                }
            }
        }
    }
}