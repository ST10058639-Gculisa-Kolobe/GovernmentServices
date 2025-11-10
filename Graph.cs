using System;
using System.Collections.Generic;
using System.Linq;

namespace GovernmentServices.DataStructures
{
    /// <summary>
    /// Simplified Graph implementation for service request dependencies
    /// Uses adjacency list representation
    /// Supports directed graphs only (as needed for dependencies)
    /// </summary>
    public class Graph<T>
    {
        private readonly Dictionary<T, List<T>> adjacencyList;

        public int VertexCount => adjacencyList.Count;
        public int EdgeCount { get; private set; }

        public Graph(bool directed = true)
        {
            adjacencyList = new Dictionary<T, List<T>>();
            EdgeCount = 0;
        }

        /// <summary>
        /// Adds a vertex to the graph - O(1)
        /// </summary>
        public void AddVertex(T vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
                adjacencyList[vertex] = new List<T>();
        }

        /// <summary>
        /// Adds an edge to the graph - O(1)
        /// </summary>
        public void AddEdge(T from, T to)
        {
            AddVertex(from);
            AddVertex(to);
            adjacencyList[from].Add(to);
            EdgeCount++;
        }

        /// <summary>
        /// Removes an edge - O(E) where E is edges from vertex
        /// </summary>
        public bool RemoveEdge(T from, T to)
        {
            if (!adjacencyList.ContainsKey(from))
                return false;

            bool removed = adjacencyList[from].Remove(to);
            if (removed) EdgeCount--;
            return removed;
        }

        /// <summary>
        /// Gets all neighbors of a vertex - O(1)
        /// </summary>
        public List<T> GetNeighbors(T vertex)
        {
            return adjacencyList.ContainsKey(vertex)
                ? new List<T>(adjacencyList[vertex])
                : new List<T>();
        }

        /// <summary>
        /// Gets all vertices - O(1)
        /// </summary>
        public List<T> GetVertices()
        {
            return new List<T>(adjacencyList.Keys);
        }

        /// <summary>
        /// Breadth-First Search traversal - O(V + E)
        /// </summary>
        public List<T> BFS(T start)
        {
            if (!adjacencyList.ContainsKey(start))
                return new List<T>();

            var result = new List<T>();
            var visited = new HashSet<T>();
            var queue = new Queue<T>();

            visited.Add(start);
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                T vertex = queue.Dequeue();
                result.Add(vertex);

                foreach (var neighbor in adjacencyList[vertex])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Detects if the graph has a cycle - O(V + E)
        /// </summary>
        public bool HasCycle()
        {
            var visited = new HashSet<T>();
            var recursionStack = new HashSet<T>();

            foreach (var vertex in adjacencyList.Keys)
            {
                if (!visited.Contains(vertex))
                {
                    if (HasCycleDFS(vertex, visited, recursionStack))
                        return true;
                }
            }

            return false;
        }

        private bool HasCycleDFS(T vertex, HashSet<T> visited, HashSet<T> recursionStack)
        {
            visited.Add(vertex);
            recursionStack.Add(vertex);

            foreach (var neighbor in adjacencyList[vertex])
            {
                if (!visited.Contains(neighbor))
                {
                    if (HasCycleDFS(neighbor, visited, recursionStack))
                        return true;
                }
                else if (recursionStack.Contains(neighbor))
                {
                    return true; // Cycle detected
                }
            }

            recursionStack.Remove(vertex);
            return false;
        }

        /// <summary>
        /// Topological sort (for DAG - Directed Acyclic Graph) - O(V + E)
        /// Returns vertices in dependency order
        /// </summary>
        public List<T> TopologicalSort()
        {
            var visited = new HashSet<T>();
            var stack = new Stack<T>();

            foreach (var vertex in adjacencyList.Keys)
            {
                if (!visited.Contains(vertex))
                    TopologicalSortDFS(vertex, visited, stack);
            }

            return stack.ToList();
        }

        private void TopologicalSortDFS(T vertex, HashSet<T> visited, Stack<T> stack)
        {
            visited.Add(vertex);

            foreach (var neighbor in adjacencyList[vertex])
            {
                if (!visited.Contains(neighbor))
                    TopologicalSortDFS(neighbor, visited, stack);
            }

            stack.Push(vertex);
        }

        /// <summary>
        /// Gets all connected components - O(V + E)
        /// </summary>
        public List<List<T>> GetConnectedComponents()
        {
            var components = new List<List<T>>();
            var visited = new HashSet<T>();

            foreach (var vertex in adjacencyList.Keys)
            {
                if (!visited.Contains(vertex))
                {
                    var component = new List<T>();
                    DFS(vertex, visited, component);
                    components.Add(component);
                }
            }

            return components;
        }

        private void DFS(T vertex, HashSet<T> visited, List<T> component)
        {
            visited.Add(vertex);
            component.Add(vertex);

            foreach (var neighbor in adjacencyList[vertex])
            {
                if (!visited.Contains(neighbor))
                    DFS(neighbor, visited, component);
            }
        }

        /// <summary>
        /// Clears the entire graph
        /// </summary>
        public void Clear()
        {
            adjacencyList.Clear();
            EdgeCount = 0;
        }
    }
}
