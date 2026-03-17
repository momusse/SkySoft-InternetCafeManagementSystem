using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;

InternetCafeService cafe = new InternetCafeService();

Customer c1 = new Customer("C001", "Hasan", "hasan@email.com", 10);

cafe.AddCustomer(c1);

PC pc1 = new PC("PC01", 5);
PC pc2 = new PC("PC02", 5);

cafe.AddPC(pc1);
cafe.AddPC(pc2);

var session = cafe.StartSession("S001", "C001");

Console.WriteLine("Session started:");
Console.WriteLine(session);

Console.ReadLine();