using System;
using System.Collections;
using System.Collections.Generic;

namespace InternetCafeManagementSystem.DataStructures
{
    // Represents a single node in the linked list
    // Each node holds data and a reference to the next node
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T>? Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null; // Next is null by default until linked to another node
        }
    }

    /// <summary>
    /// A generic singly linked list implemented from scratch.
    /// Each node points to the next, forming a chain from head to null.
    /// Implements IEnumerable to support foreach loops across the list.
    /// Used to store PCs and Sessions in the SkySoft Internet Cafe System.
    /// </summary>
    public class CustomLinkedList<T> : IEnumerable<T>
    {
        private Node<T>? head; // Points to the first node in the list
        private int count;     // Tracks total number of nodes

        public int Count => count;

        public CustomLinkedList()
        {
            head = null;
            count = 0;
        }

        // Adds a new node at the front of the list - O(1)
        // Faster than AddLast as no traversal is needed
        public void AddFirst(T data)
        {
            Node<T> newNode = new Node<T>(data);
            newNode.Next = head; // New node points to current head
            head = newNode;      // New node becomes the new head
            count++;
        }

        // Adds a new node at the end of the list - O(n)
        // Must traverse the full list to find the last node
        public void AddLast(T data)
        {
            Node<T> newNode = new Node<T>(data);

            // If list is empty, new node becomes the head
            if (head == null)
            {
                head = newNode;
                count++;
                return;
            }

            // Traverse to the last node
            Node<T> current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }

            // Link the last node to the new node
            current.Next = newNode;
            count++;
        }

        // Searches the list using a predicate (lambda condition) - O(n)
        // Returns the first matching item, or default (null) if not found
        public T? Find(Func<T, bool> predicate)
        {
            Node<T>? current = head;

            while (current != null)
            {
                if (predicate(current.Data))
                    return current.Data;

                current = current.Next;
            }

            // Returns null for reference types if no match found
            return default;
        }

        // Removes the first node matching the predicate - O(n)
        // Returns true if removed, false if no match found
        public bool Remove(Func<T, bool> predicate)
        {
            if (head == null)
                return false;

            // Special case: if the head node matches, update head to the next node
            if (predicate(head.Data))
            {
                head = head.Next;
                count--;
                return true;
            }

            // Traverse and check the next node at each step
            // This allows us to re-link around the removed node
            Node<T>? current = head;
            while (current.Next != null)
            {
                if (predicate(current.Next.Data))
                {
                    // Skip over the matching node by linking to the one after it
                    current.Next = current.Next.Next;
                    count--;
                    return true;
                }

                current = current.Next;
            }

            return false;
        }

        // Returns true if any node matches the predicate - O(n)
        public bool Contains(Func<T, bool> predicate)
        {
            return Find(predicate) != null;
        }

        // Implements IEnumerable to allow foreach loops on this list - O(n)
        // yield return produces each value one at a time without building a separate collection
        public IEnumerator<T> GetEnumerator()
        {
            Node<T>? current = head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        // Required by IEnumerable for non-generic foreach support
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}