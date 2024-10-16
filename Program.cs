using MoneyTracking.Models;
using MoneyTracking.Save;
using MoneyTracking.Data;

class Program
{
    static void Main(string[] args)
    {
        List<Movements> movementsList = MovementsStorage.LoadMovementsFromFile();
       
        bool run = true;
        while(run)
        {
            Console.Clear();
            PrintMainMenu();

            System.Console.Write(">");
            string input = Console.ReadLine();

            if(input == "1")
            {
                PrintList(movementsList);
            }
            else if(input == "2")
            {
                AddMovement(movementsList);
            }
            else if(input == "3")
            {
                // EditItem();
                System.Console.WriteLine("Input 3");
            }
            else if(input == "4")
            {
                MovementsStorage.SaveMovementsToFile(movementsList);                
                run = false;
            }
            else
            {
                System.Console.WriteLine("Please enter a valid input");
                Console.ReadKey(true);
            }
        }
    }

    static void PrintMainMenu()
    {
        //Console.Clear();
        System.Console.WriteLine($"Welcome to TrackMoney\nYou have currently {Savings.GetSavings()} kr on your account.");
        System.Console.WriteLine("Choose one of the following options:");
        System.Console.WriteLine("(1) Show items (All/Expense(s)/Income(s))\n(2) Add New Expense/Income\n(3) Edit Item (edit, remove)\n(4) Save & Quit");        
    }

    static void PrintList(List<Movements> movementsList)
    {
        bool runPrintList = true;
        while(runPrintList == true)
        {
            Console.Clear();
            System.Console.WriteLine("(1) Print all Movements\n(2) Print Expense(s)\n(3) Print Income(s)\n(4) Return to Main Menu");

            if (!movementsList.Any())
            {
                Console.WriteLine("\nNo Incomes or Expenses in the list\n");
            }

            System.Console.Write(">");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    PrintMovements(movementsList.Cast<Movements>().ToList());
                    break;
                case "2":
                    PrintMovements(movementsList.Cast<Movements>().ToList());
                    break;
                case "3":
                    PrintMovements(movementsList.Cast<Movements>().ToList());
                    break;
                case "4":
                    runPrintList = false;                    
                    break;
                default:
                    System.Console.WriteLine("Please enter a valid input");
                    Console.ReadKey(true);
                    break;
            }
        }
    }

    static void PrintMovements(List<Movements> movements)
    {
        Console.Clear();
        movements.ForEach(m => m.Print());

        System.Console.WriteLine("\nPress A for ascending or D for descending order");
        System.Console.WriteLine("Press initial letter to sort by: (M)onth, (A)mount, or (T)itle");
        System.Console.WriteLine("Or press any key to return");
        System.Console.Write(">");

        string sortingInput = Console.ReadLine().ToUpper();
        if (sortingInput != null)
        {
            switch (sortingInput)
            {
                case "A":
                    movements = movements.OrderBy(m => m.GetAmount()).ToList();
                    break;
                case "D":
                    movements = movements.OrderByDescending(m => m.GetAmount()).ToList();
                    break;
                case "M":
                    movements = movements.OrderBy(m => m.GetDate()).ToList();
                    break;
                case "T":
                    movements = movements.OrderBy(m => m.GetTitle()).ToList();
                    break;
                default:
                    System.Console.WriteLine("Invalid sorting option. Displaying unsorted list.");
                    break;
            }

            // Print sorted movements            
            movements.ForEach(m => m.Print());
        }
    }    
 
    static void AddMovement(List<Movements> movementsList)
    {
        Console.Clear();
        System.Console.WriteLine("Chose and option:\n(1) Add Income\n(2) Add Expense\nPress any key to return");
        System.Console.Write(">");
        
        string optionInput = Console.ReadLine();
        if (optionInput != "1" && optionInput != "2")
        {
            return;
        }   
        
        string title = GetInput();
        double amount = GetValidAmount();
        DateTime date = GetValidDate();
        
        if (optionInput == "1")
        {
            movementsList.Add(new Income(title, amount, date));
        }
        else if (optionInput == "2")
        {
            movementsList.Add(new Expense(title, amount, date));
        }
    }

    static string GetInput()
    {
        Console.Write("Title: ");
        string input = Console.ReadLine();
        return Capitalize(input);        
    }

    static double GetValidAmount()
    {
        double amount;
        while (true)
        {
            Console.Write("Amount: ");
            if (double.TryParse(Console.ReadLine(), out amount))
            {
                return amount;
            }
            Console.WriteLine("Invalid amount. Please enter a valid number.");
        }
    }

    static DateTime GetValidDate()
    {
        DateTime date;
        while (true)
        {
            Console.Write("Date (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out date))
            {
                return date;
            }
            Console.WriteLine("Invalid date. Please enter a valid date.");
        }
    }

    static string Capitalize(string textToCapitalize)
    {
        if (string.IsNullOrEmpty(textToCapitalize)) return textToCapitalize;
        return char.ToUpper(textToCapitalize[0]) + textToCapitalize.Substring(1).ToLower();
    }
}
