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
            var pc = GetAvailablePC();

            if (pc == null)
                throw new Exception("No available PCs.");

            pc.IsAvailable = false;

            Session session = new Session(sessionId, customerId, pc.PCID, DateTime.Now);

            sessions.Add(session);

            return session;
        }

        public void EndSession(string sessionId)
        {
            foreach (var session in sessions)
            {
                if (session.SessionID == sessionId)
                {
                    session.EndTime = DateTime.Now;

                    foreach (var pc in pcs)
                    {
                        if (pc.PCID == session.PCID)
                        {
                            pc.IsAvailable = true;
                        }
                    }
                }
            }
        }
    }
}