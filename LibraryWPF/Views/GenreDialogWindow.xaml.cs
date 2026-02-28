using SyspevPKSKR1;
using SyspevPKSKR1.Models;
using System;
using System.Windows;

namespace LibraryWPF.Views
{
    public partial class GenreDialogWindow : Window
    {
        private readonly LibraryDbContext _context;
        private readonly Genre _genre;
        private readonly DialogService _dialogService;

        public GenreDialogWindow(LibraryDbContext context, Genre genre, DialogService dialogService)
        {
            InitializeComponent();
            _context = context;
            _genre = genre;
            _dialogService = dialogService;

            // Заполняем поля при редактировании
            if (genre.Id_genre > 0)
            {
                TitleText.Text = "РЕДАКТИРОВАНИЕ ЖАНРА";
                NameBox.Text = genre.Name_genre?.Trim();
                DescriptionBox.Text = genre.Description_genre?.Trim();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _genre.Name_genre = NameBox.Text?.Trim();
                _genre.Description_genre = DescriptionBox.Text?.Trim();

                if (_genre.Id_genre == 0)
                    _context.Genres.Add(_genre);

                await _context.SaveChangesAsync();
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
    }
}