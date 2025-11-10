using System;
using System.Collections.Generic;

namespace GovernmentServices.DataStructures
{
    /// <summary>
    /// Min Heap implementation for priority queue operations
    /// Parent is always smaller than or equal to children
    /// Time Complexity: Insert O(log n), Peek O(1), ExtractMin O(log n)
    /// </summary>
    public class MinHeap<T> where T : IComparable<T>
    {
        private readonly List<T> heap;

        public int Count => heap.Count;

        public MinHeap()
        {
            heap = new List<T>();
        }

        private int GetParentIndex(int i) => (i - 1) / 2;
        private int GetLeftChildIndex(int i) => 2 * i + 1;
        private int GetRightChildIndex(int i) => 2 * i + 2;

        private bool HasParent(int i) => GetParentIndex(i) >= 0;
        private bool HasLeftChild(int i) => GetLeftChildIndex(i) < heap.Count;
        private bool HasRightChild(int i) => GetRightChildIndex(i) < heap.Count;

        private T GetParent(int i) => heap[GetParentIndex(i)];
        private T GetLeftChild(int i) => heap[GetLeftChildIndex(i)];
        private T GetRightChild(int i) => heap[GetRightChildIndex(i)];

        private void Swap(int i, int j)
        {
            T temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

        /// <summary>
        /// Moves element up the heap - O(log n)
        /// </summary>
        private void HeapifyUp(int index)
        {
            while (HasParent(index) && heap[index].CompareTo(GetParent(index)) < 0)
            {
                int parentIndex = GetParentIndex(index);
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        /// <summary>
        /// Moves element down the heap - O(log n)
        /// </summary>
        private void HeapifyDown(int index)
        {
            while (HasLeftChild(index))
            {
                int smallerChildIndex = GetLeftChildIndex(index);

                if (HasRightChild(index) && GetRightChild(index).CompareTo(GetLeftChild(index)) < 0)
                    smallerChildIndex = GetRightChildIndex(index);

                if (heap[index].CompareTo(heap[smallerChildIndex]) < 0)
                    break;

                Swap(index, smallerChildIndex);
                index = smallerChildIndex;
            }
        }

        /// <summary>
        /// Inserts a new element - O(log n)
        /// </summary>
        public void Insert(T item)
        {
            heap.Add(item);
            HeapifyUp(heap.Count - 1);
        }

        /// <summary>
        /// Gets the minimum element without removing it - O(1)
        /// </summary>
        public T Peek()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Heap is empty");
            return heap[0];
        }

        /// <summary>
        /// Gets all elements in sorted order (non-destructive) - O(n log n)
        /// </summary>
        public List<T> PeekSorted()
        {
            List<T> backup = new List<T>(heap);
            List<T> sorted = new List<T>();

            while (heap.Count > 0)
            {
                T min = heap[0];
                heap[0] = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);
                if (heap.Count > 0)
                    HeapifyDown(0);
                sorted.Add(min);
            }

            heap.Clear();
            heap.AddRange(backup);
            return sorted;
        }

        /// <summary>
        /// Gets the height of the heap - O(1)
        /// </summary>
        public int GetHeight()
        {
            if (heap.Count == 0) return 0;
            return (int)Math.Floor(Math.Log2(heap.Count)) + 1;
        }

        /// <summary>
        /// Clears all elements
        /// </summary>
        public void Clear()
        {
            heap.Clear();
        }

        /// <summary>
        /// Checks if the heap is empty
        /// </summary>
        public bool IsEmpty()
        {
            return heap.Count == 0;
        }
    }
}
