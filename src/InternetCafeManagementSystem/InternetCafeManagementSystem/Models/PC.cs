using System;
using System.Collections.Generic;
using System.Text;

namespace InternetCafeManagementSystem.Models
{
    /// <summary>
    /// Represents a PC unit in the SkySoft Internet Cafe System.
    /// Stored in the custom linked list and assigned to customers during sessions.
    /// </summary>
    public class PC
    {
        public string PCID { get; set; }        // Unique identifier for the PC
        public bool IsAvailable { get; set; }   // False when assigned to an active session
        public decimal HourlyRate { get; set; } // Cost per hour charged to the customer

        public PC(string pcid, decimal hourlyRate)
        {
            PCID = pcid;
            HourlyRate = hourlyRate;
            IsAvailable = true; // All PCs start as available when loaded
        }

        // Returns a formatted string showing availability status and hourly rate
        public override string ToString()
        {
            string status = IsAvailable ? "Available" : "In Use";
            return $"{PCID} - {status} - Rate: £{HourlyRate:F2}/hr";
        }
    }
}