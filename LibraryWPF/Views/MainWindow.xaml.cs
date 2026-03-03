using LibraryWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace LibraryWPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                if (App.ServiceProvider == null)
                {
                    throw new InvalidOperationException("ServiceProvider не инициализирован");
                }

                var context = App.ServiceProvider.GetService<SyspevPKSKR1.LibraryDbContext>();
                var dialogService = App.ServiceProvider.GetService<DialogService>();

                if (context == null)
                {
                    throw new InvalidOperationException("Не удалось получить LibraryDbContext");
                }

                if (dialogService == null)
                {
                    throw new InvalidOperationException("Не удалось получить DialogService");
                }

                DataContext = new MainViewModel(context, dialogService);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации главного окна:\n\n{ex.Message}", 
                    "Ошибка", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                
                Close();
            }
        }
    }
}