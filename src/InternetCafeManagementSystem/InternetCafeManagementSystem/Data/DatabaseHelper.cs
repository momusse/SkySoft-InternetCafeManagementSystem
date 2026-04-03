using System;
using Microsoft.Data.SqlClient;
using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.DataStructures;

namespace InternetCafeManagementSystem.Data
{
    /// <summary>
    /// Handles all database operations for the SkySoft Internet Cafe System.
    /// Uses ADO.NET to connect to SQL Server Express and persists data across sessions.
    /// </summary>
    public class DatabaseHelper
    {
        // Windows Authentication used - no username/password needed for local development
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=InternetCafeDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Loads all customers from the database into the custom hash table - O(n)
        public void LoadCustomers(CustomHashTable<string, Customer> customers)
        {
            // 'using' ensures the connection is closed automatically after use
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "SELECT CustomerID, Name, Email, Balance FROM Customers";
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string id = reader.GetString(0);
                string name = reader.GetString(1);
                string email = reader.GetString(2);
                decimal balance = reader.GetDecimal(3);

                // Avoid duplicates in case LoadCustomers is called more than once
                if (!customers.Contains(id))
                    customers.Add(id, new Customer(id, name, email, balance));
            }
        }

        // Loads all PCs from the database into the custom linked list - O(n)
        public void LoadPCs(CustomLinkedList<PC> pcs)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "SELECT PCID, HourlyRate, IsAvailable FROM PCs";
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string id = reader.GetString(0);
                decimal rate = reader.GetDecimal(1);
                bool available = reader.GetBoolean(2);

                PC pc = new PC(id, rate);
                pc.IsAvailable = available;
                pcs.AddLast(pc);
            }
        }

        // Loads all sessions from the database into the custom linked list - O(n)
        // EndTime can be NULL for sessions that are still active
        public void LoadSessions(CustomLinkedList<Session> sessions)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "SELECT SessionID, CustomerID, PCID, StartTime, EndTime FROM Sessions";
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string sessionId = reader.GetString(0);
                string customerId = reader.GetString(1);
                string pcId = reader.GetString(2);
                DateTime startTime = reader.GetDateTime(3);

                Session session = new Session(sessionId, customerId, pcId, startTime);

                // IsDBNull checks for NULL in the database - only set EndTime if session has ended
                if (!reader.IsDBNull(4))
                    session.EndTime = reader.GetDateTime(4);

                sessions.AddLast(session);
            }
        }

        // Saves a new session when it starts - EndTime and Cost are NULL until session ends - O(1)
        public void SaveSession(Session session)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // Parameterised queries used throughout to prevent SQL injection
            string query = "INSERT INTO Sessions (SessionID, CustomerID, PCID, StartTime, EndTime, Cost) VALUES (@sid, @cid, @pcid, @start, @end, @cost)";
            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@sid", session.SessionID);
            cmd.Parameters.AddWithValue("@cid", session.CustomerID);
            cmd.Parameters.AddWithValue("@pcid", session.PCID);
            cmd.Parameters.AddWithValue("@start", session.StartTime);
            // DBNull.Value stores NULL in the database for fields not yet populated
            cmd.Parameters.AddWithValue("@end", (object?)session.EndTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cost", DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        // Updates a session when it ends, storing the EndTime and calculated Cost - O(1)
        public void UpdateSession(Session session, decimal cost)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "UPDATE Sessions SET EndTime = @end, Cost = @cost WHERE SessionID = @sid";
            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@end", session.EndTime!);
            cmd.Parameters.AddWithValue("@cost", cost);
            cmd.Parameters.AddWithValue("@sid", session.SessionID);

            cmd.ExecuteNonQuery();
        }

        // Updates a customer's balance in the database after a top up - O(1)
        public void UpdateCustomerBalance(Customer customer)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "UPDATE Customers SET Balance = @balance WHERE CustomerID = @id";
            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@balance", customer.Balance);
            cmd.Parameters.AddWithValue("@id", customer.CustomerID);

            cmd.ExecuteNonQuery();
        }

        // Saves a newly created customer to the database - O(1)
        public void SaveCustomer(Customer customer)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "INSERT INTO Customers (CustomerID, Name, Email, Balance) VALUES (@id, @name, @email, @balance)";
            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@id", customer.CustomerID);
            cmd.Parameters.AddWithValue("@name", customer.Name);
            cmd.Parameters.AddWithValue("@email", customer.Email);
            cmd.Parameters.AddWithValue("@balance", customer.Balance);

            cmd.ExecuteNonQuery();
        }

        // Tests the database connection on startup - O(1)
        public bool TestConnection()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}