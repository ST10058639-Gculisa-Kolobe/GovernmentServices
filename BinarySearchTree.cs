using System;
using System.Collections.Generic;
using System.Linq;

namespace GovernmentServices.DataStructures
{
    /// <summary>
    /// Binary Search Tree implementation using .NET's SortedSet
    /// Provides O(log n) operations for this project's needs
    /// Note: .NET's SortedSet is implemented as a Red-Black tree internally
    /// </summary>
    /// <typeparam name="T">Type that implements IComparable</typeparam>
    public class BinarySearchTree<T> where T : IComparable<T>
    {
        private readonly SortedSet<T> sortedSet;

        public int Count => sortedSet.Count;

        public BinarySearchTree()
        {
            sortedSet = new SortedSet<T>();
        }

        /// <summary>
        /// Inserts a new element into the BST
        /// Time Complexity: O(log n)
        /// </summary>
        public void Insert(T data)
        {
            sortedSet.Add(data);
        }

        /// <summary>
        /// Gets all elements in sorted order (In-order traversal)
        /// Time Complexity: O(n)
        /// </summary>
        public List<T> GetInOrder()
        {
            return sortedSet.ToList();
        }

        /// <summary>
        /// Gets the height of the tree (estimated for balanced tree)
        /// Time Complexity: O(1)
        /// </summary>
        public int GetHeight()
        {
            if (sortedSet.Count == 0)
                return 0;

            // For a balanced tree, height ≈ log₂(n)
            return (int)Math.Ceiling(Math.Log(sortedSet.Count + 1, 2));
        }

        /// <summary>
        /// Clears all elements from the tree
        /// </summary>
        public void Clear()
        {
            sortedSet.Clear();
        }

        /// <summary>
        /// Checks if the tree is empty
        /// </summary>
        public bool IsEmpty()
        {
            return sortedSet.Count == 0;
        }
    }
}
