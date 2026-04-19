using System;
using System.Collections.Generic;
using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.DataStructures;
using InternetCafeManagementSystem.Data;

namespace InternetCafeManagementSystem.Services
{
    /// <summary>
    /// Core service layer for the SkySoft Internet Cafe System.
    /// Manages all business logic for customers, PCs and sessions.
    /// Uses a custom hash table for O(1) average customer lookups,
    /// and custom linked lists for PC and session management.
    /// All changes are persisted to the SQL database via DatabaseHelper.
    /// </summary>
    public class InternetCafeService
    {
        private CustomHashTable<string, Customer> customers; // Keyed by CustomerID for O(1) lookup
        private CustomLinkedList<PC> pcs;                    // Linked list of all PCs in the cafe
        private CustomLinkedList<Session> sessions;          // Linked list of all sessions
        private DatabaseHelper db;                           // Handles all database read/write operations

        public InternetCafeService()
        {
            customers = new CustomHashTable<string, Customer>(10);
            pcs = new CustomLinkedList<PC>();
            sessions = new CustomLinkedList<Session>();
            db = new DatabaseHelper();

            // Test database connection before attempting to load data
            if (!db.TestConnection())
            {
                Console.WriteLine("ERROR: Could not connect to the database.");
                Console.WriteLine("Please make sure SQL Server Express is installed and the database.sql script has been run.");
                Console.WriteLine("See README.md for setup instructions.");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            // Load data from database on startup
            db.LoadCustomers(customers);
            db.LoadPCs(pcs);
            db.LoadSessions(sessions);
        }

        // --- Customer Methods ---

        // Adds a new customer to the hash table and persists to the database - O(1) average
        public void AddCustomer(Customer customer)
        {
            customers.Add(customer.CustomerID, customer);
            db.SaveCustomer(customer);
        }

        // Retrieves a customer by ID using the hash table - O(1) average
        // Throws KeyNotFoundException if the customer does not exist
        public Customer GetCustomer(string id)
        {
            return customers.Get(id);
        }

        // Checks if a customer exists in the hash table - O(1) average
        public bool CustomerExists(string id)
        {
            return customers.Contains(id);
        }

        // Searches all customers by name using a partial, case-insensitive match - O(n)
        // Uses GetAll() to iterate over every value in the hash table
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

        // Tops up a customer's balance and updates the database - O(1) average
        public void TopUpBalance(string customerId, decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Top up amount must be greater than zero.");

            // Get reference to the customer object and update balance directly
            var customer = customers.Get(customerId);
            customer.Balance += amount;
            db.UpdateCustomerBalance(customer);
        }

        // --- PC Methods ---

        // Adds a new PC to the linked list - O(n) due to AddLast traversal
        public void AddPC(PC pc)
        {
            pcs.AddLast(pc);
        }

        // Returns the first available PC in the linked list - O(n)
        // Returns null if all PCs are currently in use
        public PC? GetAvailablePC()
        {
            return pcs.Find(pc => pc.IsAvailable);
        }

        // Returns the full linked list of PCs - O(1)
        public CustomLinkedList<PC> GetAllPCs()
        {
            return pcs;
        }

        // --- Session Methods ---

        // Starts a new session for a customer on the first available PC
        // O(1) average for customer lookup (hash table)
        // O(n) for finding an available PC (linked list traversal)
        public Session StartSession(string sessionId, string customerId)
        {
            if (!customers.Contains(customerId))
                throw new Exception("Customer not found.");

            var pc = GetAvailablePC();

            if (pc == null)
                throw new Exception("No PCs are currently available.");

            // Mark the PC as unavailable so it cannot be assigned to another session
            pc.IsAvailable = false;

            Session session = new Session(sessionId, customerId, pc.PCID, DateTime.Now);
            sessions.AddLast(session);

            // Persist the new session to the database immediately
            db.SaveSession(session);

            return session;
        }

        // Ends an active session, calculates cost and frees up the PC - O(n)
        // Traverses the session linked list to find the matching active session
        public decimal EndSession(string sessionId)
        {
            // Find the session that matches the ID and is still active (EndTime is null)
            var session = sessions.Find(s => s.SessionID == sessionId && s.EndTime == null);

            if (session == null)
                throw new Exception("Active session not found.");

            session.EndTime = DateTime.Now;

            // Find the PC used in this session and mark it as available again
            var pc = pcs.Find(p => p.PCID == session.PCID);

            if (pc == null)
                throw new Exception("PC not found.");

            pc.IsAvailable = true;

            decimal cost = session.CalculateCost(pc.HourlyRate);

            // Persist the updated session (EndTime and Cost) to the database
            db.UpdateSession(session, cost);

            return cost;
        }

        // Returns the full linked list of all sessions - O(1)
        public CustomLinkedList<Session> GetAllSessions()
        {
            return sessions;
        }

        // Returns only sessions that are still active (EndTime is null) - O(n)
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

        // Returns all sessions for a specific customer, both active and ended - O(n)
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
