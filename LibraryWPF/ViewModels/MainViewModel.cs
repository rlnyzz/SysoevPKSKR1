using Microsoft.EntityFrameworkCore;
using SyspevPKSKR1;
using SyspevPKSKR1.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LibraryWPF.Views;

namespace LibraryWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LibraryDbContext _context;
        private readonly DialogService _dialogService;

        // Поля
        private ObservableCollection<BookViewModel> _books;
        private ObservableCollection<Author> _authors;
        private ObservableCollection<Genre> _genres;
        private BookViewModel _selectedBook;
        private string _searchText;
        private Author _selectedAuthorFilter;
        private Genre _selectedGenreFilter;

        // Конструктор
        public MainViewModel(LibraryDbContext context, DialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            Books = new ObservableCollection<BookViewModel>();
            Authors = new ObservableCollection<Author>();
            Genres = new ObservableCollection<Genre>();

            _ = LoadDataAsync();

            AddBookCommand = new RelayCommand(AddBook);
            EditBookCommand = new RelayCommand(EditBook, () => SelectedBook != null);
            DeleteBookCommand = new RelayCommand(DeleteBook, () => SelectedBook != null);
            ManageAuthorsCommand = new RelayCommand(ManageAuthors);
            ManageGenresCommand = new RelayCommand(ManageGenres);
            RefreshCommand = new RelayCommand(() => _ = LoadDataAsync());
        }

        // Свойства
        public ObservableCollection<BookViewModel> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public ObservableCollection<Author> Authors
        {
            get => _authors;
            set => SetProperty(ref _authors, value);
        }

        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set => SetProperty(ref _genres, value);
        }

        public BookViewModel SelectedBook
        {
            get => _selectedBook;
            set
            {
                SetProperty(ref _selectedBook, value);
                (EditBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterBooks();
            }
        }

        public Author SelectedAuthorFilter
        {
            get => _selectedAuthorFilter;
            set
            {
                SetProperty(ref _selectedAuthorFilter, value);
                FilterBooks();
            }
        }

        public Genre SelectedGenreFilter
        {
            get => _selectedGenreFilter;
            set
            {
                SetProperty(ref _selectedGenreFilter, value);
                FilterBooks();
            }
        }

        public int TotalBooks => Books?.Count ?? 0;
        
        // ИСПРАВЛЕНО: lsbn -> QuantityInStock
        public int TotalInStock => Books?.Sum(b => b.QuantityInStock ?? 0) ?? 0;

        // Команды
        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand ManageAuthorsCommand { get; }
        public ICommand ManageGenresCommand { get; }
        public ICommand RefreshCommand { get; }

        // Методы
        private async Task LoadDataAsync()
        {
            try
            {
                // Загрузка авторов
                var authors = await _context.Authors
                    .OrderBy(a => a.Lastname)
                    .ThenBy(a => a.Firstname)
                    .ToListAsync();
                
                Authors.Clear();
                foreach (var author in authors)
                    Authors.Add(author);

                // Загрузка жанров
                var genres = await _context.Genres
                    .OrderBy(g => g.Name_genre)
                    .ToListAsync();
                
                Genres.Clear();
                foreach (var genre in genres)
                    Genres.Add(genre);

                // Загрузка книг
                var books = await _context.Books
                    .Include(b => b.AuthorNavigation)
                    .Include(b => b.GenreNavigation)
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                Books.Clear();
                foreach (var book in books)
                    Books.Add(new BookViewModel(book));

                OnPropertyChanged(nameof(TotalBooks));
                OnPropertyChanged(nameof(TotalInStock));
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void FilterBooks()
        {
            try
            {
                var query = _context.Books
                    .Include(b => b.AuthorNavigation)
                    .Include(b => b.GenreNavigation)
                    .AsQueryable();

                // Поиск по названию, автору, ISBN
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(b => 
                        b.Title.Contains(SearchText) || 
                        b.Author.Contains(SearchText) ||
                        (b.Isbn != null && b.Isbn.ToString().Contains(SearchText)));  // ИСПРАВЛЕНО: lsbn -> Isbn
                }

                // Фильтр по автору
                if (SelectedAuthorFilter != null)
                {
                    query = query.Where(b => 
                        b.AuthorId == SelectedAuthorFilter.Id_author);
                }

                // Фильтр по жанру
                if (SelectedGenreFilter != null)
                {
                    query = query.Where(b => 
                        b.GenreId == SelectedGenreFilter.Id_genre);
                }

                var filteredBooks = query.OrderBy(b => b.Title).ToList();
                
                Books.Clear();
                foreach (var book in filteredBooks)
                    Books.Add(new BookViewModel(book));

                OnPropertyChanged(nameof(TotalBooks));
                OnPropertyChanged(nameof(TotalInStock));
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка фильтрации: {ex.Message}");
            }
        }

      private async void AddBook()
{
    try
    {
        var newBook = new Book();
        var bookVm = new BookViewModel(newBook);
        
        // ✅ ВЫЗОВ КОНСТРУКТОРА С 3 ПАРАМЕТРАМИ
        var window = new BookWindow(_context, bookVm, _dialogService);
        if (window.ShowDialog() == true)
        {
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
            Books.Add(bookVm);
            _dialogService.ShowInfo("Книга успешно добавлена!");
        }
    }
    catch (Exception ex)
    {
        _dialogService.ShowError($"Ошибка: {ex.Message}");
    }
}

private async void EditBook()
{
    if (SelectedBook == null) return;

    try
    {
        var book = SelectedBook.GetBook();
        var bookVm = new BookViewModel(book);
        
        // ✅ ВЫЗОВ КОНСТРУКТОРА С 3 ПАРАМЕТРАМИ
        var window = new BookWindow(_context, bookVm, _dialogService);
        if (window.ShowDialog() == true)
        {
            await _context.SaveChangesAsync();
            var index = Books.IndexOf(SelectedBook);
            Books[index] = bookVm;
            _dialogService.ShowInfo("Книга успешно обновлена!");
        }
    }
    catch (Exception ex)
    {
        _dialogService.ShowError($"Ошибка: {ex.Message}");
    }
}

        private async void DeleteBook()
        {
            if (SelectedBook == null) return;

            var result = _dialogService.ShowConfirmation(
                "Удаление книги",
                $"Вы уверены, что хотите удалить книгу '{SelectedBook.Title}'?");

            if (result == true)
            {
                try
                {
                    var book = SelectedBook.GetBook();
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                    Books.Remove(SelectedBook);
                    OnPropertyChanged(nameof(TotalBooks));
                    OnPropertyChanged(nameof(TotalInStock));
                    _dialogService.ShowInfo("Книга успешно удалена!");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void ManageAuthors()
        {
            var window = new AuthorsWindow(_context, _dialogService);
            window.ShowDialog();
            _ = LoadDataAsync();
        }

        private void ManageGenres()
        {
            var window = new GenresWindow(_context, _dialogService);
            window.ShowDialog();
            _ = LoadDataAsync();
        }
    }
}