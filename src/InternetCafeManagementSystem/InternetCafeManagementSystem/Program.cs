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
    Console.WriteLine("3. Search Customer");
    Console.WriteLine("4. View All Sessions");
    Console.WriteLine("5. Exit");
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
        Console.Write("Enter Customer ID: ");
        string? id = Console.ReadLine();

        try
        {
            var customer = cafe.GetCustomer(id!);
            Console.WriteLine("\nCustomer found:");
            Console.WriteLine(customer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    else if (choice == "4")
    {
        var sessions = cafe.GetAllSessions();

        Console.WriteLine("\nAll Sessions:");

        if (sessions.Count == 0)
        {
            Console.WriteLine("No sessions found.");
        }
        else
        {
            foreach (var s in sessions)
            {
                Console.WriteLine(s);
            }
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "5")
    {
        break;
    }
    else
    {
        Console.WriteLine("Invalid option. Press any key...");
        Console.ReadKey();
    }
}