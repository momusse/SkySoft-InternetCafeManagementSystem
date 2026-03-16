using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.DataStructures;

Customer c1 = new Customer("C001", "Hasan", "hasan@email.com", 10);
PC pc1 = new PC("PC01", 5);
Session s1 = new Session("S001", c1.CustomerID, pc1.PCID, DateTime.Now);

Console.WriteLine(c1);
Console.WriteLine(pc1);
Console.WriteLine(s1);

Console.ReadLine();