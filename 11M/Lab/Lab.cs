using System;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public bool IsAvailable { get; private set; } = true;

    public Book(string title, string author, string isbn)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
    }

    public void MarkAsLoaned()
    {
        IsAvailable = false;
    }

    public void MarkAsAvailable()
    {
        IsAvailable = true;
    }
}

public class Reader
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public Reader(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public void BorrowBook(Book book)
    {
        if (!book.IsAvailable)
            throw new Exception($"Книга {book.Title} недоступна.");
        book.MarkAsLoaned();
        Console.WriteLine($"{Name} взял книгу {book.Title}.");
    }

    public void ReturnBook(Book book)
    {
        book.MarkAsAvailable();
        Console.WriteLine($"{Name} вернул книгу {book.Title}.");
    }
}

public class Librarian
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }

    public Librarian(int id, string name, string position)
    {
        Id = id;
        Name = name;
        Position = position;
    }

    public void AddBook(List<Book> libraryBooks, Book book)
    {
        libraryBooks.Add(book);
        Console.WriteLine($"Книга {book.Title} добавлена библиотекарем {Name}.");
    }

    public void RemoveBook(List<Book> libraryBooks, string isbn)
    {
        var book = libraryBooks.FirstOrDefault(b => b.ISBN == isbn);
        if (book == null)
            throw new Exception("Книга не найдена.");
        libraryBooks.Remove(book);
        Console.WriteLine($"Книга {book.Title} удалена библиотекарем {Name}.");
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

    public void CompleteLoan()
    {
        ReturnDate = DateTime.Now;
        Book.MarkAsAvailable();
        Console.WriteLine($"Книга {Book.Title} возвращена {Reader.Name}.");
    }
}


class Program
{
    static void Main(string[] args)
    {
        var books = new List<Book>
        {
            new Book("1984", "George Orwell", "12345"),
            new Book("To Kill a Mockingbird", "Harper Lee", "67890")
        };

        var librarian = new Librarian(1, "Anna", "Senior Librarian");
        var reader = new Reader(1, "John", "john@example.com");

        librarian.AddBook(books, new Book("Brave New World", "Aldous Huxley", "11223"));
        reader.BorrowBook(books[0]);
        reader.ReturnBook(books[0]);
    }
}
