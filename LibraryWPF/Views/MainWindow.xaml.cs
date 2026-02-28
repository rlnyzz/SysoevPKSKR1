using LibraryWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace LibraryWPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Получаем зависимости из DI
            var context = App.ServiceProvider.GetRequiredService<SyspevPKSKR1.LibraryDbContext>();
            var dialogService = App.ServiceProvider.GetRequiredService<DialogService>();
            
            DataContext = new MainViewModel(context, dialogService);
        }
    }
}