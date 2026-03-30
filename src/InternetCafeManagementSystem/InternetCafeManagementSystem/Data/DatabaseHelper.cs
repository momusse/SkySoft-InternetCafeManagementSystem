using System;
using Microsoft.Data.SqlClient;
using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.DataStructures;

namespace InternetCafeManagementSystem.Data
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=InternetCafeDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Load all customers from DB into the hash table
        public void LoadCustomers(CustomHashTable<string, Customer> customers)
        {
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

                // Only add if not already in hash table
                if (!customers.Contains(id))
                    customers.Add(id, new Customer(id, name, email, balance));
            }
        }

        // Load all PCs from DB into the linked list
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

        // Load all sessions from DB into the linked list
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

                if (!reader.IsDBNull(4))
                    session.EndTime = reader.GetDateTime(4);

                sessions.AddLast(session);
            }
        }

        // Save a new session to the DB
        public void SaveSession(Session session)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "INSERT INTO Sessions (SessionID, CustomerID, PCID, StartTime, EndTime, Cost) VALUES (@sid, @cid, @pcid, @start, @end, @cost)";
            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@sid", session.SessionID);
            cmd.Parameters.AddWithValue("@cid", session.CustomerID);
            cmd.Parameters.AddWithValue("@pcid", session.PCID);
            cmd.Parameters.AddWithValue("@start", session.StartTime);
            cmd.Parameters.AddWithValue("@end", (object?)session.EndTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cost", DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        // Update customer balance in DB
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

        // Update session when it ends
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

        // Save a new customer to the DB
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
    }
}