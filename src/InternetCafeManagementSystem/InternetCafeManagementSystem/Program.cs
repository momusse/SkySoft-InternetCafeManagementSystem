using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;

InternetCafeService cafe = new InternetCafeService();

// Preloaded data
cafe.AddCustomer(new Customer("C001", "Hasan", "hasan@email.com", 10));
cafe.AddCustomer(new Customer("C002", "Ali", "ali@email.com", 15));

cafe.AddPC(new PC("PC01", 5));
cafe.AddPC(new PC("PC02", 5));

string? lastSessionId = null;

while (true)
{
    Console.Clear();
    Console.WriteLine("=== Internet Cafe System ===");
    Console.WriteLine("1. Start Session");
    Console.WriteLine("2. End Last Session");
    Console.WriteLine("3. Exit");
    Console.Write("Select option: ");

    string? choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Enter Customer ID: ");
        string? customerId = Console.ReadLine();

        try
        {
            string sessionId = "S" + DateTime.Now.Ticks;
            var session = cafe.StartSession(sessionId, customerId!);
            lastSessionId = sessionId;

            Console.WriteLine("\nSession started:");
            Console.WriteLine(session);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "2")
    {
        try
        {
            if (lastSessionId == null)
            {
                Console.WriteLine("No session has been started yet.");
            }
            else
            {
                decimal cost = cafe.EndSession(lastSessionId);
                Console.WriteLine($"\nSession ended. Total cost: £{cost:F2}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "3")
    {
        break;
    }
}