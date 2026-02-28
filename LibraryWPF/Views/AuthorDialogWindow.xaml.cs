using SyspevPKSKR1;
using SyspevPKSKR1.Models;
using System;
using System.Windows;

namespace LibraryWPF.Views
{
    public partial class AuthorDialogWindow : Window
    {
        private readonly LibraryDbContext _context;
        private readonly Author _author;
        private readonly DialogService _dialogService;

        public AuthorDialogWindow(LibraryDbContext context, Author author, DialogService dialogService)
        {
            InitializeComponent();
            _context = context;
            _author = author;
            _dialogService = dialogService;

            // Заполняем поля при редактировании
            if (author.Id_author > 0)
            {
                TitleText.Text = "РЕДАКТИРОВАНИЕ АВТОРА";
                FirstNameBox.Text = author.Firstname?.Trim();
                LastNameBox.Text = author.Lastname?.Trim();
                BirthDatePicker.SelectedDate = author.Birthdate;
                CountryBox.Text = author.Country?.Trim();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _author.Firstname = FirstNameBox.Text?.Trim();
                _author.Lastname = LastNameBox.Text?.Trim();
                _author.Birthdate = BirthDatePicker.SelectedDate;
                _author.Country = CountryBox.Text?.Trim();

                if (_author.Id_author == 0)
                    _context.Authors.Add(_author);

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