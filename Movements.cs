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
            Amount = ammount;
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
    }

    class Expense : Movements
    {
        public Expense(string title, double amount, DateTime date) : base(title, amount, date)
        {            
            Savings.UpdateSavings(amount);
        }

        public Expense() {}
    }
}
