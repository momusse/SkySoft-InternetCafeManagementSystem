using System;
using System.Collections.Generic;
using System.Text;

namespace InternetCafeManagementSystem.Models
{
    public class PC
    {
        public string PCID { get; set; }
        public bool IsAvailable { get; set; }
        public decimal HourlyRate { get; set; }

        public PC(string pcid, decimal hourlyRate)
        {
            PCID = pcid;
            HourlyRate = hourlyRate;
            IsAvailable = true;
        }

        public override string ToString()
        {
            return $"{PCID} - Available: {IsAvailable} - Rate: £{HourlyRate}/hr";
        }
    }
}