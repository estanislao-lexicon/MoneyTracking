using System.Dynamic;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace MoneyTracking;

static class Savings
{
    private static double savings = 0;
    private static string filePath = "savings.txt";

    public static double GetSavings()
    {
        return savings;
    }

    public static void LoadSavings()
    {
        if(File.Exists(filePath))
        {
            string saveData = File.ReadAllText(filePath);
            if(double.TryParse(saveData, out double loadedSavings))
            {
                savings = loadedSavings;                
            }
        }
    }

    public static void SaveSavings()
    {
        File.WriteAllText(filePath, savings.ToString());
    }
    public static void IncrementSavings(double value)
    {
        savings = savings + value;
        SaveSavings();
    }

    public static void ReduceSavings(double value)
    {
        savings = savings - value;
        SaveSavings();
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Expense> expensesList = MoneyMovementStorage.LoadExpensesFromFile();
        List<Income> incomesList = MoneyMovementStorage.LoadIncomesFromFile();

        bool run = true;
        while(run == true)
        {
            PrintMainMenu();

            System.Console.Write(">");
            string input = Console.ReadLine();

            if(input == "1")
            {
                if(!incomesList.Any() && !expensesList.Any())
                {
                    Console.WriteLine("No Incomes and Expenses in the list\n");                                
                }
                else
                {
                    PrintList(incomesList, expensesList);
                }
                
            }
            else if(input == "2")
            {
                // AddItem();
                System.Console.WriteLine("Input 2");
            }
            else if(input == "3")
            {
                // EditItem();
                System.Console.WriteLine("Input 3");
            }
            else if(input == "4")
            {
                MoneyMovementStorage.SaveIncomesToFile(incomesList);
                MoneyMovementStorage.SaveExpensesToFile(expensesList);
                run = false;
            }
            else
            {
                System.Console.WriteLine("Please enter a valid input");
            }
        }
    }

    static void PrintMainMenu()
    {
        System.Console.WriteLine($"Welcome to TrackMoney\n You have currently {Savings.GetSavings()} kr on your account.");
        System.Console.WriteLine("Choose one of the following options:");
        System.Console.WriteLine("(1) Show items (All/Expense(s)/Income(s))\n(2) Add New Expense/Income\n(3) Edit Item (edit, remove)\n(4) Save & Quit");
    }

    static void PrintList(List<Income> incomesList, List<Expense> expensesList)
    {
        foreach(Income income in incomesList)
        {
            income.Print();
        }
        foreach(Expense expense in expensesList)
        {
            expense.Print();
        }
    }
}

abstract class MoneyMovement
{
    private string Title { get; set; }    
    private double Amount { get; set; }
    private DateTime Date { get; set; }        
    public MoneyMovement(string title, double amount, DateTime date)
    {
        Title = title;        
        Amount = amount;
        Date = date;        
    }   

    public string GetTitle() => Title;
    public double GetAmount() => Amount;
    public DateTime GetDate() => Date;

    public void EditTitle(string title)
    {        
        Title = title;
    }    

    public void EditAmount(double ammount)
    {        
        Amount = ammount;
    }    

    public void EditDate (DateTime date)
    {        
        Date = date;
    }    

    public void Print()
    {
        System.Console.WriteLine(Title + " " + Amount + "kr " + Date);
    }
}

class MoneyMovementStorage
{
    private static string incomesFilePath = "incomes.json";
    private static string expensesFilePath = "expenses.json";
    
    public static void SaveIncomesToFile(List<Income> incomesList) 
    {
        var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new MoneyMovementConverter() } };
        string json = JsonSerializer.Serialize(incomesList, options);
        File.WriteAllText(incomesFilePath, json);
    }   

    public static void SaveExpensesToFile(List<Expense> expensesList) 
    {
        var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new MoneyMovementConverter() } };
        string json = JsonSerializer.Serialize(expensesList, options);
        File.WriteAllText(expensesFilePath, json);
    }        

    public static List<Income> LoadIncomesFromFile()
    {
        if(File.Exists(incomesFilePath))
        {
            string json = File.ReadAllText(incomesFilePath);
            if(string.IsNullOrWhiteSpace(json))
            {
                return new List<Income>();
            }

            try
            {
                var options = new JsonSerializerOptions { Converters = { new MoneyMovementConverter() } };
                return JsonSerializer.Deserialize<List<Income>>(json, options);
            }
            catch (JsonException ex)
            {
                System.Console.WriteLine($"Error deserializing the file: {ex.Message}");
                return new List<Income>();
            }
        }
        return new List<Income>();
    }

    public static List<Expense> LoadExpensesFromFile()
    {
        if(File.Exists(expensesFilePath))
        {
            string json = File.ReadAllText(expensesFilePath);
            if(string.IsNullOrWhiteSpace(json))
            {
                return new List<Expense>();
            }

            try
            {
                var options = new JsonSerializerOptions { Converters = { new MoneyMovementConverter() } };
                return JsonSerializer.Deserialize<List<Expense>>(json, options);
            }
            catch (JsonException ex)
            {
                System.Console.WriteLine($"Error deserializing the file: {ex.Message}");
                return new List<Expense>();
            }
        }
        return new List<Expense>();
    }
}

class MoneyMovementConverter : JsonConverter<MoneyMovement>
{
    public override MoneyMovement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var jsonObject = doc.RootElement;
            string type = jsonObject.GetProperty("Type").GetString();
            string title = jsonObject.GetProperty("Title").GetString();
            double amount = jsonObject.GetProperty("Amount").GetDouble();
            DateTime date = jsonObject.GetProperty("Date").GetDateTime();

            if (type == nameof(Income))
                return new Income(title, amount, date);
            else if (type == nameof(Expense))
                return new Expense(title, amount, date);

            throw new NotSupportedException($"Unsupported type: {type}");
        }
    }

    public override void Write(Utf8JsonWriter writer, MoneyMovement value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().Name);
        writer.WriteString("Title", value.GetTitle());
        writer.WriteNumber("Amount", value.GetAmount());
        writer.WriteString("Date", value.GetDate());
        writer.WriteEndObject();
    }
}

class Income : MoneyMovement
{
    public Income(string title, double amount, DateTime date) : base(title, amount, date)
    {
        Savings.IncrementSavings(amount);
    }    
}

class Expense : MoneyMovement
{
    public Expense(string title, double amount, DateTime date) : base(title, amount, date)
    {
        Savings.ReduceSavings(amount);        
    }

}
