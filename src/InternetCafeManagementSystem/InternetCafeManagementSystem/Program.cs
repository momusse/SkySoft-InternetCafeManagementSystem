using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.DataStructures;

CustomHashTable<string, Customer> customerTable = new CustomHashTable<string, Customer>(10);

Customer c1 = new Customer("C001", "Hasan", "hasan@email.com", 10);
Customer c2 = new Customer("C002", "Ali", "ali@email.com", 15);

customerTable.Add(c1.CustomerID, c1);
customerTable.Add(c2.CustomerID, c2);

Console.WriteLine(customerTable.Get("C001"));
Console.WriteLine(customerTable.Get("C002"));
Console.WriteLine("Contains C001: " + customerTable.Contains("C001"));
Console.WriteLine("Removed C001: " + customerTable.Remove("C001"));
Console.WriteLine("Contains C001 after removal: " + customerTable.Contains("C001"));

Console.ReadLine();