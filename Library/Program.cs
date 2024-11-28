using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        Regex regBookName = new Regex("^[a-zA-Z0-9 ]+$");
        Regex regAuthor = new Regex("^[a-zA-Z ]+$");
        Regex regInt = new Regex("^[0-9]+$");

        string filepath = "C:\\Users\\us store\\Desktop\\examWorks\\library\\bookList.txt";

        //ვამოწმებთ ფაილი არსებობს თუ არა და ცარიელია თუ არა
        if (!File.Exists(filepath) || string.IsNullOrWhiteSpace(File.ReadAllText(filepath)))
        {
            //File.WriteAllText(filepath, "[]");
            Console.WriteLine("File is Empty");
        }

        //კითხულობს მონაცემებს ფაილიდან
        string readFromJson = File.ReadAllText(filepath);
        List<BookList> book = JsonSerializer.Deserialize<List<BookList>>(readFromJson) ?? new List<BookList>();

        while (true)
        {
            Console.WriteLine("\nMenu: ");
            Console.WriteLine("1 - View Book List");
            Console.WriteLine("2 - Search for a Book");
            Console.WriteLine("3 - Add a Book to the Catalog");
            Console.WriteLine("4 - Exit menu");
            Console.Write("Enter service number: ");

            if (int.TryParse(Console.ReadLine().Trim(), out int selectService) && selectService > 0 && selectService < 5)
            {
                switch (selectService)
                {
                    case 1: 
                        if (book.Count == 0)
                        {
                            Console.WriteLine("The catalog is empty.");
                        }
                        else
                        {
                            foreach (var b in book)
                                b.DisplayBook();
                        }
                        break;

                    case 2: //წიგნის არასრული სახელით ძებნის დროს ვიყენებთ ასინქრონულ მეთოდს და 
                            //გვიძებნის ყველა შესაბამის წიგნს
                        Console.Write("Enter the Title of the book: ");
                        string searchBook = Console.ReadLine().Trim();
                        var matchedBooks = await SearchbooksAsync(book.ToArray(), searchBook);

                        if (matchedBooks.Any())
                        {
                            Console.WriteLine("\nBooks found:");
                            foreach (var b in matchedBooks)
                                b.DisplayBook();
                        }
                        else
                        {
                            Console.WriteLine("No books found with the given title.");
                        }
                        break;

                    case 3: //ვამატებთ წიგნს სახელით, ავტორით და გამოცემის წლით. 
                            //ვამოწმებთ შეყვანილი ტექსტს RegExp-ით
                        Console.Write("Enter the Title of the book: ");
                        string title = Console.ReadLine().Trim();
                        if (!regBookName.IsMatch(title))
                        {
                            Console.WriteLine("Invalid title format. Use alphabetic and numeric characters only.");
                            break;
                        }

                        Console.Write("Enter the Author: ");
                        string author = Console.ReadLine().Trim();
                        if (!regAuthor.IsMatch(author))
                        {
                            Console.WriteLine("Invalid author name format. Use alphabetic characters only.");
                            break;
                        }

                        Console.Write("Enter the Publish Year: ");
                        if (!int.TryParse(Console.ReadLine().Trim(), out int year) || !regInt.IsMatch(year.ToString()))
                        {
                            Console.WriteLine("Invalid publish year.");
                            break;
                        }
                        //ვამოწმებთ ანალოგიური წიგნი თუ უკვე არსებობს კატალოგში
                        if (book.Any(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase) &&
                                          b.Author.Equals(author, StringComparison.OrdinalIgnoreCase) &&
                                          b.Year == year))
                        {
                            Console.WriteLine("This book is already in the catalog.");
                        }
                        else
                        {
                            book.Add(new BookList(title, author, year));
                            File.WriteAllText(filepath, JsonSerializer.Serialize(book, new JsonSerializerOptions { WriteIndented = true }));
                            Console.WriteLine("Book added successfully!");
                        }
                        break;

                    case 4: 
                        Console.WriteLine("You exited the Menu.");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please select a valid menu item.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
            }
        }
    }

    //ვქმნით ასინქრონულ მეთოდს წიგნის ძიებისთვის. 
    public static Task<BookList[]> SearchbooksAsync(BookList[] books, string keyword)
    {
        return Task.Run(() =>
            books
                .Where(b => b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToArray()
        );
    }
}

public class BookList
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    public BookList(string title, string author, int year)
    {
        Title = title;
        Author = author;
        Year = year;
    }

    public void DisplayBook()
    {
        Console.WriteLine($"Title: {Title}, Author: {Author}, Published Year: {Year}");
    }
}
