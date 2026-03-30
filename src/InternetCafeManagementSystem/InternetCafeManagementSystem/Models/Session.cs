using System;
using System.Collections.Generic;
using System.Text;

namespace InternetCafeManagementSystem.Models
{
    /// <summary>
    /// Represents a customer's PC usage session in the SkySoft Internet Cafe System.
    /// Stored in the custom linked list and persisted to the Sessions table in the database.
    /// EndTime is nullable - null means the session is still active.
    /// </summary>
    public class Session
    {
        public string SessionID { get; set; }   // Unique identifier generated at session start
        public string CustomerID { get; set; }  // Links session to a customer in the hash table
        public string PCID { get; set; }        // Links session to a PC in the linked list
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }  // Null until the session is ended

        public Session(string sessionID, string customerID, string pcid, DateTime startTime)
        {
            SessionID = sessionID;
            CustomerID = customerID;
            PCID = pcid;
            StartTime = startTime;
            EndTime = null; // Session starts as active
        }

        // Calculates the total cost based on duration and hourly rate - O(1)
        // Returns 0 if the session is still active (EndTime is null)
        public decimal CalculateCost(decimal hourlyRate)
        {
            if (EndTime == null)
                return 0;

            TimeSpan duration = EndTime.Value - StartTime;
            return (decimal)duration.TotalHours * hourlyRate;
        }

        // Returns a formatted string showing session details and current status
        public override string ToString()
        {
            string durationText = "In progress";

            if (EndTime != null)
            {
                TimeSpan duration = EndTime.Value - StartTime;
                durationText = $"{duration.TotalMinutes:F1} mins";
            }

            return $"{SessionID} - Customer: {CustomerID} - PC: {PCID} - Duration: {durationText}";
        }
    }
}