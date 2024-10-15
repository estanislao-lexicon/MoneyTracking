using MoneyTracking.Models;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace MoneyTracking.Data
{
    class MovementsStorage
    {
        private static string incomesFilePath = "incomes.json";
        private static string expensesFilePath = "expenses.json";
        
        public static void SaveIncomesToFile(List<Income> incomesList) 
        {
            var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new MovementsConverter() } };
            string json = JsonSerializer.Serialize(incomesList, options);
            File.WriteAllText(incomesFilePath, json);
        }   

        public static void SaveExpensesToFile(List<Expense> expensesList) 
        {
            var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new MovementsConverter() } };
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
                    var options = new JsonSerializerOptions { Converters = { new MovementsConverter() } };
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
                    var options = new JsonSerializerOptions { Converters = { new MovementsConverter() } };
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

    class MovementsConverter : JsonConverter<Movements>
    {
        public override Movements Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        public override void Write(Utf8JsonWriter writer, Movements value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Type", value.GetType().Name);
            writer.WriteString("Title", value.GetTitle());
            writer.WriteNumber("Amount", value.GetAmount());
            writer.WriteString("Date", value.GetDate());
            writer.WriteEndObject();
        }
    }
}
