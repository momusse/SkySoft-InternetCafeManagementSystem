using System;
using System.Collections;
using System.Collections.Generic;

namespace InternetCafeManagementSystem.DataStructures
{
    // A single node in the linked list
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T>? Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    public class CustomLinkedList<T> : IEnumerable<T>
    {
        private Node<T>? head;
        private int count;

        public int Count => count;

        public CustomLinkedList()
        {
            head = null;
            count = 0;
        }

        // O(1) - always adds to the front
        public void AddFirst(T data)
        {
            Node<T> newNode = new Node<T>(data);
            newNode.Next = head;
            head = newNode;
            count++;
        }

        // O(n) - traverses to the end to add
        public void AddLast(T data)
        {
            Node<T> newNode = new Node<T>(data);

            if (head == null)
            {
                head = newNode;
                count++;
                return;
            }

            Node<T> current = head;

            while (current.Next != null)
            {
                current = current.Next;
            }

            current.Next = newNode;
            count++;
        }

        // O(n) - searches through each node
        public T? Find(Func<T, bool> predicate)
        {
            Node<T>? current = head;

            while (current != null)
            {
                if (predicate(current.Data))
                    return current.Data;

                current = current.Next;
            }

            return default;
        }

        // O(n) - must traverse to find the node
        public bool Remove(Func<T, bool> predicate)
        {
            if (head == null)
                return false;

            // Special case: removing the head node
            if (predicate(head.Data))
            {
                head = head.Next;
                count--;
                return true;
            }

            Node<T>? current = head;

            while (current.Next != null)
            {
                if (predicate(current.Next.Data))
                {
                    current.Next = current.Next.Next;
                    count--;
                    return true;
                }

                current = current.Next;
            }

            return false;
        }

        // O(n) - checks every node
        public bool Contains(Func<T, bool> predicate)
        {
            return Find(predicate) != null;
        }

        // Allows foreach loops to work on this list - O(n)
        public IEnumerator<T> GetEnumerator()
        {
            Node<T>? current = head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}