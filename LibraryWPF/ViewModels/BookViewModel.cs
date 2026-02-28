using SyspevPKSKR1.Models;
using System;

namespace LibraryWPF.ViewModels
{
    public class BookViewModel : ViewModelBase
    {
        private Book _book;

        public BookViewModel(Book book)
        {
            _book = book ?? new Book();
        }

        public int Id => _book.Id;

        public string Title
        {
            get => _book.Title;
            set
            {
                _book.Title = value;
                OnPropertyChanged();
            }
        }

        public string Author
        {
            get => _book.Author;
            set
            {
                _book.Author = value;
                OnPropertyChanged();
            }
        }

        public string Genre
        {
            get => _book.Genre;
            set
            {
                _book.Genre = value;
                OnPropertyChanged();
            }
        }

        public int? PublishYear
        {
            get => _book.Publishyear;
            set
            {
                _book.Publishyear = value;
                OnPropertyChanged();
            }
        }

        public int? Isbn
        {
            get => _book.Isbn;
            set
            {
                _book.Isbn = value;
                OnPropertyChanged();
            }
        }

        // ✅ ТОЛЬКО ОДНО СВОЙСТВО QuantityInStock!
        public int? QuantityInStock
        {
            get => _book.Quantityinstock;
            set
            {
                _book.Quantityinstock = value;
                OnPropertyChanged();
            }
        }

        public string AuthorName => _book.AuthorNavigation != null 
            ? $"{_book.AuthorNavigation.Firstname?.Trim()} {_book.AuthorNavigation.Lastname?.Trim()}" 
            : _book.Author;

        public string GenreName => _book.GenreNavigation != null 
            ? _book.GenreNavigation.Name_genre?.Trim() 
            : _book.Genre;

        public Book GetBook() => _book;
    }
}