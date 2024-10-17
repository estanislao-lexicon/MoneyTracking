# Money Tracking

### Project Brief 

Your task is to build a money tracking application. The application will allow a user to enter expense(s) and income(s) to the application. Further, they should be able to assign a month to an expense or income. They will interact with a text based user interface via the command-line. 
Once they are using the application, the user should be able to also edit and remove items from the application. They can also quit and save the current task list to file, and then restart the application with the former state restored. The interface should look similar to the mock up below: 

> ```csharp
> Welcome to TrackMoney
> You have currently (-) X kr on your account.
> Pick an option:
> (1) Show items (All/Expense(s)/Income(s))
> (2) Add New Expense/Income
> (3) Edit Item (edit, remove)
> (4) Save and Quit
> ```

#### Requirements 
The solution must achieve the following requirements: 

- [X] Model an item with title, amount, and month. 
	- [X] Solve the problem where you have to distinguish income and expense.
- [X] Display a collection of items that can be sorted in ascending or descending order. 
	- [X] Sorted by month, amount or title. 
	- [X] Display only expenses or only incomes. 
- [X] Support the ability to edit, and remove items 
- [X] Support a text-based user interface 
- [X] Load and save items list to file 

The solution may also include other creative features at your discretion in case you wish to show some flair.


#### Solution

- UML diagram: https://app.smartdraw.com/editor.aspx?credID=-69326817&depoId=60759559&flags=128
