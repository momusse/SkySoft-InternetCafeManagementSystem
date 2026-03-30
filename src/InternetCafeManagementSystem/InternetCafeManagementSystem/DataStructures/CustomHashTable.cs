using System;
using System.Collections.Generic;

namespace InternetCafeManagementSystem.DataStructures
{
    public class CustomHashTable<TKey, TValue> where TKey : notnull
    {
        private LinkedList<KeyValuePair<TKey, TValue>>[] buckets;
        private int size;
        private int count;

        public int Count => count;

        public CustomHashTable(int size)
        {
            this.size = size;
            count = 0;
            buckets = new LinkedList<KeyValuePair<TKey, TValue>>[size];

            for (int i = 0; i < size; i++)
            {
                buckets[i] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }
        }

        // O(1) average - uses hash to find bucket directly
        private int GetBucketIndex(TKey key)
        {
            return (key.GetHashCode() & 0x7FFFFFFF) % size;
        }

        // O(1) average - only searches within one bucket
        public void Add(TKey key, TValue value)
        {
            int index = GetBucketIndex(key);

            foreach (var pair in buckets[index])
            {
                if (pair.Key.Equals(key))
                {
                    throw new ArgumentException($"Key '{key}' already exists.");
                }
            }

            buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            count++;
        }

        // O(1) average - only searches within one bucket
        public TValue Get(TKey key)
        {
            int index = GetBucketIndex(key);

            foreach (var pair in buckets[index])
            {
                if (pair.Key.Equals(key))
                {
                    return pair.Value;
                }
            }

            throw new KeyNotFoundException($"Key '{key}' was not found.");
        }

        // O(1) average - only searches within one bucket
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

        // O(1) average - only searches within one bucket
        public bool Remove(TKey key)
        {
            int index = GetBucketIndex(key);
            var bucket = buckets[index];
            var current = bucket.First;

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

        // O(n) - must visit every bucket and every item
        public IEnumerable<TValue> GetAll()
        {
            foreach (var bucket in buckets)
            {
                foreach (var pair in bucket)
                {
                    yield return pair.Value;
                }
            }
        }
    }
}