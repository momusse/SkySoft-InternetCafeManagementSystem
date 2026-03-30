using System;
using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.DataStructures;
using InternetCafeManagementSystem.Data;

namespace InternetCafeManagementSystem.Services
{
    public class InternetCafeService
    {
        private CustomHashTable<string, Customer> customers;
        private CustomLinkedList<PC> pcs;
        private CustomLinkedList<Session> sessions;
        private DatabaseHelper db;

        public InternetCafeService()
        {
            customers = new CustomHashTable<string, Customer>(10);
            pcs = new CustomLinkedList<PC>();
            sessions = new CustomLinkedList<Session>();
            db = new DatabaseHelper();

            // Load data from database on startup
            db.LoadCustomers(customers);
            db.LoadPCs(pcs);
            db.LoadSessions(sessions);
        }

        // --- Customer methods ---

        public void AddCustomer(Customer customer)
        {
            customers.Add(customer.CustomerID, customer);
            db.SaveCustomer(customer);
        }

        public Customer GetCustomer(string id)
        {
            return customers.Get(id);
        }

        public bool CustomerExists(string id)
        {
            return customers.Contains(id);
        }

        // --- PC methods ---

        public void AddPC(PC pc)
        {
            pcs.AddLast(pc);
        }

        public PC? GetAvailablePC()
        {
            return pcs.Find(pc => pc.IsAvailable);
        }

        // --- Session methods ---

        // O(1) average for customer lookup (hash table)
        // O(n) for finding available PC (linked list traversal)
        public Session StartSession(string sessionId, string customerId)
        {
            if (!customers.Contains(customerId))
                throw new Exception("Customer not found.");

            var pc = GetAvailablePC();

            if (pc == null)
                throw new Exception("No PCs are currently available.");

            pc.IsAvailable = false;

            Session session = new Session(sessionId, customerId, pc.PCID, DateTime.Now);
            sessions.AddLast(session);

            // Save to database
            db.SaveSession(session);

            return session;
        }

        // O(n) - traverses session linked list to find the session
        public decimal EndSession(string sessionId)
        {
            var session = sessions.Find(s => s.SessionID == sessionId && s.EndTime == null);

            if (session == null)
                throw new Exception("Active session not found.");

            session.EndTime = DateTime.Now;

            var pc = pcs.Find(p => p.PCID == session.PCID);

            if (pc == null)
                throw new Exception("PC not found.");

            pc.IsAvailable = true;

            decimal cost = session.CalculateCost(pc.HourlyRate);

            // Update database
            db.UpdateSession(session, cost);

            return cost;
        }

        public CustomLinkedList<Session> GetAllSessions()
        {
            return sessions;
        }

        public CustomLinkedList<PC> GetAllPCs()
        {
            return pcs;
        }
    }
}