using MoneyTracking.Models;
using MoneyTracking.Save;
using MoneyTracking.Data;

class Program
{
    static void Main(string[] args)
    {
        List<Expense> expensesList = MovementsStorage.LoadExpensesFromFile();
        List<Income> incomesList = MovementsStorage.LoadIncomesFromFile();

        bool run = true;
        while(run)
        {
            PrintMainMenu();

            System.Console.Write(">");
            string input = Console.ReadLine();

            if(input == "1")
            {
                PrintList(incomesList, expensesList);
            }
            else if(input == "2")
            {
                AddMovement(incomesList, expensesList);
            }
            else if(input == "3")
            {
                // EditItem();
                System.Console.WriteLine("Input 3");
            }
            else if(input == "4")
            {
                MovementsStorage.SaveIncomesToFile(incomesList);
                MovementsStorage.SaveExpensesToFile(expensesList);
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
        Console.Clear();
        System.Console.WriteLine($"Welcome to TrackMoney\nYou have currently {Savings.GetSavings()} kr on your account.");
        System.Console.WriteLine("Choose one of the following options:");
        System.Console.WriteLine("(1) Show items (All/Expense(s)/Income(s))\n(2) Add New Expense/Income\n(3) Edit Item (edit, remove)\n(4) Save & Quit");
    }

    static void PrintList(List<Income> incomesList, List<Expense> expensesList)
    {
        bool runPrintList = true;
        while(runPrintList == true)
        {
            Console.Clear();
            System.Console.WriteLine("(1) Print all Movements\n(2) Print Expense(s)\n(3) Print Income(s)\n(4) Return to Main Menu");

            if (!incomesList.Any() && !expensesList.Any())
            {
                Console.WriteLine("No Incomes or Expenses in the list\n");
            }

            System.Console.Write(">");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    PrintMovements(incomesList.Cast<Movements>().ToList(), expensesList.Cast<Movements>().ToList());
                    break;
                case "2":
                    PrintMovements(expensesList.Cast<Movements>().ToList());
                    break;
                case "3":
                    PrintMovements(incomesList.Cast<Movements>().ToList());
                    break;
                case "4":
                    runPrintList = false;                    
                    break;
                default:
                    System.Console.WriteLine("Please enter a valid input");
                    break;
            }
        }
    }

    static void PrintMovements(List<Movements> movements)
    {
        movements.ForEach(m => m.Print());

        System.Console.WriteLine("\nPress A for ascending or D for descending order");
        System.Console.WriteLine("Press initial letter to sort by: (M)onth, (A)mount, or (T)itle");
        System.Console.WriteLine("Or press any key to return");
        System.Console.Write(">");

        string sortingInput = Console.ReadLine()?.ToUpper();
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
            Console.Clear();
            movements.ForEach(m => m.Print());
        }
    }

    static void PrintMovements(List<Movements> incomes, List<Movements> expenses)
    {
        List<Movements> allMovements = new List<Movements>();
        allMovements.AddRange(incomes);
        allMovements.AddRange(expenses);
        PrintMovements(allMovements);
    }

    static void AddMovement(List<Income> incomesList, List<Expense> expensesList)
    {
        Console.Clear();
        System.Console.WriteLine("Chose and option:\n(1) Add Expense\n(2) Add Income\nPress any key to return");
        System.Console.Write(">");
        string optionInput = Console.ReadLine();   
        
        bool runAddMovement = true;
        while(runAddMovement)
        {
            if(optionInput == "1" || optionInput == "2")
            {
                System.Console.Write("Title: ");
                string title = Console.ReadLine();

                System.Console.Write("Amount: ");
                double amount = Convert.ToDouble(Console.ReadLine());

                System.Console.Write("Date: ");
                DateTime date = Convert.ToDateTime(Console.ReadLine());

                if(optionInput == "1")
                {
                    expensesList.Add(new Expense(title, amount, date));
                    runAddMovement = false;
                }
                else if(optionInput == "2")
                {
                    incomesList.Add(new Income(title, amount, date));
                    runAddMovement = false;
                }
            }
            else
            {
                runAddMovement = false;
            }
        }

    }
}
