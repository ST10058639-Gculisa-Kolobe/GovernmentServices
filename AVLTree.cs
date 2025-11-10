using System;
using System.Collections.Generic;

namespace GovernmentServices.DataStructures
{
    /// <summary>
    /// Simplified AVL Tree - Self-balancing Binary Search Tree
    /// Maintains balance factor: |height(left) - height(right)| ≤ 1
    /// Ensures O(log n) time complexity for insert/search operations
    /// </summary>
    public class AVLTree<T> where T : IComparable<T>
    {
        private class Node
        {
            public T Data;
            public Node Left, Right;
            public int Height;

            public Node(T data) { Data = data; Height = 1; }
        }

        private Node root;
        public int Count { get; private set; }

        private int Height(Node node) => node?.Height ?? 0;
        private int GetBalance(Node node) => node == null ? 0 : Height(node.Left) - Height(node.Right);

        private void UpdateHeight(Node node)
        {
            if (node != null)
                node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;
        }

        private Node RotateRight(Node y)
        {
            Node x = y.Left, T2 = x.Right;
            x.Right = y; y.Left = T2;
            UpdateHeight(y); UpdateHeight(x);
            return x;
        }

        private Node RotateLeft(Node x)
        {
            Node y = x.Right, T2 = y.Left;
            y.Left = x; x.Right = T2;
            UpdateHeight(x); UpdateHeight(y);
            return y;
        }

        /// <summary>
        /// Inserts element and maintains balance - O(log n)
        /// </summary>
        public void Insert(T data)
        {
            root = InsertNode(root, data);
            Count++;
        }

        private Node InsertNode(Node node, T data)
        {
            if (node == null) return new Node(data);

            int cmp = data.CompareTo(node.Data);
            if (cmp < 0) node.Left = InsertNode(node.Left, data);
            else if (cmp > 0) node.Right = InsertNode(node.Right, data);
            else return node; // Duplicate

            UpdateHeight(node);
            int balance = GetBalance(node);

            // Left-Left
            if (balance > 1 && data.CompareTo(node.Left.Data) < 0)
                return RotateRight(node);

            // Right-Right
            if (balance < -1 && data.CompareTo(node.Right.Data) > 0)
                return RotateLeft(node);

            // Left-Right
            if (balance > 1 && data.CompareTo(node.Left.Data) > 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right-Left
            if (balance < -1 && data.CompareTo(node.Right.Data) < 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        /// <summary>
        /// Gets all elements in sorted order - O(n)
        /// </summary>
        public List<T> GetInOrder()
        {
            var result = new List<T>();
            InOrder(root, result);
            return result;
        }

        private void InOrder(Node node, List<T> result)
        {
            if (node != null)
            {
                InOrder(node.Left, result);
                result.Add(node.Data);
                InOrder(node.Right, result);
            }
        }

        /// <summary>
        /// Gets elements in a range (inclusive) - O(k + log n)
        /// </summary>
        public List<T> GetRange(T min, T max)
        {
            var result = new List<T>();
            GetRangeRecursive(root, min, max, result);
            return result;
        }

        private void GetRangeRecursive(Node node, T min, T max, List<T> result)
        {
            if (node == null) return;

            if (node.Data.CompareTo(min) > 0)
                GetRangeRecursive(node.Left, min, max, result);

            if (node.Data.CompareTo(min) >= 0 && node.Data.CompareTo(max) <= 0)
                result.Add(node.Data);

            if (node.Data.CompareTo(max) < 0)
                GetRangeRecursive(node.Right, min, max, result);
        }

        /// <summary>
        /// Gets tree height - O(1)
        /// </summary>
        public int GetHeight() => Height(root);

        /// <summary>
        /// Checks if tree is balanced - O(n)
        /// </summary>
        public bool IsBalanced() => IsBalancedRecursive(root);

        private bool IsBalancedRecursive(Node node)
        {
            if (node == null) return true;
            int balance = Math.Abs(GetBalance(node));
            return balance <= 1 && IsBalancedRecursive(node.Left) && IsBalancedRecursive(node.Right);
        }

        /// <summary>
        /// Clears all elements
        /// </summary>
        public void Clear()
        {
            root = null;
            Count = 0;
        }

        public bool IsEmpty() => root == null;
    }
}
