using System;
using System.Collections.Generic;
using System.Linq;
using GovernmentServices.DataStructures;

namespace GovernmentServices
{
    /// <summary>
    /// Manages service requests using multiple advanced data structures
    /// Demonstrates efficient organization, retrieval, and relationship management
    /// </summary>
    public class ServiceRequestManager
    {
        // Tree structures for efficient searching and sorting
        private BinarySearchTree<ServiceRequest> bst;           // Basic BST by submission date
        private AVLTree<ServiceRequest> avlTree;                // Self-balancing tree for O(log n) operations
        private RedBlackTree<ServiceRequest> rbTree;            // Alternative balanced tree

        // Heap for priority-based operations
        private MinHeap<PriorityRequestWrapper> priorityHeap;   // Min heap for highest priority first

        // Graph for dependency management
        private Graph<string> dependencyGraph;                  // Tracks request dependencies

        // Dictionary for O(1) lookup by ID
        private Dictionary<string, ServiceRequest> requestById;

        // Dictionary for categorization
        private Dictionary<string, List<ServiceRequest>> requestsByCategory;
        private Dictionary<RequestStatus, List<ServiceRequest>> requestsByStatus;

        // Statistics
        public int TotalRequests => requestById.Count;

        public ServiceRequestManager()
        {
            bst = new BinarySearchTree<ServiceRequest>();
            avlTree = new AVLTree<ServiceRequest>();
            rbTree = new RedBlackTree<ServiceRequest>();
            priorityHeap = new MinHeap<PriorityRequestWrapper>();
            dependencyGraph = new Graph<string>(directed: true);
            requestById = new Dictionary<string, ServiceRequest>();
            requestsByCategory = new Dictionary<string, List<ServiceRequest>>();
            requestsByStatus = new Dictionary<RequestStatus, List<ServiceRequest>>();

            // Initialize status dictionary
            foreach (RequestStatus status in Enum.GetValues(typeof(RequestStatus)))
            {
                requestsByStatus[status] = new List<ServiceRequest>();
            }
        }

        /// <summary>
        /// Adds a new service request to all data structures
        /// Time Complexity: O(log n) for trees + O(log n) for heap = O(log n)
        /// </summary>
        public void AddRequest(ServiceRequest request)
        {
            if (request == null || requestById.ContainsKey(request.RequestId))
                return;

            // Add to all tree structures
            bst.Insert(request);
            avlTree.Insert(request);
            rbTree.Insert(request);

            // Add to priority heap
            priorityHeap.Insert(new PriorityRequestWrapper(request));

            // Add to graph
            dependencyGraph.AddVertex(request.RequestId);

            // Add dependencies if any
            foreach (var depId in request.DependsOn)
            {
                if (requestById.ContainsKey(depId))
                {
                    dependencyGraph.AddEdge(depId, request.RequestId);
                }
            }

            // Add to dictionaries
            requestById[request.RequestId] = request;

            if (!requestsByCategory.ContainsKey(request.Category))
                requestsByCategory[request.Category] = new List<ServiceRequest>();
            requestsByCategory[request.Category].Add(request);

            requestsByStatus[request.Status].Add(request);
        }

        /// <summary>
        /// Gets a request by ID
        /// Time Complexity: O(1)
        /// </summary>
        public ServiceRequest GetRequestById(string requestId)
        {
            return requestById.ContainsKey(requestId) ? requestById[requestId] : null;
        }

        /// <summary>
        /// Gets all requests sorted by submission date (using BST in-order traversal)
        /// Time Complexity: O(n)
        /// </summary>
        public List<ServiceRequest> GetAllRequestsSorted()
        {
            return bst.GetInOrder();
        }

        /// <summary>
        /// Gets all requests sorted by submission date (using AVL tree)
        /// Time Complexity: O(n)
        /// Demonstrates balanced tree efficiency
        /// </summary>
        public List<ServiceRequest> GetAllRequestsAVL()
        {
            return avlTree.GetInOrder();
        }

        /// <summary>
        /// Gets all requests sorted by submission date (using Red-Black tree)
        /// Time Complexity: O(n)
        /// </summary>
        public List<ServiceRequest> GetAllRequestsRB()
        {
            return rbTree.GetInOrder();
        }

        /// <summary>
        /// Gets requests by category
        /// Time Complexity: O(1)
        /// </summary>
        public List<ServiceRequest> GetRequestsByCategory(string category)
        {
            return requestsByCategory.ContainsKey(category)
                ? new List<ServiceRequest>(requestsByCategory[category])
                : new List<ServiceRequest>();
        }

        /// <summary>
        /// Gets requests by status
        /// Time Complexity: O(1)
        /// </summary>
        public List<ServiceRequest> GetRequestsByStatus(RequestStatus status)
        {
            return new List<ServiceRequest>(requestsByStatus[status]);
        }

        /// <summary>
        /// Gets the highest priority request (using heap)
        /// Time Complexity: O(1)
        /// </summary>
        public ServiceRequest GetHighestPriorityRequest()
        {
            if (priorityHeap.IsEmpty())
                return null;

            return priorityHeap.Peek().Request;
        }

        /// <summary>
        /// Gets top N priority requests
        /// Time Complexity: O(n log n)
        /// </summary>
        public List<ServiceRequest> GetTopPriorityRequests(int count)
        {
            var sortedRequests = priorityHeap.PeekSorted();
            return sortedRequests.Take(count).Select(w => w.Request).ToList();
        }

        /// <summary>
        /// Gets all unresolved requests (not Resolved or Closed)
        /// </summary>
        public List<ServiceRequest> GetUnresolvedRequests()
        {
            List<ServiceRequest> unresolved = new List<ServiceRequest>();

            foreach (var request in requestById.Values)
            {
                if (request.Status != RequestStatus.Resolved && request.Status != RequestStatus.Closed)
                {
                    unresolved.Add(request);
                }
            }

            return unresolved;
        }

        /// <summary>
        /// Gets overdue requests (more than 30 days old and not resolved)
        /// </summary>
        public List<ServiceRequest> GetOverdueRequests()
        {
            return requestById.Values.Where(r => r.IsOverdue()).ToList();
        }

        /// <summary>
        /// Gets requests in a date range using AVL tree range query
        /// Time Complexity: O(k + log n) where k is number of results
        /// </summary>
        public List<ServiceRequest> GetRequestsInDateRange(DateTime startDate, DateTime endDate)
        {
            var dummyStart = new ServiceRequest { SubmittedDate = startDate };
            var dummyEnd = new ServiceRequest { SubmittedDate = endDate };

            return avlTree.GetRange(dummyStart, dummyEnd);
        }

        /// <summary>
        /// Gets all dependencies of a request (using graph BFS)
        /// Time Complexity: O(V + E)
        /// </summary>
        public List<string> GetRequestDependencies(string requestId)
        {
            if (!dependencyGraph.GetVertices().Contains(requestId))
                return new List<string>();

            return dependencyGraph.BFS(requestId);
        }

        /// <summary>
        /// Gets requests that depend on this request
        /// </summary>
        public List<string> GetDependentRequests(string requestId)
        {
            return dependencyGraph.GetNeighbors(requestId);
        }

        /// <summary>
        /// Checks if resolving a request will create a circular dependency
        /// Time Complexity: O(V + E)
        /// </summary>
        public bool WouldCreateCircularDependency(string fromId, string toId)
        {
            // Temporarily add edge and check for cycle
            if (!requestById.ContainsKey(fromId) || !requestById.ContainsKey(toId))
                return false;

            dependencyGraph.AddEdge(fromId, toId);
            bool hasCycle = dependencyGraph.HasCycle();
            dependencyGraph.RemoveEdge(fromId, toId);

            return hasCycle;
        }

        /// <summary>
        /// Gets the optimal processing order using topological sort
        /// Time Complexity: O(V + E)
        /// </summary>
        public List<string> GetProcessingOrder()
        {
            try
            {
                return dependencyGraph.TopologicalSort();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Gets connected request groups (related requests)
        /// Time Complexity: O(V + E)
        /// </summary>
        public List<List<string>> GetRelatedRequestGroups()
        {
            return dependencyGraph.GetConnectedComponents();
        }

        /// <summary>
        /// Updates request status
        /// </summary>
        public void UpdateRequestStatus(string requestId, RequestStatus newStatus, string updateMessage)
        {
            var request = GetRequestById(requestId);
            if (request == null)
                return;

            // Remove from old status list
            requestsByStatus[request.Status].Remove(request);

            // Update status
            request.Status = newStatus;
            request.AddUpdate(updateMessage);

            if (newStatus == RequestStatus.Resolved)
            {
                request.ResolvedDate = DateTime.Now;
            }

            // Add to new status list
            requestsByStatus[newStatus].Add(request);
        }

        /// <summary>
        /// Adds a dependency between requests
        /// </summary>
        public bool AddDependency(string fromId, string toId)
        {
            if (!requestById.ContainsKey(fromId) || !requestById.ContainsKey(toId))
                return false;

            if (WouldCreateCircularDependency(fromId, toId))
                return false;

            dependencyGraph.AddEdge(fromId, toId);
            requestById[toId].DependsOn.Add(fromId);

            return true;
        }

        /// <summary>
        /// Searches requests by keyword in title or description
        /// Time Complexity: O(n)
        /// </summary>
        public List<ServiceRequest> SearchRequests(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAllRequestsSorted();

            keyword = keyword.ToLower();

            return requestById.Values
                .Where(r => r.Title.ToLower().Contains(keyword) ||
                           r.Description.ToLower().Contains(keyword) ||
                           r.RequestId.ToLower().Contains(keyword))
                .OrderBy(r => r.SubmittedDate)
                .ToList();
        }

        /// <summary>
        /// Gets all unique categories
        /// </summary>
        public List<string> GetCategories()
        {
            return new List<string>(requestsByCategory.Keys);
        }

        /// <summary>
        /// Gets statistics about the data structures
        /// </summary>
        public Dictionary<string, object> GetStatistics()
        {
            var stats = new Dictionary<string, object>
            {
                ["Total Requests"] = TotalRequests,
                ["BST Height"] = bst.GetHeight(),
                ["AVL Height"] = avlTree.GetHeight(),
                ["AVL Is Balanced"] = avlTree.IsBalanced(),
                ["Red-Black Height"] = rbTree.GetHeight(),
                ["Red-Black Black Height"] = rbTree.GetBlackHeight(),
                ["Priority Heap Size"] = priorityHeap.Count,
                ["Heap Height"] = priorityHeap.GetHeight(),
                ["Graph Vertices"] = dependencyGraph.VertexCount,
                ["Graph Edges"] = dependencyGraph.EdgeCount,
                ["Categories"] = requestsByCategory.Count,
                ["Connected Components"] = GetRelatedRequestGroups().Count
            };

            // Status breakdown
            foreach (RequestStatus status in Enum.GetValues(typeof(RequestStatus)))
            {
                stats[$"Status: {status}"] = requestsByStatus[status].Count;
            }

            return stats;
        }

        /// <summary>
        /// Clears all data structures
        /// </summary>
        public void Clear()
        {
            bst.Clear();
            avlTree.Clear();
            rbTree.Clear();
            priorityHeap.Clear();
            dependencyGraph.Clear();
            requestById.Clear();
            requestsByCategory.Clear();

            foreach (var status in requestsByStatus.Keys.ToList())
            {
                requestsByStatus[status].Clear();
            }
        }
    }

    /// <summary>
    /// Wrapper class for ServiceRequest to use in priority heap
    /// Lower priority value = higher priority (Critical=4 comes before Low=0)
    /// </summary>
    public class PriorityRequestWrapper : IComparable<PriorityRequestWrapper>
    {
        public ServiceRequest Request { get; set; }

        public PriorityRequestWrapper(ServiceRequest request)
        {
            Request = request;
        }

        public int CompareTo(PriorityRequestWrapper other)
        {
            if (other == null) return 1;

            // Higher priority value should come first (reverse comparison)
            int priorityCompare = other.Request.Priority.CompareTo(Request.Priority);

            if (priorityCompare != 0)
                return priorityCompare;

            // If same priority, older requests come first
            return Request.SubmittedDate.CompareTo(other.Request.SubmittedDate);
        }
    }
}
