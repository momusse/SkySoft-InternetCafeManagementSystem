using System;
using System.Collections.Generic;
using System.Text;

namespace InternetCafeManagementSystem.Models
{
    /// <summary>
    /// Represents a registered customer in the SkySoft Internet Cafe System.
    /// Stored in the custom hash table using CustomerID as the key.
    /// </summary>
    public class Customer
    {
        public string CustomerID { get; set; } // Unique identifier used as the hash table key
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }   // Credit balance used to pay for sessions

        public Customer(string customerID, string name, string email, decimal balance)
        {
            CustomerID = customerID;
            Name = name;
            Email = email;
            Balance = balance;
        }

        // Returns a formatted string representation of the customer
        public override string ToString()
        {
            return $"{CustomerID} - {Name} - Balance: £{Balance:F2}";
        }
    }
}