using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;

InternetCafeService cafe = new InternetCafeService();

// Preloaded data
cafe.AddCustomer(new Customer("C001", "Hasan", "hasan@email.com", 10));
cafe.AddCustomer(new Customer("C002", "Ali", "ali@email.com", 15));

cafe.AddPC(new PC("PC01", 5));
cafe.AddPC(new PC("PC02", 5));

List<string> bannedWords = new List<string>
{
    "porn",
    "pornography",
    "fuck",
    "xxx"
};

while (true)
{
    Console.Clear();
    Console.WriteLine("=== Internet Cafe System ===");
    Console.WriteLine("1. Start Session");
    Console.WriteLine("2. End Session");
    Console.WriteLine("3. Search Customer");
    Console.WriteLine("4. View All Sessions");
    Console.WriteLine("5. Exit");
    Console.Write("Select option: ");

    string? choice = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(choice))
    {
        Console.WriteLine("\nError: Menu choice cannot be empty.");
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
        continue;
    }

    if (choice == "1")
    {
        Console.Write("Enter Customer ID: ");
        string? customerId = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(customerId))
        {
            Console.WriteLine("\nError: Customer ID cannot be empty.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        bool isBlocked = false;
        foreach (var word in bannedWords)
        {
            if (customerId.ToLower().Contains(word))
            {
                isBlocked = true;
                break;
            }
        }

        if (isBlocked)
        {
            Console.WriteLine("\nError: Inappropriate input detected.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            string sessionId = "S" + DateTime.Now.ToString("HHmmss");
            var session = cafe.StartSession(sessionId, customerId);

            Console.WriteLine("\nSession started successfully!");
            Console.WriteLine(session);
            Console.WriteLine("\n*** YOUR SESSION ID IS: " + sessionId + " ***");
            Console.WriteLine("*** Write this down to end your session! ***");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "2")
    {
        Console.Write("Enter Session ID to end: ");
        string? sessionId = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            Console.WriteLine("\nError: Session ID cannot be empty.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            decimal cost = cafe.EndSession(sessionId);
            Console.WriteLine($"\nSession ended successfully.");
            Console.WriteLine($"Total cost: £{cost:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "3")
    {
        Console.Write("Enter Customer ID: ");
        string? id = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(id))
        {
            Console.WriteLine("\nError: Customer ID cannot be empty.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            var customer = cafe.GetCustomer(id);
            Console.WriteLine("\nCustomer found:");
            Console.WriteLine(customer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
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
        Console.WriteLine("\nGoodbye!");
        break;
    }
    else
    {
        Console.WriteLine("\nInvalid option. Please select 1-5.");
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }
}