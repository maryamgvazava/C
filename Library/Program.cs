using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Program
{
    static void Main(string[] args)
    {
        Regex regBookName = new Regex("^[a-zA-Z0-9]+$"); 
        Regex regAuthor = new Regex("^[a-zA-Z]+$");
        Regex regInt = new Regex("^[0-9]+$");
        string filepath = "C:\\Users\\us store\\Desktop\\examWorks\\library\\bookList.txt";

        string readFromJson = File.ReadAllText(filepath);
        List<BookList> book = JsonSerializer.Deserialize<List<BookList>>(readFromJson);

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Menu: ");
            Console.WriteLine("1 - View Book List");
            Console.WriteLine("2 - Search for a Book");
            Console.WriteLine("3 - Add a Book to the Catalog");
            Console.WriteLine("4 - Exit menu");
            Console.WriteLine();
            Console.WriteLine("Enter service number:");

            if (int.TryParse(Console.ReadLine(), out int selectService) && selectService > 0 && selectService < 5)
            {
                switch (selectService)
                {
                    case 1:
                        {
                            foreach (var i in book)
                            {
                                i.DisplayBook();
                            }
                            break;
                        }

                    case 2:
                        {
                            Console.WriteLine("Enter the Title of the book:");
                            string searchBook = Console.ReadLine();
                            bool found = false;
                            foreach (var title in book)
                            {
                                if (string.Equals(searchBook, title.Title, StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine($"{title.Title} is in the catalog!");
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                Console.WriteLine("Entered title is not in the catalog or there was a typing error.");
                            }
                            break;
                        }



                    case 3:
                        {
                            string title, author;
                            int year;

                            Console.WriteLine("Enter the Title of the book:");
                            title = Console.ReadLine();
                            if (!regBookName.IsMatch(title))
                            {
                                Console.WriteLine("Title should contain alphabet characters only.");
                                break;
                            }
                 
                            Console.WriteLine("Enter the Author:");
                            author = Console.ReadLine();
                            if (!regAuthor.IsMatch(author))
                            {
                                Console.WriteLine("Author name should contain alphabet characters only.");
                                break;
                            }
                      
                            Console.WriteLine("Enter the Publish Year:");
                            string yearInput = Console.ReadLine();
                            if (!regInt.IsMatch(yearInput) || !int.TryParse(yearInput, out year))
                            {
                                Console.WriteLine("Invalid publish year.");
                                break;
                            }

                            bool found = false;
                            BookList newBook = new BookList(title, author, year);

                            foreach (var each in book)
                            { //თუ იდენტური სახელის, ავტორის და გამოცმის წლის წიგნი არსებობს, მაშინ არ უნდა დაამატოს
                                if (string.Equals(title, each.Title, StringComparison.OrdinalIgnoreCase) &&
                                    string.Equals(author, each.Author, StringComparison.OrdinalIgnoreCase) &&
                                    year == each.Year)
                                {
                                    Console.WriteLine($"{each.Title} is already in the catalog!");
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                book.Add(newBook);

                                string updatedContent = JsonSerializer.Serialize(book, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                                File.WriteAllText(filepath, updatedContent);

                                Console.WriteLine("Book was added successfully!");
                            }
                            break;
                        }

                    case 4: { Console.WriteLine("You exited the Menu.");  return;  }

                    default: Console.WriteLine("Invalid option. Please select a valid menu item."); break;
                }
            }
            else { Console.WriteLine("Invalid input. Please enter a number between 1 and 4."); }
        }
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
