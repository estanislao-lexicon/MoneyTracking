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
            string manuTitle = $"Welcome to TrackMoney\nYou have currently {Savings.GetSavings()} kr on your account.";
            
            List<string> mainMenu = new List<string>
                {
                    "Show items (All/Expense(s)/Income(s))",
                    "Add New Expense/Income", 
                    "Edit Item (edit, remove)", 
                    "Save & Quit"
                };

            int input = 0;
            bool runMainMenu = true;
            while(runMainMenu)
            {
                input = PrintList(mainMenu, manuTitle);
                runMainMenu = false;
            }

            switch (input)
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
        bool runShowMovementsMenu = true;
        while(runShowMovementsMenu)
        {
            List<string> showMovementsMenu = new List<string>
            {
                "Print all Movements",
                "Print Income(s)",
                "Print Expense(s)",
                "Return to Main Menu"
            };

            int input = 0;
            bool runShowMenu = true;
            while(runShowMenu)
            {
                input = PrintList(showMovementsMenu);
                runShowMenu = false;
            }

            switch (input)
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
                    runShowMovementsMenu = false;                    
                    break;                
            }
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
            System.Console.WriteLine("Press initial letter to sort by: (M)onth, (A)mount, or (T)itle");
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
                        movements = movements.OrderBy(m => m.GetDate().Month).ToList();
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
        int input = 0;
        bool runAddMenu = true;
        while(runAddMenu)
        {
            List<string> addMovementList = new List<string>
            {
                "Add Income",
                "Add Expense",
                "Return"
            };
            input = PrintList(addMovementList);
            runAddMenu = false;
        }
        if (input == 2)
        {
            return;
        }        
        
        string title = GetInput();
        double amount = GetValidAmount();
        DateTime date = GetValidDate();
        
        if (input == 0)
        {
            movementsList.Add(new Income(title, amount, date));
        }
        else if (input == 1)
        {
            movementsList.Add(new Expense(title, -amount, date));
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

    static void EditOrRemoveMovement(List<Movements> movementsList)
    {
        int movementInput = 0;
        movementInput = PrintList(movementsList);
        
        string movementTitle = movementsList[movementInput].GetTitle() + " " + movementsList[movementInput].GetAmount() + " " + movementsList[movementInput].GetDate().ToString("dd-MM-yyyy");
        List<string> editOrRemove = new List<string> {"Edit", "Remove", "Return"};

        int editOrRemoveInput = 0;
        editOrRemoveInput = PrintList(editOrRemove, movementTitle);

        switch (editOrRemoveInput)
        {
            case 0:
                EditMovement(movementsList, movementInput, movementTitle);
                break;
            case 1:
                movementsList.Remove(movementsList[movementInput]);
                break;
            case 2:                
                break;
        }
    }

    static void EditMovement(List<Movements> movementsList, int index, string movementTitle)
    {
        int editMovementInput = 0;
        List<string> editMovementAlternatives = new List<string>{"Title", "Amount", "Date", "Return"};
        editMovementInput = PrintList(editMovementAlternatives, movementTitle);
        switch(editMovementInput)
        {
            case 0:
                string title = GetInput();
                movementsList[index].EditTitle(title);
                break;
            case 1:
                double amount = GetValidAmount();
                movementsList[index].EditAmount(amount);
                break;
            case 2:
                DateTime date = GetValidDate();
                movementsList[index].EditDate(date);
                break;
            case 3:
                break;
        }                
    }

    static int PrintList<T>(List<T> list, string title = null)
    {                
        int selectedIndex = 0;
        bool done = false;

        ConsoleKey key;
        while (!done)
        {
            // Clear the console and print the updated list with the selected item highlighted
            Console.Clear();
            System.Console.WriteLine(title + "\n");
            RollingList(list, selectedIndex);

            // Wait for user input and process it
            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? list.Count - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == list.Count - 1) ? 0 : selectedIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    Console.Clear();                    
                    done = true;                    
                    break;
                case ConsoleKey.Escape:
                    selectedIndex = -1;
                    done = true;
                    break;
            }            
        }    
        return selectedIndex;    
    }

    static void RollingList<T>(List<T> list, int selectedIndex)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i == selectedIndex)
            {
                // Highlight the selected item
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;
                if(list[i] is Movements movement)
                {
                    movement.Print();
                }                                                
                else if(list[i] is string str)
                {
                    System.Console.WriteLine(str);
                }                
                else
                {
                    System.Console.WriteLine(list[i]?.ToString());
                }
                Console.ResetColor();
            }
            else
                {
                    // Print normal item
                if (list[i] is Movements movement)
                {
                    movement.Print();
                }
                else if (list[i] is string str)
                {
                    Console.WriteLine(str);
                }
                else
                {
                    Console.WriteLine(list[i]?.ToString());
                }                
            }
        }
        System.Console.WriteLine("\nUse arrows to move Up and Down. Press Enter for options or Esc to return");
    }
}
