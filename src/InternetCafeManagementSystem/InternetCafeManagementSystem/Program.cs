using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;

InternetCafeService cafe = new InternetCafeService();

List<string> bannedWords = new List<string>
{
    "porn", "pornography", "fuck", "xxx", "shit", "bitch", "ass"
};

// Reusable input validation methods
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

while (true)
{
    Console.Clear();
    Console.WriteLine("=== Internet Cafe System ===");
    Console.WriteLine("1. Start Session");
    Console.WriteLine("2. End Session");
    Console.WriteLine("3. Search Customer");
    Console.WriteLine("4. View All Sessions");
    Console.WriteLine("5. Add Customer");
    Console.WriteLine("6. Exit");
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
    else if (choice == "6")
    {
        Console.WriteLine("\nGoodbye!");
        break;
    }
    else
    {
        Console.WriteLine("\nInvalid option. Please select 1-6.");
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }
}