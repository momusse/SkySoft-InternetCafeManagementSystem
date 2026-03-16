using System;
using System.Collections.Generic;
using System.Text;

namespace InternetCafeManagementSystem.Models
{
    public class Customer
    {
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }

        public Customer(string customerID, string name, string email, decimal balance)
        {
            CustomerID = customerID;
            Name = name;
            Email = email;
            Balance = balance;
        }

        public override string ToString()
        {
            return $"{CustomerID} - {Name} - Balance: £{Balance}";
        }
    }
}