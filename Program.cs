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
            string mainTitle = $"Welcome to TrackMoney\nYou have currently {Savings.GetSavings()} kr on your account.";
            
            List<string> mainOptions = new List<string>
                {
                    "Show items (All/Expense(s)/Income(s))",
                    "Add New Expense/Income", 
                    "Edit Item (edit, remove)", 
                    "Save & Quit"
                };

            Menu mainMenu = new Menu(mainTitle, mainOptions);
            int choice = mainMenu.Display();

            switch (choice)
            {
                case 0:
                    ShowMovementsAlternatives(movementsList);
                    break;
                case 1:
                    AddMovement(movementsList);
                    break;
                case 2:
                    EditOrRemoveMovement(movementsList);
                    break;
                case 3:
                    MovementsStorage.SaveMovementsToFile(movementsList);
                    run = false;
                    break;
            }
        }
    }

    static void ShowMovementsAlternatives(List<Movements> movementsList)
    {
        string title = "Show Movements Menu";
        List<string> options = new List<string>
        {
            "Print all Movements",
            "Print Income(s)",
            "Print Expense(s)",
            "Return to Main Menu"
        };

        Menu showMenu = new Menu(title, options);
        int choice = showMenu.Display();

        switch (choice)
        {
            case 0:
                PrintMovementsList(movementsList.Cast<Movements>().ToList());
                break;
            case 1:
                List<Movements> incomeList = movementsList.OfType<Income>().Cast<Movements>().ToList();
                PrintMovementsList(incomeList);
                break;
            case 2:
                List<Movements> expenseList = movementsList.OfType<Expense>().Cast<Movements>().ToList();
                PrintMovementsList(expenseList);
                break;
            case 3:
                return;                
        }        
    }

    static void PrintMovementsList(List<Movements> movements)
    {
        Console.Clear();
        movements.ForEach(m => m.Print());

        bool runSorting = true;
        while(runSorting)
        {
            System.Console.WriteLine("\nPress A for ascending or D for descending order");
            System.Console.WriteLine("Press initial letter to sort by: (M)onth, or (T)itle");
            System.Console.WriteLine("Or press any key to return");
            System.Console.Write(">");

            string sortingInput = Console.ReadLine().ToUpper();
            System.Console.WriteLine(sortingInput);
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
                        runSorting = false;
                        break;
                }

                // Print sorted movements
                Console.Clear();         
                movements.ForEach(m => m.Print());
            }
        }
    }    
 
    static void AddMovement(List<Movements> movementsList)
    {
        List<string> options = new List<string> { "Add Income", "Add Expense", "Return" };
        Menu addMovementMenu = new Menu("Select Movement Type", options);
        int input = addMovementMenu.Display();
        
        if (input == 2) return;
                
        string title = GetInput();
        double amount = GetValidAmount();
        DateTime date = GetValidDate();
        
        movementsList.Add(input == 0 ? new Income(title, amount, date) : new Expense(title, amount, date));
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

    static void EditOrRemoveMovement(List<Movements> movementsList)
    {
        List<string> movementTitles = movementsList
        .Select(m => $"{m.GetTitle()} {m.GetAmount()}kr {m.GetDate():dd-MM-yyyy}")
        .ToList();
        
        Menu movementMenu = new Menu("Select a Movement", movementTitles);
        int movementInput = movementMenu.Display();
        string movementTitle = movementTitles[movementInput];

        List<string> editOrRemove = new List<string> { "Edit", "Remove", "Return" };
        Menu editOrRemoveMenu = new Menu($"Selected Movement: {movementTitle}", editOrRemove);

        int editOrRemoveInput = editOrRemoveMenu.Display();        

        switch (editOrRemoveInput)
        {
            case 0:
                EditMovement(movementsList, movementInput, movementTitle);
                break;
            case 1:
                if (movementsList[movementInput] is Income income)
                {
                    income.DeleteIncome();
                    movementsList.RemoveAt(movementInput);
                }
                else if (movementsList[movementInput] is Expense expense)
                {
                    expense.DeleteExpense();
                    movementsList.RemoveAt(movementInput);
                }
                break;
            case 2:                
                break;
        }
    }

    static void EditMovement(List<Movements> movementsList, int index, string movementTitle)
    {        
        List<string> editMovementAlternatives = new List<string>{"Title", "Amount", "Date", "Return"};
        Menu editMenu = new Menu($"Edit Movement: {movementTitle}", editMovementAlternatives);
        int editMovementInput = editMenu.Display();

        switch(editMovementInput)
        {
            case 0:
                string title = GetInput();
                movementsList[index].EditTitle(title);
                break;
            case 1:
                double amount = GetValidAmount();
                movementsList[index].EditAmount(amount);
                if (movementsList[index] is Income income)
                {
                    income.UpdateIncomeAmount(amount);                    
                }
                else if (movementsList[index] is Expense expense)
                {
                    expense.UpdateExpenseAmount(amount);                    
                }                                                  
                break;
            case 2:
                DateTime date = GetValidDate();
                movementsList[index].EditDate(date);
                break;
            case 3:
                break;
        }                
    }
}

class Menu
{
    public string Title { get; set; }
    public List<string> Options { get; set; }

    public Menu(string title, List<string> options)
    {
        Title = title;
        Options = options;
    }

    public int Display()
    {
        int selectedIndex = 0;
        bool done = false;
        ConsoleKey key;

        while (!done)
        {
            Console.Clear();
            Console.WriteLine(Title + "\n");
            RollingList(Options, selectedIndex);

            key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? Options.Count - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == Options.Count - 1) ? 0 : selectedIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    done = true;
                    break;
            }
        }
        return selectedIndex;
    }

    private void RollingList(List<string> list, int selectedIndex)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(list[i]);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(list[i]);
            }
        }
        Console.WriteLine("\nUse arrows to navigate, Enter to select.");
    }
}
