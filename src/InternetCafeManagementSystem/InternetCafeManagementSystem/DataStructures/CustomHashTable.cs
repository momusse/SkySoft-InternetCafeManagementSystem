using System;
using System.Collections.Generic;

namespace InternetCafeManagementSystem.DataStructures
{
    public class CustomHashTable<TKey, TValue>
    {
        private LinkedList<KeyValuePair<TKey, TValue>>[] buckets;
        private int size;

        public CustomHashTable(int size)
        {
            this.size = size;
            buckets = new LinkedList<KeyValuePair<TKey, TValue>>[size];

            for (int i = 0; i < size; i++)
            {
                buckets[i] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }
        }

        private int GetBucketIndex(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % size;
        }

        public void Add(TKey key, TValue value)
        {
            int index = GetBucketIndex(key);

            foreach (var pair in buckets[index])
            {
                if (pair.Key.Equals(key))
                {
                    throw new ArgumentException("Duplicate key is not allowed.");
                }
            }

            buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
        }

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

            throw new KeyNotFoundException("Key not found.");
        }

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
                    return true;
                }

                current = current.Next;
            }

            return false;
        }
    }
}