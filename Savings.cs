namespace MoneyTracking.Save
{
    static class Savings
    {
        private static double savings = 0;
        public static double GetSavings()
        {
            return savings;
        }
        
        public static void UpdateSavings(double value)
        {
            savings = savings + value;         
        }
    }
}
