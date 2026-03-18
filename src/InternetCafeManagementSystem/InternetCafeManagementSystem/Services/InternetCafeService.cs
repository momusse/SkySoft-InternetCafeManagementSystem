using System;
using System.Collections.Generic;
using System.Text;

using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.DataStructures;

namespace InternetCafeManagementSystem.Services
{
    public class InternetCafeService
    {
        private CustomHashTable<string, Customer> customers;
        private List<PC> pcs;
        private List<Session> sessions;

        public InternetCafeService()
        {
            customers = new CustomHashTable<string, Customer>(10);
            pcs = new List<PC>();
            sessions = new List<Session>();
        }

        public void AddCustomer(Customer customer)
        {
            customers.Add(customer.CustomerID, customer);
        }

        public Customer GetCustomer(string id)
        {
            return customers.Get(id);
        }

        public bool CustomerExists(string id)
        {
            return customers.Contains(id);
        }

        public void AddPC(PC pc)
        {
            pcs.Add(pc);
        }

        public PC? GetAvailablePC()
        {
            foreach (var pc in pcs)
            {
                if (pc.IsAvailable)
                    return pc;
            }

            return null;
        }

        public Session StartSession(string sessionId, string customerId)
        {
            if (!customers.Contains(customerId))
                throw new Exception("Customer not found.");

            var pc = GetAvailablePC();

            if (pc == null)
                throw new Exception("No available PCs.");

            pc.IsAvailable = false;

            Session session = new Session(sessionId, customerId, pc.PCID, DateTime.Now);
            sessions.Add(session);

            return session;
        }

        public decimal EndSession(string sessionId)
        {
            foreach (var session in sessions)
            {
                if (session.SessionID == sessionId && session.EndTime == null)
                {
                    session.EndTime = DateTime.Now;

                    decimal hourlyRate = 0;

                    foreach (var pc in pcs)
                    {
                        if (pc.PCID == session.PCID)
                        {
                            pc.IsAvailable = true;
                            hourlyRate = pc.HourlyRate;
                            break;
                        }
                    }

                    return session.CalculateCost(hourlyRate);
                }
            }

            throw new Exception("Active session not found.");
        }

        public List<Session> GetAllSessions()
        {
            return sessions;
        }
    }
}