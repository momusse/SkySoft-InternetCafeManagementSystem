using System;
using System.Collections.Generic;
using System.Text;

namespace InternetCafeManagementSystem.Models
{
    public class Session
    {
        public string SessionID { get; set; }
        public string CustomerID { get; set; }
        public string PCID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Session(string sessionID, string customerID, string pcid, DateTime startTime)
        {
            SessionID = sessionID;
            CustomerID = customerID;
            PCID = pcid;
            StartTime = startTime;
            EndTime = null;
        }

        public decimal CalculateCost(decimal hourlyRate)
        {
            if (EndTime == null)
                return 0;

            TimeSpan duration = EndTime.Value - StartTime;
            return (decimal)duration.TotalHours * hourlyRate;
        }

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