using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LibraryProgram
{
    class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public bool Loan { get; set; }
        public Book(string title, string author, string genre, int year)
        {
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
        }

        public bool IsDuplicate(Book newBook, Book existingBook)
        {
            return newBook.Title == Title &&
                   newBook.Author == Author &&
                   newBook.Genre == Genre &&
                   newBook.Year == Year;
        }
    }

    public static class ColoredText

    {
        public static void ConsoleColorGreen(string coloredText)
        {
            ConsoleColorGeneric(coloredText, ConsoleColor.Green);
        }
        public static void ConsoleColorRed(string coloredText)
        {
            ConsoleColorGeneric(coloredText, ConsoleColor.Red);
        }
        public static void ConsoleColorBlue(string coloredText)
        {
            ConsoleColorGeneric(coloredText, ConsoleColor.Blue);
        }
        public static void ConsoleColorMagenta(string coloredText)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(coloredText);
            Console.ResetColor();
        }
        public static void ConsoleColorGeneric(string coloredText, ConsoleColor color)
        {

            Console.ForegroundColor = color;
            Console.WriteLine(coloredText);
            Console.ResetColor();
        }
    }

    public class BookComparator
    {
        // Prevent duplicate
        internal bool IsStrictDuplicate(Book newBook, Book existingBook)
        {
            return newBook.Title == existingBook.Title &&
                   newBook.Author == existingBook.Author &&
                   newBook.Genre == existingBook.Genre &&
                   newBook.Year == existingBook.Year;
        }

        // Prevent author error
        internal bool IsDifferentAuthor(Book newBook, Book existingBook)
        {
            return newBook.Title == existingBook.Title &&
                   newBook.Author != existingBook.Author &&
                   newBook.Genre == existingBook.Genre &&
                   newBook.Year == existingBook.Year;
        }

        // Prevent genre and year error
        internal bool IsSameTitleAuthor(Book newBook, Book existingBook)
        {
            return newBook.Title == existingBook.Title &&
                   newBook.Author == existingBook.Author;

        }

        // Prevent genre error
        internal bool IsDifferentGenre(Book newBook, Book existingBook)
        {
            return newBook.Title == existingBook.Title &&
                   newBook.Author == existingBook.Author &&
                   newBook.Genre != existingBook.Genre &&
                   newBook.Year == existingBook.Year;

        }

        // Prevent republished book
        internal bool IsRepublished(Book newBook, Book existingBook)
        {
            return newBook.Title == existingBook.Title &&
                   newBook.Author == existingBook.Author &&
                   newBook.Year != existingBook.Year;
        }

    }

    public class LibraryManager
    {
        private List<Book> listBook = new List<Book>();
        BookComparator bookComparator = new BookComparator();
        private readonly string filePath;

        public LibraryManager()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("pathLibrary.json")
                 .Build();

            // Mandatory file 
            filePath = configuration["libraryFilePath"];

            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("The configuration file does not contain the path");
            }
        }
        public void ShowBooks()
        {
            Console.WriteLine("\nLibrary books :\n");

            foreach (var book in listBook)
            {
                Console.Write($"{book.Title}, {book.Author}");

                if (book.Loan)
                {
                    ColoredText.ConsoleColorRed(" - On loan");
                }
                else
                {
                    ColoredText.ConsoleColorGreen(" - Available");
                }
            }
        }
        public void AddBook()
        {

            try
            {
                Console.WriteLine("\nWhich book would you like to add?");
                ColoredText.ConsoleColorMagenta("Title : ");
                string title = Console.ReadLine().ToUpper();
                ColoredText.ConsoleColorMagenta("Author : ");
                string author = Console.ReadLine().ToUpper();
                ColoredText.ConsoleColorMagenta("Genre : ");
                string genre = Console.ReadLine().ToUpper();
                ColoredText.ConsoleColorMagenta("Year : ");
                int year = int.Parse(Console.ReadLine());
                var newBook = new Book(title, author, genre, year);

                /// TO-DO : Typing error, numbers for author / genre, spaces, case yes / no - Refacto


                foreach (var existingBook in listBook)
                {
                    if (bookComparator.IsRepublished(newBook, existingBook))
                    {
                        ColoredText.ConsoleColorRed($"The book seems to exist, the library doesn't accept republications \nPerhaps you're trying to add {existingBook.Title} from {existingBook.Author}, from the {existingBook.Genre} genre, published in {existingBook.Year} ?");
                        return;
                    }

                    if (bookComparator.IsDifferentGenre(newBook, existingBook))
                    {
                        ColoredText.ConsoleColorRed($"The book seems to exist in a subsidiary genre \nPerhaps you're trying to add {existingBook.Title} from {existingBook.Author}, from the {existingBook.Genre} genre, published in {existingBook.Year} ?");
                        return;
                    }

                    if (bookComparator.IsSameTitleAuthor(newBook, existingBook))
                    {
                        ColoredText.ConsoleColorRed($"The book seems to exist \nPerhaps you're trying to add {existingBook.Title} from {existingBook.Author}, from the {existingBook.Genre}genre, published in {existingBook.Year} ?");
                        return;

                    }

                    if (bookComparator.IsDifferentAuthor(newBook, existingBook))
                    {
                        ColoredText.ConsoleColorRed($"The book seems to exist under a different author \nPerhaps you're trying to add {existingBook.Title} from {existingBook.Author}, from the {existingBook.Genre} genre, published in {existingBook.Year} ?");
                        return;
                    }

                    if (bookComparator.IsStrictDuplicate(newBook, existingBook))
                    {
                        ColoredText.ConsoleColorRed("The book already exists");
                        return;
                    }
                }

                listBook.Add(newBook);
                ColoredText.ConsoleColorGreen("The book has been added");

            }

            catch (FormatException)
            {
                ColoredText.ConsoleColorRed("Please enter numbers in the year section");
            }
        }
        public void DeleteBook()
        {
            Console.WriteLine("\nWhich book would you like to delete?");
            ColoredText.ConsoleColorMagenta("Title :");
            string title = Console.ReadLine().ToUpper();
            ColoredText.ConsoleColorMagenta("Author :");
            string author = Console.ReadLine().ToUpper();
            Book tempBook = null;

            foreach (var book in listBook)
            {
                if (book.Title == title && book.Author == author)
                {
                    tempBook = book;
                }

                if (book.Title == title && book.Author != author)
                {
                    ColoredText.ConsoleColorRed($"The author does not seem correct \nPerhaps you're trying to delete {book.Title} from {book.Author} ?");
                    return;
                }
            }

            if (tempBook != null)
            {
                listBook.Remove(tempBook);
                ColoredText.ConsoleColorGreen("The book has been deleted");
            }

            else
            {
                ColoredText.ConsoleColorRed("The book doesn't exist");
            }
        }
        public void LoadLibrary()
        {
            if (File.Exists(filePath))
            {
                ColoredText.ConsoleColorGreen("Importing the library...");
                string json = File.ReadAllText(filePath);
                listBook = JsonConvert.DeserializeObject<List<Book>>(json);
            }
        }
        public void SaveLibrary()
        {
            if (File.Exists(filePath))
            {
                ColoredText.ConsoleColorGreen("Saving the library...");
                string json = JsonConvert.SerializeObject(listBook, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
        }
        public void LoanBook()
        {
            Console.WriteLine("\nWhich book would you like to borrow? :");
            ColoredText.ConsoleColorMagenta("Title :");
            string title = Console.ReadLine().ToUpper();
            ColoredText.ConsoleColorMagenta("Author :");
            string author = Console.ReadLine().ToUpper();
            Book tempLivre = null;

            foreach (var book in listBook)
            {
                if (book.Title == title && book.Author == author && book.Loan)
                {
                    tempLivre = book;
                    ColoredText.ConsoleColorRed("The book is already on loan");
                }

                if (book.Title == title && book.Author == author && !book.Loan)
                {
                    tempLivre = book;
                    book.Loan = true;
                    ColoredText.ConsoleColorGreen("The book has been borrowed");
                }
            }
            if (tempLivre == null)
            {
                ColoredText.ConsoleColorRed("The book doesn't exist");
            }

        }
        public void ReturnBook()
        {
            Console.WriteLine("\nWhich book would you like to return? :");
            ColoredText.ConsoleColorMagenta("Title : ");
            string title = Console.ReadLine().ToUpper();
            ColoredText.ConsoleColorMagenta("Author :");
            string author = Console.ReadLine().ToUpper();
            Book tempLivre = null;

            foreach (var book in listBook)
            {

                if (book.Title == title && book.Author == author && !book.Loan)
                {
                    tempLivre = book;
                    ColoredText.ConsoleColorRed("The book has not been lent");
                }

                if (book.Title == title && book.Author == author && book.Loan)
                {
                    tempLivre = book;
                    book.Loan = false;
                    ColoredText.ConsoleColorGreen("The book has been returned");
                }
            }

            if (tempLivre == null)
            {
                ColoredText.ConsoleColorRed("The book doesn't exist");
            }
        }
        public void SearchingLibrary()
        {
            List<Book> searchResults = new List<Book>();
            ColoredText.ConsoleColorMagenta("\nType your search : ");

            string searchTerm = Console.ReadLine().ToUpper();

            foreach (var book in listBook)
            {
                if (book.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || book.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    book.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || book.Year.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    book.Loan.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    searchResults.Add(book);
                }
            }

            if (searchResults.Count > 0)
            {
                Console.WriteLine("\nSearch result : \n");
                foreach (var result in searchResults)
                {
                    Console.Write($"{result.Title}, {result.Author}, {result.Genre}, {result.Year}");

                    if (result.Loan == true)
                    {
                        ColoredText.ConsoleColorRed(" - On loan");
                    }
                    else
                    {
                        ColoredText.ConsoleColorGreen(" - Available");
                    }

                    if (searchResults.Count == 1 && result.Loan == false)
                    {
                        string commandLoan = string.Empty;

                        Console.WriteLine("\nWould you like to borrow the book? : ");

                        ColoredText.ConsoleColorBlue("\n[YES] : Borrow the book");
                        ColoredText.ConsoleColorBlue("[NO] : Do not borrow the book");

                        ColoredText.ConsoleColorMagenta("\nType your command : ");
                        commandLoan = Console.ReadLine().ToUpper();

                        switch (commandLoan)
                        {
                            case "OUI":
                                result.Loan = true;
                                ColoredText.ConsoleColorGreen("The book has been borrowed");
                                break;
                            case "NON":
                                result.Loan = false;
                                ColoredText.ConsoleColorRed("The book has not been borrowed");
                                break;
                            default:
                                ColoredText.ConsoleColorRed("\nInvalid command");
                                break;
                        }
                    }
                }
            }
            else
            {
                ColoredText.ConsoleColorRed("No books were found");
            }
        }
    }

    public static class Command
    {
        static void Main()
        {

            LibraryManager libraryManager = new LibraryManager();

            Console.WriteLine("Press enter to display the menu");
            libraryManager.LoadLibrary();
            Console.ReadLine();

            string commands = string.Empty;

            while (commands != "X")
            {
                Console.WriteLine("\nPlease choose a command\n");

                ColoredText.ConsoleColorBlue("[SHOW] : Display library content\n");

                Console.WriteLine("/ Library management :\n");
                ColoredText.ConsoleColorBlue("[ADD] : Add a book");
                ColoredText.ConsoleColorBlue("[DEL] : Delete a book\n");

                Console.WriteLine("/ Loan management :\n");

                ColoredText.ConsoleColorBlue("[TAKE] : Borrow a book");
                ColoredText.ConsoleColorBlue("[GIVE] : Return a book\n");

                ColoredText.ConsoleColorBlue("[SEARCH] : Search for books\n");

                ColoredText.ConsoleColorBlue("[X] : Close the console\n");

                ColoredText.ConsoleColorMagenta("Enter your command : ");

                commands = Console.ReadLine().ToUpper();

                switch (commands)
                {
                    case "SHOW":
                        libraryManager.ShowBooks();
                        break;
                    case "ADD":
                        libraryManager.AddBook();
                        break;
                    case "DEL":
                        libraryManager.DeleteBook();
                        break;
                    case "TAKE":
                        libraryManager.LoanBook();
                        break;
                    case "GIVE":
                        libraryManager.ReturnBook();
                        break;
                    case "SEARCH":
                        libraryManager.SearchingLibrary();
                        break;
                    case "X":
                        libraryManager.SaveLibrary();
                        Console.WriteLine("Closing the application");
                        Environment.Exit(0);
                        break;
                    default:
                        ColoredText.ConsoleColorRed("\nInvalid command");
                        break;
                }
            }
        }
    }
}
