using Microsoft.EntityFrameworkCore;
using SyspevPKSKR1;
using SyspevPKSKR1.Models;
using LibraryWPF.ViewModels;
using System;
using System.Linq;
using System.Windows;

namespace LibraryWPF.Views
{
    public partial class BookWindow : Window
    {
        private readonly LibraryDbContext _context;
        private readonly BookViewModel _viewModel;
        private readonly DialogService _dialogService;

        // ✅ КОНСТРУКТОР С 3 ПАРАМЕТРАМИ
        public BookWindow(LibraryDbContext context, BookViewModel viewModel, DialogService dialogService)
        {
            InitializeComponent();
            
            _context = context;
            _viewModel = viewModel;
            _dialogService = dialogService;

            DataContext = viewModel;

            // Загружаем авторов и жанры
            LoadAuthors();
            LoadGenres();

            // Заполняем поля
            TitleBox.Text = viewModel.Title;
            AuthorManualBox.Text = viewModel.Author;
            GenreManualBox.Text = viewModel.Genre;
            YearBox.Text = viewModel.PublishYear?.ToString();
            IsbnBox.Text = viewModel.Isbn?.ToString();
            QuantityBox.Text = viewModel.QuantityInStock?.ToString();  // ✅ ТЕПЕРЬ РАБОТАЕТ!

            // Выбираем автора в ComboBox
            var authorNav = viewModel.GetBook().AuthorNavigation;
            if (authorNav != null)
            {
                AuthorComboBox.SelectedItem = authorNav;
            }

            // Выбираем жанр в ComboBox
            var genreNav = viewModel.GetBook().GenreNavigation;
            if (genreNav != null)
            {
                GenreComboBox.SelectedItem = genreNav;
            }

            // Обновляем заголовок
            TitleText.Text = viewModel.Id > 0 ? "РЕДАКТИРОВАНИЕ КНИГИ" : "ДОБАВЛЕНИЕ КНИГИ";
        }

        private void LoadAuthors()
        {
            try
            {
                var authors = _context.Authors
                    .OrderBy(a => a.Lastname)
                    .ThenBy(a => a.Firstname)
                    .ToList();
                
                AuthorComboBox.ItemsSource = authors;
                AuthorComboBox.DisplayMemberPath = "FullNameReverse";
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка загрузки авторов: {ex.Message}");
            }
        }

        private void LoadGenres()
        {
            try
            {
                var genres = _context.Genres
                    .OrderBy(g => g.Name_genre)
                    .ToList();
                
                GenreComboBox.ItemsSource = genres;
                GenreComboBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка загрузки жанров: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(TitleBox.Text))
                {
                    _dialogService.ShowError("Название книги обязательно!");
                    TitleBox.Focus();
                    return;
                }

                // Сохраняем данные
                _viewModel.Title = TitleBox.Text.Trim();
                _viewModel.Author = AuthorManualBox.Text?.Trim();
                _viewModel.Genre = GenreManualBox.Text?.Trim();

                // Год
                if (!string.IsNullOrWhiteSpace(YearBox.Text))
                {
                    if (int.TryParse(YearBox.Text, out int year))
                    {
                        _viewModel.PublishYear = year;
                    }
                }

                // ISBN
                if (!string.IsNullOrWhiteSpace(IsbnBox.Text))
                {
                    if (int.TryParse(IsbnBox.Text, out int isbn))
                    {
                        _viewModel.Isbn = isbn;
                    }
                }

                // Количество - ✅ ТЕПЕРЬ РАБОТАЕТ!
                if (!string.IsNullOrWhiteSpace(QuantityBox.Text))
                {
                    if (int.TryParse(QuantityBox.Text, out int quantity))
                    {
                        _viewModel.QuantityInStock = quantity;
                    }
                }

                // Связи
                var book = _viewModel.GetBook();
                book.AuthorNavigation = AuthorComboBox.SelectedItem as Author;
                book.AuthorId = book.AuthorNavigation?.Id_author;
                book.GenreNavigation = GenreComboBox.SelectedItem as Genre;
                book.GenreId = book.GenreNavigation?.Id_genre;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AuthorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AuthorComboBox.SelectedItem != null)
                AuthorManualBox.Text = "";
        }

        private void GenreComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (GenreComboBox.SelectedItem != null)
                GenreManualBox.Text = "";
        }

        private void AuthorManualBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AuthorManualBox.Text))
                AuthorComboBox.SelectedItem = null;
        }

        private void GenreManualBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(GenreManualBox.Text))
                GenreComboBox.SelectedItem = null;
        }
    }
}