using MoneyTracking.Save;

namespace MoneyTracking.Models
{
    abstract class Movements
    {
        private string Title { get; set; }    
        private double Amount { get; set; }
        private DateTime Date { get; set; }        
        protected Movements(string title, double amount, DateTime date)
        {
            Title = title;        
            Amount = amount;
            Date = date;
        }   

        protected Movements() {}
        public string GetTitle() => Title;
        public double GetAmount() => Amount;
        public DateTime GetDate() => Date;

        public void EditTitle(string title)
        {        
            Title = title;
        }    

        public void EditAmount(double ammount)
        {        
            double difference = ammount - Amount;
            Amount = ammount;
            Savings.UpdateSavings(difference);
        }    

        public void EditDate (DateTime date)
        {        
            Date = date;
        }    

        public void Print()
        {
            System.Console.WriteLine($"{Title} {Amount}kr {Date.ToString("dd-MM-yyyy")}");
        }
    }

    class Income : Movements
    {
        public Income(string title, double amount, DateTime date) : base(title, amount, date)
        {
            Savings.UpdateSavings(amount);
        }   

        public Income() {} 

        public void UpdateIncomeAmount(double newAmount)
        {
            // Undo the previous amount
            Savings.UpdateSavings(-GetAmount());

            // Update the amount and add the new amount to savings
            this.EditAmount(Math.Abs(newAmount));
            Savings.UpdateSavings(Math.Abs(newAmount));
        }

        public void DeleteIncome()
        {            
            Savings.UpdateSavings(-GetAmount());
        }
    }

    class Expense : Movements
    {
        public Expense(string title, double amount, DateTime date) : base(title, -Math.Abs(amount), date)
        {            
            Savings.UpdateSavings(-Math.Abs(amount));
        }

        public Expense() {}

        public void UpdateExpenseAmount(double newAmount)
        {
            // Undo the previous amount (which was negative)
            Savings.UpdateSavings(GetAmount());

            // Update the amount and deduct the new amount from savings
            this.EditAmount(-Math.Abs(newAmount));
            Savings.UpdateSavings(-Math.Abs(newAmount));
        }

        public void DeleteExpense()
        {            
            Savings.UpdateSavings(GetAmount());
        }

    }
}
