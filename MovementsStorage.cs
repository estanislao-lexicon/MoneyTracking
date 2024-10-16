using MoneyTracking.Models;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace MoneyTracking.Data
{
    class MovementsStorage
    {
        private static string movementsFilePath = "Movements.json";        
        
        public static void SaveMovementsToFile(List<Movements> movementsList) 
        {
            if (!movementsList.Any())
            {
                return;
            }
            else
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new MovementsConverter() } };
                string json = JsonSerializer.Serialize(movementsList.Cast<Movements>().ToList(), options);
                File.WriteAllText(movementsFilePath, json);
            }
        }   
        public static List<Movements> LoadMovementsFromFile()
        {
            if(File.Exists(movementsFilePath))
            {
                string json = File.ReadAllText(movementsFilePath);
                if(string.IsNullOrWhiteSpace(json))
                {
                    return new List<Movements>();
                }

                try
                {
                    var options = new JsonSerializerOptions { Converters = { new MovementsConverter() } };
                    List<Movements> allMovements = JsonSerializer.Deserialize<List<Movements>>(json, options);
                    return allMovements.OfType<Movements>().ToList();
                    // return JsonSerializer.Deserialize<List<Movements>>(json, options);
                }
                catch (JsonException ex)
                {
                    System.Console.WriteLine($"Error deserializing the file: {ex.Message}");
                    return new List<Movements>();
                }
            }
            return new List<Movements>();
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

                // Manually parse the date using the expected format
                string dateStr = jsonObject.GetProperty("Date").GetString();
                DateTime date = DateTime.ParseExact(dateStr, "dd-MM-yyyy", null);

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
            writer.WriteString("Date", value.GetDate().ToString("dd-MM-yyyy"));
            writer.WriteEndObject();
        }
    }
}
