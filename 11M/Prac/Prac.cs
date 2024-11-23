using System;

public interface IUser
{
    void Register(string name, string email);
    void Login(string email);
}

public interface IBookOperations
{
    void AddBook(Book book);
    void RemoveBook(string isbn);
    void SearchBook(string title);
}

public class Book
{
    public string Title { get; set; }
    public string ISBN { get; set; }
    public List<Author> Authors { get; set; }
    public int PublicationYear { get; set; }
    public bool IsAvailable { get; private set; } = true;

    public Book(string title, string isbn, List<Author> authors, int publicationYear)
    {
        Title = title;
        ISBN = isbn;
        Authors = authors;
        PublicationYear = publicationYear;
    }

    public void ChangeAvailabilityStatus(bool status)
    {
        IsAvailable = status;
    }

    public string GetBookInfo()
    {
        return $"{Title} ({PublicationYear}) by {string.Join(", ", Authors.Select(a => a.Name))}";
    }
}

vpublic class Author
{
    public string Name { get; set; }
    public string Biography { get; set; }

    public Author(string name, string biography)
    {
        Name = name;
        Biography = biography;
    }
}

public abstract class User : IUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    protected User(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public void Register(string name, string email)
    {
        // Логика регистрации
    }

    public void Login(string email)
    {
        // Логика входа
    }
}

public class Reader : User
{
    public Reader(string name, string email) : base(name, email) { }

    public void BorrowBook(Book book)
    {
        if (!book.IsAvailable)
            throw new Exception("Книга недоступна.");

        book.ChangeAvailabilityStatus(false);
        Console.WriteLine($"Книга {book.Title} выдана читателю {Name}.");
    }

    public void ReturnBook(Book book)
    {
        book.ChangeAvailabilityStatus(true);
        Console.WriteLine($"Книга {book.Title} возвращена.");
    }
}

public class Librarian : User, IBookOperations
{
    private List<Book> _libraryBooks;

    public Librarian(string name, string email, List<Book> libraryBooks) : base(name, email)
    {
        _libraryBooks = libraryBooks;
    }

    public void AddBook(Book book)
    {
        _libraryBooks.Add(book);
        Console.WriteLine($"Книга {book.Title} добавлена в библиотеку.");
    }

    public void RemoveBook(string isbn)
    {
        var book = _libraryBooks.FirstOrDefault(b => b.ISBN == isbn);
        if (book == null)
            throw new Exception("Книга не найдена.");

        _libraryBooks.Remove(book);
        Console.WriteLine($"Книга {book.Title} удалена из библиотеки.");
    }

    public void SearchBook(string title)
    {
        var books = _libraryBooks.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        if (books.Count == 0)
        {
            Console.WriteLine("Книги не найдены.");
            return;
        }

        foreach (var book in books)
        {
            Console.WriteLine(book.GetBookInfo());
        }
    }
}

public class Loan
{
    public Book Book { get; set; }
    public Reader Reader { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public Loan(Book book, Reader reader)
    {
        Book = book;
        Reader = reader;
        LoanDate = DateTime.Now;
    }

    public void ReturnBook()
    {
        ReturnDate = DateTime.Now;
        Book.ChangeAvailabilityStatus(true);
        Console.WriteLine($"Книга {Book.Title} возвращена.");
    }
}


class Program
{
    static void Main(string[] args)
    {
        var authors = new List<Author>
        {
            new Author("Лев Толстой", "Русский писатель"),
            new Author("Федор Достоевский", "Русский классик")
        };

        var books = new List<Book>
        {
            new Book("Война и мир", "12345", authors, 1869),
            new Book("Преступление и наказание", "67890", authors, 1866)
        };

        var librarian = new Librarian("Анна", "anna@library.com", books);
        var reader = new Reader("Иван", "ivan@reader.com");

        librarian.SearchBook("Война");
        reader.BorrowBook(books[0]);
        reader.ReturnBook(books[0]);
    }
}

