using Microsoft.EntityFrameworkCore;
using SyspevPKSKR1;
using SyspevPKSKR1.Models;
using System;
using System.Linq;
using System.Windows;

namespace LibraryWPF.Views
{
    public partial class GenresWindow : Window
    {
        private readonly LibraryDbContext _context;
        private readonly DialogService _dialogService;

        public GenresWindow(LibraryDbContext context, DialogService dialogService)
        {
            InitializeComponent();
            _context = context;
            _dialogService = dialogService;
            LoadGenres();
        }

        private void LoadGenres()
        {
            var genres = _context.Genres
                .OrderBy(g => g.Name_genre)
                .ToList();
            
            GenresGrid.ItemsSource = genres;
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GenreDialogWindow(_context, new Genre(), _dialogService);
            if (dialog.ShowDialog() == true)
            {
                LoadGenres();
                _dialogService.ShowInfo("Жанр успешно добавлен!");
            }
        }

        private void EditGenre_Click(object sender, RoutedEventArgs e)
        {
            var genre = GenresGrid.SelectedItem as Genre;
            if (genre == null)
            {
                _dialogService.ShowWarning("Выберите жанр для редактирования");
                return;
            }

            var dialog = new GenreDialogWindow(_context, genre, _dialogService);
            if (dialog.ShowDialog() == true)
            {
                LoadGenres();
                _dialogService.ShowInfo("Жанр успешно обновлен!");
            }
        }

        private async void DeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            var genre = GenresGrid.SelectedItem as Genre;
            if (genre == null)
            {
                _dialogService.ShowWarning("Выберите жанр для удаления");
                return;
            }

            var result = _dialogService.ShowConfirmation(
                "Удаление жанра",
                $"Вы уверены, что хотите удалить жанр '{genre.Name_genre?.Trim()}'?\n\nПри удалении жанра, у всех книг этого жанра будет снята связь (genre_id = NULL).");

            if (result == true)
            {
                try
                {
                    _context.Genres.Remove(genre);
                    await _context.SaveChangesAsync();
                    LoadGenres();
                    _dialogService.ShowInfo("Жанр успешно удален!");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"Ошибка при удалении: {ex.Message}");
                }
            }
        }
    }
}