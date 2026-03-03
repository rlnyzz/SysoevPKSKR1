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
        }

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            
            try
            {
                DataContext = viewModel;
                
                Loaded += MainWindow_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации: {ex.Message}");
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                Title = $"Библиотека - Управление книгами (всего книг: {viewModel.TotalBooks})";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
            }
        }
    }
}