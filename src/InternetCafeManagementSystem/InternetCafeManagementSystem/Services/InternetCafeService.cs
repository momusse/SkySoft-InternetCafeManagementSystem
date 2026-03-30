using System;
using System.Collections.Generic;
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

        // O(1) average - hash table lookup
        public Customer GetCustomer(string id)
        {
            return customers.Get(id);
        }

        public bool CustomerExists(string id)
        {
            return customers.Contains(id);
        }

        // O(n) - searches all customers by name
        public List<Customer> SearchCustomersByName(string name)
        {
            List<Customer> results = new List<Customer>();

            foreach (var customer in customers.GetAll())
            {
                if (customer.Name.ToLower().Contains(name.ToLower()))
                {
                    results.Add(customer);
                }
            }

            return results;
        }

        // O(n) - top up customer balance
        public void TopUpBalance(string customerId, decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Top up amount must be greater than zero.");

            var customer = customers.Get(customerId);
            customer.Balance += amount;
            db.UpdateCustomerBalance(customer);
        }

        // --- PC methods ---

        public void AddPC(PC pc)
        {
            pcs.AddLast(pc);
        }

        // O(n) - searches linked list for available PC
        public PC? GetAvailablePC()
        {
            return pcs.Find(pc => pc.IsAvailable);
        }

        // O(n) - returns all PCs
        public CustomLinkedList<PC> GetAllPCs()
        {
            return pcs;
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

        // O(n) - returns all sessions
        public CustomLinkedList<Session> GetAllSessions()
        {
            return sessions;
        }

        // O(n) - returns only active sessions
        public List<Session> GetActiveSessions()
        {
            List<Session> active = new List<Session>();

            foreach (var session in sessions)
            {
                if (session.EndTime == null)
                    active.Add(session);
            }

            return active;
        }

        // O(n) - returns session history for a specific customer
        public List<Session> GetSessionHistory(string customerId)
        {
            List<Session> history = new List<Session>();

            foreach (var session in sessions)
            {
                if (session.CustomerID == customerId)
                    history.Add(session);
            }

            return history;
        }
    }
}