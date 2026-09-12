using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// b. Marker interface
public interface IInventoryEntity
{
    int Id { get; }
}

// a. Immutable InventoryItem record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// c. Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return _log;
    }

    public void SaveToFile()
    {
        try
        {
            using (var writer = new StreamWriter(_filePath))
            {
                string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            using (var reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                if (items != null)
                {
                    _log = items;
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: File '{_filePath}' was not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

// f. InventoryApp
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Monitor", 15, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Keyboard", 30, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Mouse", 40, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Webcam", 12, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:d}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string filePath = "inventory.json";

        // First "session" - seed and save
        InventoryApp app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();
        Console.WriteLine("Data seeded and saved to file.");

        // Simulate a new session by creating a fresh InventoryApp instance
        Console.WriteLine("\n=== Simulating new session ===");
        InventoryApp newSessionApp = new InventoryApp(filePath);
        newSessionApp.LoadData();

        Console.WriteLine("\n=== Loaded Inventory Items ===");
        newSessionApp.PrintAllItems();
    }
}