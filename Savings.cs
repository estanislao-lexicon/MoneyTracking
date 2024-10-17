namespace MoneyTracking.Save
{
    static class Savings
    {
        private static double savings;
        private static string filePath = "Savings.txt";

        public static double GetSavings()
        {
            LoadSavings();
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
        public static void UpdateSavings(double value)
        {
            savings = savings + value;
            SaveSavings();
        }
    }
}
