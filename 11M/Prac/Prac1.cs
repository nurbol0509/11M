using System;

public interface ICatalog
{
    void AddBook(Book book);
    void RemoveBook(string isbn);
    List<Book> SearchBooks(string query, string filter);
}

public interface ILoanSystem
{
    void LoanBook(string isbn, Reader reader);
    void ReturnBook(string isbn, Reader reader);
    List<LoanRecord> GetLoansByReader(Reader reader);
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string ISBN { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Book(string title, string author, string genre, string isbn)
    {
        Title = title;
        Author = author;
        Genre = genre;
        ISBN = isbn;
    }
}

public class Reader
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string TicketNumber { get; set; }

    public Reader(string firstName, string lastName, string ticketNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        TicketNumber = ticketNumber;
    }
}

public class Librarian
{
    private readonly ICatalog _catalog;
    private readonly ILoanSystem _loanSystem;

    public Librarian(ICatalog catalog, ILoanSystem loanSystem)
    {
        _catalog = catalog;
        _loanSystem = loanSystem;
    }

    public void IssueBook(string isbn, Reader reader)
    {
        _loanSystem.LoanBook(isbn, reader);
    }

    public void ReturnBook(string isbn, Reader reader)
    {
        _loanSystem.ReturnBook(isbn, reader);
    }
}

public class Catalog : ICatalog
{
    private List<Book> _books = new List<Book>();

    public void AddBook(Book book)
    {
        _books.Add(book);
    }

    public void RemoveBook(string isbn)
    {
        _books.RemoveAll(b => b.ISBN == isbn);
    }

    public List<Book> SearchBooks(string query, string filter)
    {
        return filter switch
        {
            "Title" => _books.Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList(),
            "Author" => _books.Where(b => b.Author.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList(),
            "Genre" => _books.Where(b => b.Genre.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => new List<Book>()
        };
    }
}

public class LoanSystem : ILoanSystem
{
    private List<LoanRecord> _loanRecords = new List<LoanRecord>();

    public void LoanBook(string isbn, Reader reader)
    {
        var record = _loanRecords.FirstOrDefault(r => r.Book.ISBN == isbn && r.IsReturned == false);
        if (record != null) throw new Exception("Книга уже выдана.");

        _loanRecords.Add(new LoanRecord
        {
            Book = new Book("", "", "", isbn) { IsAvailable = false },
            Reader = reader,
            LoanDate = DateTime.Now,
            IsReturned = false
        });
    }

    public void ReturnBook(string isbn, Reader reader)
    {
        var record = _loanRecords.FirstOrDefault(r => r.Book.ISBN == isbn && r.Reader == reader && !r.IsReturned);
        if (record == null) throw new Exception("Данной книги нет в списке выданных.");

        record.IsReturned = true;
        record.ReturnDate = DateTime.Now;
    }

    public List<LoanRecord> GetLoansByReader(Reader reader)
    {
        return _loanRecords.Where(r => r.Reader == reader).ToList();
    }
}

public class LoanRecord
{
    public Book Book { get; set; }
    public Reader Reader { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        ICatalog catalog = new Catalog();
        ILoanSystem loanSystem = new LoanSystem();
        Librarian librarian = new Librarian(catalog, loanSystem);

        catalog.AddBook(new Book("Война и мир", "Лев Толстой", "Роман", "12345"));
        catalog.AddBook(new Book("Мастер и Маргарита", "М. Булгаков", "Фантастика", "54321"));

        Reader reader = new Reader("Иван", "Иванов", "001");

        librarian.IssueBook("12345", reader);

        var books = catalog.SearchBooks("Мастер", "Title");
        foreach (var book in books)
        {
            Console.WriteLine($"Книга: {book.Title}, Автор: {book.Author}");
        }

        librarian.ReturnBook("12345", reader);

        Console.WriteLine("Все операции завершены.");
    }
}
