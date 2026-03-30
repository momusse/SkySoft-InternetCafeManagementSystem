using System;
using System.Collections.Generic;

namespace InternetCafeManagementSystem.DataStructures
{
    /// <summary>
    /// A generic hash table implemented using separate chaining for collision resolution.
    /// Keys are hashed to a bucket index, and each bucket holds a linked list of key-value pairs.
    /// Average time complexity for Add, Get, Contains and Remove is O(1).
    /// </summary>
    public class CustomHashTable<TKey, TValue> where TKey : notnull
    {
        // Array of linked lists - each index is a bucket that holds key-value pairs
        private LinkedList<KeyValuePair<TKey, TValue>>[] buckets;
        private int size;  // Total number of buckets
        private int count; // Total number of key-value pairs stored

        public int Count => count;

        public CustomHashTable(int size)
        {
            this.size = size;
            count = 0;
            buckets = new LinkedList<KeyValuePair<TKey, TValue>>[size];

            // Initialise each bucket as an empty linked list
            for (int i = 0; i < size; i++)
            {
                buckets[i] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }
        }

        // Converts a key into a valid bucket index using bitwise AND to ensure positive value
        // & 0x7FFFFFFF strips the sign bit, avoiding negative index from GetHashCode() - O(1)
        private int GetBucketIndex(TKey key)
        {
            return (key.GetHashCode() & 0x7FFFFFFF) % size;
        }

        // Adds a key-value pair to the hash table - O(1) average
        // Throws ArgumentException if the key already exists (no duplicate keys allowed)
        public void Add(TKey key, TValue value)
        {
            int index = GetBucketIndex(key);

            // Check the bucket for duplicate keys before inserting
            foreach (var pair in buckets[index])
            {
                if (pair.Key.Equals(key))
                {
                    throw new ArgumentException($"Key '{key}' already exists.");
                }
            }

            // Add new pair to the end of the bucket's linked list
            buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            count++;
        }

        // Retrieves a value by key - O(1) average
        // Throws KeyNotFoundException if the key does not exist
        public TValue Get(TKey key)
        {
            int index = GetBucketIndex(key);

            // Search only within the relevant bucket
            foreach (var pair in buckets[index])
            {
                if (pair.Key.Equals(key))
                {
                    return pair.Value;
                }
            }

            throw new KeyNotFoundException($"Key '{key}' was not found.");
        }

        // Returns true if the key exists in the hash table, false otherwise - O(1) average
        public bool Contains(TKey key)
        {
            int index = GetBucketIndex(key);

            foreach (var pair in buckets[index])
            {
                if (pair.Key.Equals(key))
                {
                    return true;
                }
            }

            return false;
        }

        // Removes a key-value pair from the hash table - O(1) average
        // Returns true if removed, false if key was not found
        public bool Remove(TKey key)
        {
            int index = GetBucketIndex(key);
            var bucket = buckets[index];
            var current = bucket.First;

            // Traverse the bucket's linked list to find the matching key
            while (current != null)
            {
                if (current.Value.Key.Equals(key))
                {
                    bucket.Remove(current);
                    count--;
                    return true;
                }

                current = current.Next;
            }

            return false;
        }

        // Returns all values across every bucket - O(n)
        // Used for operations that need to iterate over all stored values e.g. search by name
        public IEnumerable<TValue> GetAll()
        {
            foreach (var bucket in buckets)
            {
                foreach (var pair in bucket)
                {
                    // yield return produces values one at a time without building a full list
                    yield return pair.Value;
                }
            }
        }
    }
}