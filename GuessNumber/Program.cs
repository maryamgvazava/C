using System;

class Program
{
    static void Main()
    {
        Random random = new Random();
        int secretNumber = random.Next(1, 21);
        int score = 20;

        Console.WriteLine("Welcome to the Number Guessing Game!");
        Console.WriteLine("Guess the secret number between 1 and 20.");
        Console.WriteLine("Type 'exit' to quit the game at any time.");

        while (true)
        {
            Console.WriteLine("\nEnter your guess: "); string input = Console.ReadLine();
            //თუ ავკრეფთ "exit"-ს, აღარ გავაგრძელებთ თამაშს
            if (input.ToLower() == "exit") {Console.WriteLine("Game exited. Thank you for playing!"); break;}
            //
            if (!int.TryParse(input, out int guess) || guess < 1 || guess > 20){ Console.WriteLine("Please enter a valid number between 1 and 20.");continue;}
            //თუ შეყვანილი რიცხვი საიდუმლო რიცხვის ტოლია, უნდა გამოვიდეს გამარჯვების შეტყობინება
            if (guess == secretNumber){ Console.WriteLine("Correct! You guessed the secret number!"); Console.WriteLine($"Your score: {score}");
                Console.WriteLine("\nDo you want to play again? (yes/no)");
                string playAgain = Console.ReadLine().ToLower();
                // თუ დავთანხმდებით თამაშის თავიდან დაწყებას... 
                if (playAgain == "yes"){secretNumber = random.Next(1, 21); score = 20; Console.WriteLine("\nGame reset! Start guessing again."); }
                    //ან...
                    else { Console.WriteLine("Thank you for playing! Goodbye."); break;}
            } else { score--;   //თუ საიდუმლო რიცხვს ვერ გამოვიცნობთ
                    
                    if (score > 0) {Console.WriteLine(guess > secretNumber ? "Too high! Try again.": "Too low! Try again.");Console.WriteLine($"Your score: {score}");}
                        else{
                            Console.WriteLine("You lost the game! The secret number was " + secretNumber);
                            Console.WriteLine("\nDo you want to play again? (yes/no)");
                            string playAgain = Console.ReadLine().ToLower();
                    
                            if (playAgain == "yes"){ secretNumber = random.Next(1, 21); score = 20; Console.WriteLine("\nGame reset! Start guessing again."); }
                                else { Console.WriteLine("Thank you for playing! Goodbye.");  break; }
                                }
                        }
                    }
            }
        }