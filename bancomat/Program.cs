using OfficeOpenXml;
using System;
using System.IO;

public class Program
{
    public static void Main()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string firstFileName = "account1.txt";
        string secondFileName = "account2.txt";
        string directoryPath = "C:\\Users\\us store\\Desktop\\examWorks\\bancomat\\";

        RefreshValue file = new RefreshValue();
        //Directory.CreateDirectory(directoryPath);

        //ვამოწმებთ ფაილი შექმნილია თუ არა. თუ არ არის, ქმნის მათ
        if (!File.Exists(Path.Combine(directoryPath, firstFileName))) { file.FileCreation(directoryPath, firstFileName, 1000);  }
        if (!File.Exists(Path.Combine(directoryPath, secondFileName))) { file.FileCreation(directoryPath, secondFileName, 1000); }
        
        //შევქმენით ექსელის ფაილი შესაბამისი გრაფებით
        string transactionList = Path.Combine(directoryPath, "transactions.xlsx");
        using (var package = new ExcelPackage(new FileInfo(transactionList)))
        {
            var worksheet = package.Workbook.Worksheets.Count > 0 ? package.Workbook.Worksheets[0] : package.Workbook.Worksheets.Add("Transactions");
            if (worksheet.Dimension == null)
            {
                worksheet.Cells[1, 1].Value = "Date";
                worksheet.Cells[1, 2].Value = "Transactions";
                worksheet.Cells[1, 3].Value = "First Account";
                worksheet.Cells[1, 4].Value = "Second Account";
                package.Save();
            }
        }

     
        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Check balance");
            Console.WriteLine("2. Deposit money into the first account");
            Console.WriteLine("3. Withdraw money from the first account");
            Console.WriteLine("4. Transfer money between accounts");
            Console.WriteLine("5. View transaction history");
            Console.WriteLine("6. Exit");
            Console.Write("\nSelect a service: ");

            //თუ იუზერი შეიყვანს არარიცხვით მონაცემს, მოვთხოვთ სწორი მონაცემის შეყვანას
            if (!int.TryParse(Console.ReadLine(), out int selectedOperation) && selectedOperation>6 && selectedOperation < 1) 
            {Console.WriteLine("Invalid input. Please the correct number."); continue; }

            switch (selectedOperation)
            {
                case 1:   //ვამოწმებთ ორივე ანგარიშის მონაცემებს
                    Console.WriteLine($"First Account Balance: {file.FileReader(directoryPath, firstFileName)} GEL");
                    Console.WriteLine($"Second Account Balance: {file.FileReader(directoryPath, secondFileName)} GEL");
                    break;

                case 2: 
                    Console.Write("\nEnter desired amount: ");  //ვამოწმებთ გადარიცხული თანხა რომ მეტი იყოს ბალანსზე და შეყვანილი თანხა უნდა იყოს 0-ზე მეტი
                    if (int.TryParse(Console.ReadLine(), out int depositAmount) && depositAmount > 0)
                    {
                        //ვაახლებთ პირველ ექაუნთს
                        int newFirstAccountBalance = file.UpdateBalance(directoryPath, firstFileName, depositAmount);
                        //ვკითხულობთ მეორე ექაუნთს
                        int secondAccountBalance = file.FileReader(directoryPath, secondFileName);
                        //გადაგვაქვს ექსელის ფაილში შესაბამისი მონაცემები
                        file.LogTransaction(transactionList, depositAmount, newFirstAccountBalance, secondAccountBalance);
                        Console.WriteLine($"Successful transaction");
                    } else { Console.WriteLine("Invalid input!"); }  break;

                case 3:
                    Console.Write("\nEnter amount to withdraw: ");

                    //ვამოწმებთ შეყვანილი თანხა თუ მეტია 0-ზე
                    if (int.TryParse(Console.ReadLine(), out int withdrawAmount) && withdrawAmount > 0)
                    {
                        int firstAccountBalance = file.FileReader(directoryPath, firstFileName);
                        //ვამოწმებთ ანგარიშზე არის თუ არა საკმარისი თანხა
                        if (firstAccountBalance >= withdrawAmount)
                        {
                            //ვააფდეითებთ პირველ ექაუნთს
                            int newFirstAccountBalance = file.UpdateBalance(directoryPath, firstFileName, -withdrawAmount);
                            //ვკითხულობთ მეორე ექაუნთს
                            int secondAccountBalance = file.FileReader(directoryPath, secondFileName);
                            //ავსახავთ ექსელის ფაილში მონაცემებს
                            file.LogTransaction(transactionList, -withdrawAmount, newFirstAccountBalance, secondAccountBalance);
                            Console.WriteLine($"Successful withdrewal!");
                        } else {  Console.WriteLine("Insufficient funds in the first account."); }
                    } else {  Console.WriteLine("Invalid withdrawal amount."); } break;

                case 4:
                    Console.Write("\nEnter amount to transfer: ");
                    if (int.TryParse(Console.ReadLine(), out int transferAmount) && transferAmount > 0)
                    {
                        int firstAccountBalance = file.FileReader(directoryPath, firstFileName);
                        //მოწმდება არის თუ არა ბალანსზე საკმარისი თანხა
                        if (firstAccountBalance >= transferAmount)
                        {
                            //ვაახლებთ ორივე ანგარიშს და გადაგვაქვს მონაცემები ექსელში
                            int newFirstAccountBalance = file.UpdateBalance(directoryPath, firstFileName, -transferAmount);
                            int newSecondAccountBalance = file.UpdateBalance(directoryPath, secondFileName, transferAmount);
                            file.LogTransaction(transactionList, -transferAmount, newFirstAccountBalance, newSecondAccountBalance);
                            Console.WriteLine($"Successful transfer!");
                        } else  {  Console.WriteLine("Insufficient funds in the first account."); }
                    } else {  Console.WriteLine("Invalid input"); }  break;
                    //ექსელიდან ვკითხულობთ მონაცემებს
                case 5: Console.WriteLine("\nTransaction History:"); file.ReadExcelFile(transactionList); break;
                case 6: Console.WriteLine("Goodbye!"); return;
                default: Console.WriteLine("Invalid option. Please select a valid menu item."); break;
            }
        }
    }



    public class RefreshValue   //კლასი სადაც შენახულია მეთოდები
    {
        //მეთოდი ფაილების შექმნისთვის
        public void FileCreation(string directory, string fileName, int initialBalance)
        {
            string filePath = Path.Combine(directory, fileName);
            using (StreamWriter writer = new StreamWriter(filePath, false))  { writer.Write(initialBalance); }
        }
        //მეთოდი ფაილების წაკითხვისთვის
        public int FileReader(string directory, string fileName)
        {
            string filePath = Path.Combine(directory, fileName);
            using (StreamReader reader = new StreamReader(filePath))
            {
                string content = reader.ReadToEnd();
                return int.TryParse(content, out int balance) ? balance : throw new InvalidDataException("Invalid data in account file.");
            }
        }
        //მეთოდი ფაილებში ბალანსის განახლებისთვის
        public int UpdateBalance(string directory, string fileName, int amount)
        {
            string filePath = Path.Combine(directory, fileName);
            int currentBalance = FileReader(directory, fileName);
            int newBalance = currentBalance + amount;

            using (StreamWriter writer = new StreamWriter(filePath, false)) {  writer.Write(newBalance); }
            return newBalance;
        }

        //მეთოდი ექსელში მონაცემების გადატანისთვის
        public void LogTransaction(string transactionList, int transactionAmount, int firstAccountBalance, int secondAccountBalance)
        {
            FileInfo fileInfo = new FileInfo(transactionList);
            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int nextRow = worksheet.Dimension?.Rows + 1 ?? 2;

                worksheet.Cells[nextRow, 1].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[nextRow, 2].Value = transactionAmount;
                worksheet.Cells[nextRow, 3].Value = firstAccountBalance;
                worksheet.Cells[nextRow, 4].Value = secondAccountBalance;
                package.Save();
            }
        }

        //მეთოდი ექსელიდან მონაცემების წაკითხვისთვის, case:5- ბრძანებისთვის.
        public void ReadExcelFile(string transactionList)
        {
            FileInfo fileInfo = new FileInfo(transactionList);
            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet.Dimension != null)
                {
                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        string date = worksheet.Cells[row, 1].Text;
                        string transactionAmount = worksheet.Cells[row, 2].Text;
                        string firstAccountBalance = worksheet.Cells[row, 3].Text;
                        string secondAccountBalance = worksheet.Cells[row, 4].Text;
                        Console.WriteLine($"Date: {date}, Transaction: {transactionAmount}, Balance I: {firstAccountBalance}, Balance II: {secondAccountBalance}");
                    }
                } else { Console.WriteLine("No transaction history available."); }
            }
        }
    }
}
