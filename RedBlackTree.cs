using System;
using System.Collections.Generic;
using System.Linq;

namespace GovernmentServices.DataStructures
{
    /// <summary>
    /// Red-Black Tree implementation using .NET's SortedSet
    /// .NET's SortedSet is internally implemented as a Red-Black tree,
    /// providing guaranteed O(log n) operations
    ///
    /// Red-Black Properties (maintained by .NET):
    /// 1. Every node is RED or BLACK
    /// 2. Root is always BLACK
    /// 3. RED nodes cannot have RED children
    /// 4. All paths from root to leaves have same number of BLACK nodes
    /// </summary>
    /// <typeparam name="T">Type that implements IComparable</typeparam>
    public class RedBlackTree<T> where T : IComparable<T>
    {
        private readonly SortedSet<T> sortedSet;

        public int Count => sortedSet.Count;

        public RedBlackTree()
        {
            sortedSet = new SortedSet<T>();
        }

        /// <summary>
        /// Inserts a new element
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
        /// Gets the height of the tree (estimated for Red-Black tree)
        /// For Red-Black tree: height ≤ 2 * log₂(n + 1)
        /// Time Complexity: O(1)
        /// </summary>
        public int GetHeight()
        {
            if (sortedSet.Count == 0)
                return 0;

            // Red-Black tree has maximum height of 2 * log₂(n + 1)
            return (int)Math.Ceiling(2 * Math.Log(sortedSet.Count + 1, 2));
        }

        /// <summary>
        /// Gets the black height of the tree (estimated)
        /// Black height is approximately log₂(n + 1)
        /// </summary>
        public int GetBlackHeight()
        {
            if (sortedSet.Count == 0)
                return 0;

            // Black height is approximately half the total height
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
