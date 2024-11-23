using System;
using System.Collections.Generic;

enum BookStatus
{
    Available,
    Rented
}

class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public BookStatus Status { get; set; } = BookStatus.Available;
}

class Reader
{
    public string Name { get; set; }
    public List<Book> RentedBooks { get; set; } = new List<Book>();
    public int MaxBooksLimit { get; set; } = 3;

    public void RentBook(Book book)
    {
        if (RentedBooks.Count >= MaxBooksLimit)
        {
            Console.WriteLine($"{Name} достиг лимита арендованных книг.");
            return;
        }

        if (book.Status == BookStatus.Rented)
        {
            Console.WriteLine($"Книга \"{book.Title}\" уже арендована.");
            return;
        }

        book.Status = BookStatus.Rented;
        RentedBooks.Add(book);
        Console.WriteLine($"{Name} арендовал книгу \"{book.Title}\".");
    }

    public void ReturnBook(Book book)
    {
        if (RentedBooks.Remove(book))
        {
            book.Status = BookStatus.Available;
            Console.WriteLine($"{Name} вернул книгу \"{book.Title}\".");
        }
        else
        {
            Console.WriteLine($"{Name} не арендовал книгу \"{book.Title}\".");
        }
    }
}

class Librarian
{
    public string Name { get; set; }

    public void AddBook(Library library, Book book)
    {
        library.Books.Add(book);
        Console.WriteLine($"Библиотекарь {Name} добавил книгу \"{book.Title}\".");
    }

    public void RemoveBook(Library library, Book book)
    {
        if (library.Books.Remove(book))
        {
            Console.WriteLine($"Библиотекарь {Name} удалил книгу \"{book.Title}\".");
        }
        else
        {
            Console.WriteLine($"Книга \"{book.Title}\" не найдена в библиотеке.");
        }
    }
}

class Library
{
    public List<Book> Books { get; set; } = new List<Book>();

    public void DisplayAvailableBooks()
    {
        Console.WriteLine("Доступные книги:");
        foreach (var book in Books)
        {
            if (book.Status == BookStatus.Available)
            {
                Console.WriteLine($"- {book.Title} ({book.Author})");
            }
        }
    }

    public void DisplayAllBooks()
    {
        Console.WriteLine("Все книги в библиотеке:");
        foreach (var book in Books)
        {
            string status = book.Status == BookStatus.Available ? "Доступна" : "Арендована";
            Console.WriteLine($"- {book.Title} ({book.Author}) [{status}]");
        }
    }

    public Book SearchBook(string searchTerm)
    {
        return Books.Find(book => book.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                   || book.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }
}


class Program
{
    static void Main(string[] args)
    {
        var library = new Library();
        var librarian = new Librarian { Name = "Анна" };
        var reader = new Reader { Name = "Иван" };

        librarian.AddBook(library, new Book { Title = "Война и мир", Author = "Лев Толстой", ISBN = "12345" });
        librarian.AddBook(library, new Book { Title = "Преступление и наказание", Author = "Федор Достоевский", ISBN = "67890" });

        library.DisplayAvailableBooks();

        var bookToRent = library.SearchBook("Война и мир");
        if (bookToRent != null)
        {
            reader.RentBook(bookToRent);
        }

        library.DisplayAvailableBooks();

        reader.ReturnBook(bookToRent);

        library.DisplayAllBooks();
    }
}

