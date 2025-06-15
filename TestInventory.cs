using System;
using GBDMS.Data;

namespace GBDMS
{
    public class TestInventory
    {
        public static void TestInventoryModel()
        {
            // Test creating a new inventory item with district
            var item = new InventoryItem
            {
                ItemCode = "GI-F001",
                Name = "Test Rice",
                Category = "Food",
                District = "Gilgit",
                Quantity = 100,
                Unit = "bags",
                MinimumLevel = 50,
                LastUpdated = DateTime.Now
            };

            Console.WriteLine($"Created inventory item: {item.Name} in {item.District}");
            Console.WriteLine($"Item Code: {item.ItemCode}");
            Console.WriteLine($"Category: {item.Category}");
            Console.WriteLine($"Quantity: {item.Quantity} {item.Unit}");
        }
    }
}
