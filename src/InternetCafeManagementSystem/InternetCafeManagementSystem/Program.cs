using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;

// Initialise the service layer - loads all data from the database on startup
InternetCafeService cafe = new InternetCafeService();

// List of banned words used to filter inappropriate user input
List<string> bannedWords = new List<string>
{
    "porn", "pornography", "fuck", "xxx", "shit", "bitch", "ass"
};

// Checks if the input contains any banned words - case insensitive - O(n)
bool ContainsBannedWord(string? input)
{
    if (string.IsNullOrWhiteSpace(input)) return false;
    foreach (var word in bannedWords)
    {
        if (input.ToLower().Contains(word))
            return true;
    }
    return false;
}

// Reusable input validation method used across all menu options
// Checks for empty input and banned words, returns a specific error message via out parameter
bool IsValidInput(string? input, string fieldName, out string? error)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        error = $"{fieldName} cannot be empty.";
        return false;
    }
    if (ContainsBannedWord(input))
    {
        error = "Inappropriate input detected.";
        return false;
    }
    error = null;
    return true;
}

// Main application loop - runs until the user selects Exit
while (true)
{
    Console.Clear();
    Console.WriteLine("=== SkySoft Internet Cafe System ===");
    Console.WriteLine("1. Start Session");
    Console.WriteLine("2. End Session");
    Console.WriteLine("3. Search Customer by ID");
    Console.WriteLine("4. Search Customer by Name");
    Console.WriteLine("5. View All Sessions");
    Console.WriteLine("6. View Active Sessions");
    Console.WriteLine("7. View PC Availability");
    Console.WriteLine("8. View Customer Session History");
    Console.WriteLine("9. Top Up Customer Balance");
    Console.WriteLine("10. Add Customer");
    Console.WriteLine("11. Exit");
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

        if (!IsValidInput(customerId, "Customer ID", out string? error))
        {
            Console.WriteLine($"\nError: {error}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            // Generate unique session ID using current time - format: S143022
            string sessionId = "S" + DateTime.Now.ToString("HHmmss");
            var session = cafe.StartSession(sessionId, customerId!);

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

        if (!IsValidInput(sessionId, "Session ID", out string? error))
        {
            Console.WriteLine($"\nError: {error}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            decimal cost = cafe.EndSession(sessionId!);
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
        // O(1) average - looks up customer directly in the hash table by ID
        Console.Write("Enter Customer ID: ");
        string? id = Console.ReadLine();

        if (!IsValidInput(id, "Customer ID", out string? error))
        {
            Console.WriteLine($"\nError: {error}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            var customer = cafe.GetCustomer(id!);
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
        // O(n) - iterates over all customers in the hash table using GetAll()
        Console.Write("Enter customer name to search: ");
        string? name = Console.ReadLine();

        if (!IsValidInput(name, "Name", out string? error))
        {
            Console.WriteLine($"\nError: {error}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        var results = cafe.SearchCustomersByName(name!);

        if (results.Count == 0)
        {
            Console.WriteLine("\nNo customers found with that name.");
        }
        else
        {
            Console.WriteLine($"\nFound {results.Count} customer(s):");
            foreach (var c in results)
            {
                Console.WriteLine(c);
            }
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "5")
    {
        // Iterates over the full session linked list - O(n)
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
    else if (choice == "6")
    {
        // Filters sessions where EndTime is null - O(n)
        var active = cafe.GetActiveSessions();

        Console.WriteLine("\nActive Sessions:");

        if (active.Count == 0)
        {
            Console.WriteLine("No active sessions.");
        }
        else
        {
            foreach (var s in active)
            {
                Console.WriteLine(s);
            }
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "7")
    {
        // Iterates over the PC linked list and displays availability status - O(n)
        var pcs = cafe.GetAllPCs();

        Console.WriteLine("\nPC Availability:");

        foreach (var pc in pcs)
        {
            Console.WriteLine(pc);
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "8")
    {
        // Filters sessions by CustomerID - O(n)
        Console.Write("Enter Customer ID: ");
        string? id = Console.ReadLine();

        if (!IsValidInput(id, "Customer ID", out string? error))
        {
            Console.WriteLine($"\nError: {error}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            var history = cafe.GetSessionHistory(id!);

            if (history.Count == 0)
            {
                Console.WriteLine("\nNo session history found for this customer.");
            }
            else
            {
                Console.WriteLine($"\nSession history for {id}:");
                foreach (var s in history)
                {
                    Console.WriteLine(s);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "9")
    {
        Console.Write("Enter Customer ID: ");
        string? id = Console.ReadLine();

        if (!IsValidInput(id, "Customer ID", out string? error))
        {
            Console.WriteLine($"\nError: {error}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        Console.Write("Enter top up amount (£): ");
        string? amountInput = Console.ReadLine();

        // Validate amount is a valid positive decimal number
        if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0)
        {
            Console.WriteLine("\nError: Please enter a valid amount greater than zero.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            cafe.TopUpBalance(id!, amount);
            Console.WriteLine($"\nBalance topped up successfully!");
            Console.WriteLine(cafe.GetCustomer(id!));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "10")
    {
        Console.Write("Enter Customer ID: ");
        string? newId = Console.ReadLine();

        if (!IsValidInput(newId, "Customer ID", out string? idError))
        {
            Console.WriteLine($"\nError: {idError}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        Console.Write("Enter Name: ");
        string? newName = Console.ReadLine();

        if (!IsValidInput(newName, "Name", out string? nameError))
        {
            Console.WriteLine($"\nError: {nameError}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        Console.Write("Enter Email: ");
        string? newEmail = Console.ReadLine();

        if (!IsValidInput(newEmail, "Email", out string? emailError))
        {
            Console.WriteLine($"\nError: {emailError}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        Console.Write("Enter Starting Balance (£): ");
        string? balanceInput = Console.ReadLine();

        // Validate balance is a non-negative decimal number
        if (!decimal.TryParse(balanceInput, out decimal balance) || balance < 0)
        {
            Console.WriteLine("\nError: Please enter a valid balance.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            continue;
        }

        try
        {
            var newCustomer = new Customer(newId!, newName!, newEmail!, balance);
            cafe.AddCustomer(newCustomer);
            Console.WriteLine($"\nCustomer added successfully!");
            Console.WriteLine(newCustomer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    else if (choice == "11")
    {
        Console.WriteLine("\nThank You for using SkySoft Internet Cafe System. Goodbye!");
        break;
    }
    else
    {
        Console.WriteLine("\nInvalid option. Please select 1-11.");
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }
}