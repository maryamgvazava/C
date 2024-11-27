using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        //ვალიდაციისთვის ვიყენებთ RegExp-ს
        Regex studentName = new Regex("^[a-zA-Z ]+$");
        Regex regGrade = new Regex("^[A-F]$");

        string filePath = "C:\\Users\\us store\\Desktop\\examWorks\\StudentApp\\students.txt";

        Addstud add = new Addstud();
        RemoveStud remove = new RemoveStud();
        UpdateStud edit = new UpdateStud();

        StudentList[] studs = Array.Empty<StudentList>();

        //ამოწმებს ფაილი თუ არსებობს, და თუ არსებობს, მაშინ კითხულობს ამ ფაილიდან
        if (File.Exists(filePath))
        {
            string readFile = File.ReadAllText(filePath);
            studs = JsonSerializer.Deserialize<StudentList[]>(readFile) ?? Array.Empty<StudentList>();
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("**Menu**");
            Console.WriteLine("1: To see an unordered list of students");
            Console.WriteLine("2: To see a sorted list according to Name");
            Console.WriteLine("3: To see a sorted list according to Roll Number");
            Console.WriteLine("4: To see a sorted list according to Grade");
            Console.WriteLine("5: To add a student to the list");
            Console.WriteLine("6: To remove a student from the list");
            Console.WriteLine("7: Edit Student's Data");
            Console.WriteLine("8: Search for a student");
            Console.WriteLine("9: To exit");
            Console.WriteLine();

            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine().Trim(), out int choice))  //ვამოწმებთ თუ რიცხვი არ არის არჩეული 
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    foreach (var stud in studs)
                        stud.DisplayStudentInfo();  //თუ აირჩევს 1-იანს, ჩატვირთავს სტუდენტების სიას
                    break;

                case 2:
                    var sortByName = studs.OrderBy(x => x.Name);
                    foreach (var stud in sortByName)    //თუ აირჩევს 2-ს, ჩატვირთავს სახელებით დასორტირებული სიას
                        stud.DisplayStudentInfo();
                    break;

                case 3:
                    var sortByRollNumber = studs.OrderBy(x => x.RollNumber);
                    foreach (var stud in sortByRollNumber) //თუ აირჩევს 3-ს, ჩატვირთავს id-ებით დასორტირებული სიას
                        stud.DisplayStudentInfo();
                    break;

                case 4:
                    var sortByGrade = studs.OrderBy(x => x.Grade);
                    foreach (var stud in sortByGrade)  //თუ აირჩევს 4-ს, ჩატვირთავს ქულებით-ებით დასორტირებული სიას
                        stud.DisplayStudentInfo();
                    break;

                case 5:
                    Console.Write("Enter Student Name: ");
                    string addStudentName = Console.ReadLine().Trim();

                    Console.Write("Enter Student Grade (A-F): ");
                    string addStudentGrade = Console.ReadLine().Trim();

                    
                    //5-ის არჩევისას ჯერ მოწმდება სტუდენტის სახელი და ქულა არის თუ არა ვალიდური სიმბოლოები
                    if (studentName.IsMatch(addStudentName) && regGrade.IsMatch(addStudentGrade))
                    {
                        int lastRollNumber = studs.Length > 0 ? studs[^1].RollNumber + 1 : 1;

                        StudentList newStudent = new StudentList // თუ სტუდენტი არ არის სიაში, მაშინ ემატება
                        {
                            Name = addStudentName,
                            RollNumber = lastRollNumber,
                            Grade = addStudentGrade
                        };

                        Array.Resize(ref studs, studs.Length + 1);
                        studs[^1] = newStudent;

                        add.Feedback(); break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter valid symbols.");
                    }
                    break;

                case 6: //6-ის არჩევისას, მოწმდება ინდექსის ვალიდაცია და სახელების დამთხვევა (შეყვანილი სახელი თუ იძებნება სიაში)
                    Console.Write("Enter Student Name: ");
                    string removeStudentName = Console.ReadLine().Trim();

                    Console.Write("Enter Student Roll Number: ");
                    if (!int.TryParse(Console.ReadLine().Trim(), out int removeStudentNumber))
                    {
                        Console.WriteLine("Invalid roll number.");
                        break;
                    }

                    bool studentFound = false;
                    studs = studs.Where(stud =>
                    {
                        if (stud.Name.Equals(removeStudentName, StringComparison.OrdinalIgnoreCase) &&
                            stud.RollNumber == removeStudentNumber)
                        {
                            studentFound = true;
                            return false;
                        }
                        return true;
                    }).ToArray();

                    if (studentFound)
                    {
                        remove.Feedback(); break;
                    }
                    else { Console.WriteLine("Student not found."); } break;


                case 7:  //ვეძებთ სტუდენტს სახელით და აიდით. 
                    Console.Write("Enter Student Name: ");
                    string editableStudent = Console.ReadLine().Trim();

                    Console.Write("Enter Student Roll Number: ");
                    if (!int.TryParse(Console.ReadLine().Trim(), out int editableId))
                    {
                        Console.WriteLine("Invalid roll number.");
                        break;
                    }

                    studentFound = false;
                    foreach (var stud in studs)
                    {
                        if (stud.Name.Equals(editableStudent, StringComparison.OrdinalIgnoreCase) && stud.RollNumber == editableId)
                        {
                            studentFound = true;

                            Console.Write("Enter new Name: ");
                            string newName = Console.ReadLine().Trim();

                            //ვამოწმებთ RecExp-ით ვალიდაციას
                            if (!string.IsNullOrWhiteSpace(newName) && studentName.IsMatch(newName))
                                stud.Name = newName;

                            Console.Write("Enter new Grade: ");
                            string newGrade = Console.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(newGrade) && regGrade.IsMatch(newGrade))
                                stud.Grade = newGrade;

                            edit.Feedback(); break;
                        }
                    }

                    if (!studentFound) Console.WriteLine("Student not found."); break;

                case 8:  //სტუდენტის ძებნა არასრული მონაცემებით
                    Console.Write("Enter Student Name: ");
                    string searchStudentByName = Console.ReadLine().Trim();
                    var matchedStudents = await SearchStudentsAsync(studs, searchStudentByName);

                    if (matchedStudents.Any()){ foreach (var student in matchedStudents) student.DisplayStudentInfo(); }
                        else {  Console.WriteLine("No matching students found.");  } break;

                case 9:  Console.WriteLine("You exited the Service!"); return;  //პროგრამიდან გამოსვლა

                default: Console.WriteLine("Invalid choice, please try again."); break;
            }
        }
    }

    public static Task<StudentList[]> SearchStudentsAsync(StudentList[] students, string keyword)
    {
        return Task.Run(() =>
            students
                .Where(s => s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToArray()
        );
    }
}



public class JsonSerializeClass
{
    public void SerializeToFile(string filePath, StudentList[] studs)
    {
        string updatedJson = JsonSerializer.Serialize(studs, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, updatedJson);
    }
    public virtual void Feedback()
    {
        Console.WriteLine("Custom Message");
    }
    
//ინკაფსულაცია - კლასი თავის სხეულში { } ახდენე ზემოთ მოყვანილი 2 მეთოდის "შეფუთვას";
}

public class Addstud : JsonSerializeClass
{
    public override void Feedback()
    {
        Console.WriteLine("Student was added successfully!");
    }
}
public class RemoveStud : JsonSerializeClass
//მემკვიდრეობითობა: Addstud კლასი უღებს JsonSerializeClass-ის მეთოდებს მემკვიდრეობით
{
    public override void Feedback()
    {
        Console.WriteLine("Student was removed successfully!");
    }
}

public class UpdateStud : JsonSerializeClass
{
    public override void Feedback()
    {
        Console.WriteLine("Student details updated successfully!");       
//პოლიმორფიზმი - ვიყენებ JsonSerializeClass-ს მაგრამ ვცვლი კონსოლს
    }
}


public class StudentList
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("rollNumber")]
    public int RollNumber { get; set; }

    [JsonPropertyName("grade")]
    public string Grade { get; set; }

    public void DisplayStudentInfo()
    {
        Console.WriteLine($"Name: {Name}, RollNumber: {RollNumber}, Grade: {Grade}");
    }
}
