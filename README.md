# Municipal Services Application - README

## Table of Contents
1. [Overview](#overview)
2. [System Requirements](#system-requirements)
3. [Installation & Compilation](#installation--compilation)
4. [Running the Application](#running-the-application)
5. [Application Features](#application-features)
6. [Data Structures Implementation](#data-structures-implementation)
7. [Service Request Status Feature](#service-request-status-feature)
8. [Database Schema](#database-schema)
9. [Architecture Overview](#architecture-overview)
10. [Troubleshooting](#troubleshooting)

---

## Overview

The **Municipal Services Application** is a comprehensive desktop application built using Windows Forms (.NET Framework 4.8) that provides citizens with access to essential municipal services. The application demonstrates advanced data structure implementations for efficient data management and retrieval.

**Version:** 3.0 - Service Request Status Edition

**Key Features:**
- Issue Reporting System
- Local Events and Announcements
- Service Request Status Tracking (NEW)
- User Account Management
- About & Information Section

---

## System Requirements

### Minimum Requirements:
- **Operating System:** Windows 10 or later (64-bit)
- **.NET Framework:** 4.8 or later
- **RAM:** 4 GB minimum, 8 GB recommended
- **Disk Space:** 100 MB for application + database
- **Display:** 1280x720 minimum resolution

### Development Requirements:
- **IDE:** Visual Studio 2019 or later
- **SDK:** .NET Framework 4.8 SDK
- **NuGet Packages:**
  - Microsoft.Data.Sqlite (v9.0.8)
  - SQLitePCLRaw.bundle_e_sqlite3 (v2.1.10)
  - SQLitePCLRaw.core (v2.1.10)
  - SQLitePCLRaw.provider.e_sqlite3 (v2.1.10)

---

## Installation & Compilation

### Method 1: Using Visual Studio

1. **Clone or Download the Project**
   ```
   Download the project folder to your local machine
   ```

2. **Open the Solution**
   - Launch Visual Studio 2019 or later
   - Open `GovernmentServices.sln`

3. **Restore NuGet Packages**
   - Right-click on the solution in Solution Explorer
   - Select "Restore NuGet Packages"
   - Wait for all packages to download

4. **Build the Solution**
   - Select `Build > Build Solution` (or press `Ctrl+Shift+B`)
   - Ensure there are no compilation errors
   - The build output will be in `bin\Debug` or `bin\Release`

### Method 2: Using MSBuild (Command Line)

1. **Open Developer Command Prompt for Visual Studio**

2. **Navigate to Project Directory**
   ```cmd
   cd path\to\GovernmentServices
   ```

3. **Restore NuGet Packages**
   ```cmd
   nuget restore GovernmentServices.sln
   ```

4. **Build the Project**
   ```cmd
   msbuild GovernmentServices.sln /p:Configuration=Release
   ```

5. **Executable Location**
   ```
   bin\Release\GovernmentServices.exe
   ```

---

## Running the Application

### First Time Launch

1. **Locate the Executable**
   - Navigate to `bin\Debug` or `bin\Release`
   - Find `GovernmentServices.exe`

2. **Launch the Application**
   - Double-click `GovernmentServices.exe`
   - The application will create a database on first run at:
     ```
     %APPDATA%\GovernmentServices\government.db
     ```

3. **Welcome Screen**
   - You'll see the main menu with 5 options:
     - Report Issues
     - Local Events and Announcements
     - Service Request Status
     - About
     - Account

### Database Location

The application stores all data in an SQLite database at:
```
C:\Users\[YourUsername]\AppData\Roaming\GovernmentServices\government.db
```

To reset the application, simply delete this folder.

---

## Application Features

### 1. Report Issues (PageOne)

**Purpose:** Submit municipal issues for resolution

**Features:**
- Location input field
- Category selection (Sanitation, Roads, Utilities, Safety, Other)
- Rich text description
- Multiple file attachments (images, documents)
- Progress indicator during submission
- Persistent storage in SQLite database

**How to Use:**
1. Click "Report Issues" from the main menu
2. Enter the location of the issue
3. Select appropriate category
4. Provide detailed description
5. (Optional) Attach images or documents
6. Click "Submit Issue"

### 2. Local Events and Announcements (PageTwo)

**Purpose:** Browse and search community events

**Features:**
- 15 pre-loaded sample events
- Advanced search functionality
- Multiple filter options (keyword, category, date range, priority)
- Event details display
- Data structure statistics view
- Recent events queue
- High-priority events display

**Data Structures Used:**
- `SortedDictionary<DateTime, List<Event>>` - Events by date
- `Dictionary<string, List<Event>>` - Events by category
- `HashSet<string>` - Unique categories
- `Queue<Event>` - Recently added events
- `Stack<Event>` - View history
- `SortedDictionary<Priority, Queue<Event>>` - Priority queues

**How to Use:**
1. Click "Local Events and Announcements"
2. Browse the event list
3. Use search filters to narrow results
4. Click on an event to view details
5. Double-click for detailed popup
6. Click "Statistics" to view data structure usage

### 3. Service Request Status (PageThree) ⭐ NEW

**Purpose:** Track and manage service requests using advanced data structures

**Features:**
- Display all submitted service requests
- Unique request identifiers (e.g., SR-20241201-ABC12345)
- Status tracking (Submitted, Pending, InProgress, OnHold, Resolved, Closed)
- Priority levels (Low, Normal, High, Urgent, Critical)
- Advanced filtering by status, category, priority
- Full-text search functionality
- Request dependency management
- Update request status with notes
- Comprehensive statistics view
- Test data generation

**How to Use:**
1. Click "Service Request Status"
2. Click "Add Test Data" to generate sample requests
3. Use filters to narrow down requests
4. Select a request to view details
5. Click "Update Status" to modify request status
6. Click "View Statistics" to see data structure metrics
7. Double-click a request for detailed popup

### 4. About (PageFour)

**Purpose:** Information about the application

**Content:**
- Application version and description
- Feature overview
- Data structures explanation
- Technology stack details
- Development credits

### 5. Account (PageFive)

**Purpose:** Manage user profile information

**Features:**
- Save/load user profile
- Name, email, phone, address fields
- Local database storage
- Account statistics

**How to Use:**
1. Click "Account"
2. Enter your information
3. Click "Save Changes"
4. Use "Load Profile" to retrieve saved data

---

## Data Structures Implementation

### Overview

The Service Request Status feature implements **7 advanced data structures** for efficient data management:

### 1. Binary Search Tree (BST)

**File:** `BinarySearchTree.cs`

**Purpose:** Basic tree structure for storing service requests sorted by submission date.

**Key Operations:**
- `Insert(T data)` - O(log n) average, O(n) worst
- `Search(T data)` - O(log n) average
- `Delete(T data)` - O(log n) average
- `GetInOrder()` - O(n) - Returns sorted list
- `GetHeight()` - O(n)

**Implementation Details:**
- Nodes contain service request objects
- Comparison based on `IComparable<T>`
- In-order traversal returns chronologically sorted requests

**Usage Example:**
```csharp
BinarySearchTree<ServiceRequest> bst = new BinarySearchTree<ServiceRequest>();
bst.Insert(request);
List<ServiceRequest> sortedRequests = bst.GetInOrder();
```

**Efficiency Contribution:**
- Provides O(log n) average case search performance
- Maintains sorted order without explicit sorting
- Useful for range queries by date

---

### 2. AVL Tree

**File:** `AVLTree.cs`

**Purpose:** Self-balancing binary search tree ensuring O(log n) operations even in worst case.

**Key Operations:**
- `Insert(T data)` - O(log n) guaranteed
- `Delete(T data)` - O(log n) guaranteed
- `Search(T data)` - O(log n) guaranteed
- `GetInOrder()` - O(n)
- `IsBalanced()` - Verification method

**Implementation Details:**
- Maintains balance factor: |height(left) - height(right)| ≤ 1
- Four rotation types: Left-Left, Right-Right, Left-Right, Right-Left
- Height information stored in each node
- Automatic rebalancing after insertions/deletions

**Balancing Process:**
```
1. Insert node using standard BST insertion
2. Update heights of affected nodes
3. Calculate balance factor
4. Perform appropriate rotation if |balance| > 1
```

**Usage Example:**
```csharp
AVLTree<ServiceRequest> avl = new AVLTree<ServiceRequest>();
avl.Insert(request);
bool isBalanced = avl.IsBalanced(); // Always true
```

**Efficiency Contribution:**
- **Guaranteed** O(log n) performance
- No degradation to O(n) in worst case
- Optimal for frequently accessed data
- Better worst-case performance than regular BST

**Real-World Benefit:**
With 1000 service requests:
- BST worst case: 1000 comparisons
- AVL worst case: 10 comparisons (log₂ 1000 ≈ 10)

---

### 3. Red-Black Tree

**File:** `RedBlackTree.cs`

**Purpose:** Alternative self-balancing tree with different balancing strategy.

**Key Operations:**
- `Insert(T data)` - O(log n)
- `Search(T data)` - O(log n)
- `GetInOrder()` - O(n)
- `GetBlackHeight()` - Verification method

**Implementation Details:**
- Every node is RED or BLACK
- Root is always BLACK
- RED nodes cannot have RED children
- All paths from root to leaves have same number of BLACK nodes
- Uses NIL sentinel nodes
- Fewer rotations than AVL tree

**Red-Black Properties:**
1. Every node is colored RED or BLACK
2. Root node is BLACK
3. All leaf nodes (NIL) are BLACK
4. RED nodes have BLACK children
5. Every path from root to leaf has same BLACK node count

**Usage Example:**
```csharp
RedBlackTree<ServiceRequest> rbTree = new RedBlackTree<ServiceRequest>();
rbTree.Insert(request);
int blackHeight = rbTree.GetBlackHeight();
```

**Efficiency Contribution:**
- Faster insertion than AVL (fewer rotations)
- Slightly slower search than AVL
- Good for insert-heavy workloads
- Used in Java TreeMap and C++ map

**Comparison: AVL vs Red-Black**

| Operation | AVL Tree | Red-Black Tree |
|-----------|----------|----------------|
| Search | Faster (more balanced) | Slightly slower |
| Insert | Slower (more rotations) | Faster |
| Delete | Slower | Faster |
| Balance | Stricter | Looser |
| Use Case | Read-heavy | Insert-heavy |

---

### 4. Min Heap (Priority Queue)

**File:** `MinHeap.cs`

**Purpose:** Manage service requests by priority, highest priority first.

**Key Operations:**
- `Insert(T item)` - O(log n)
- `ExtractMin()` - O(log n)
- `Peek()` - O(1)
- `BuildHeap()` - O(n)
- `Remove(T item)` - O(n)

**Implementation Details:**
- Array-based implementation using `List<T>`
- Min-heap property: Parent ≤ Children
- Parent index: `(i-1)/2`
- Left child: `2i+1`, Right child: `2i+2`
- Heapify-up after insertion
- Heapify-down after extraction

**Heap Structure:**
```
         [5]
        /   \
      [10]  [15]
      / \    / \
   [20][25][20][30]
```

**Priority Wrapper:**
```csharp
public class PriorityRequestWrapper : IComparable<PriorityRequestWrapper>
{
    public ServiceRequest Request { get; set; }

    // Higher priority values come first (Critical > Low)
    public int CompareTo(PriorityRequestWrapper other)
    {
        return other.Request.Priority.CompareTo(Request.Priority);
    }
}
```

**Usage Example:**
```csharp
MinHeap<PriorityRequestWrapper> heap = new MinHeap<PriorityRequestWrapper>();
heap.Insert(new PriorityRequestWrapper(request));
ServiceRequest highestPriority = heap.Peek().Request;
```

**Efficiency Contribution:**
- O(1) access to highest priority request
- Efficient priority-based processing
- Dynamic priority adjustment
- Used in task scheduling

**Real-World Application:**
Emergency services would process:
1. Critical requests (water main break)
2. Urgent requests (power outage)
3. High requests (road damage)
4. Normal requests (street cleaning)
5. Low requests (park maintenance)

---

### 5. Graph

**File:** `Graph.cs`

**Purpose:** Manage dependencies between service requests.

**Key Operations:**
- `AddVertex(T vertex)` - O(1)
- `AddEdge(T from, T to)` - O(1)
- `DFS(T start)` - O(V + E)
- `BFS(T start)` - O(V + E)
- `ShortestPath(T start, T end)` - O(V + E)
- `TopologicalSort()` - O(V + E)
- `HasCycle()` - O(V + E)
- `MinimumSpanningTreeKruskal()` - O(E log E)

**Implementation Details:**
- Adjacency list representation using `Dictionary<T, List<Edge<T>>>`
- Supports both directed and undirected graphs
- Weighted edges for priority/cost
- Generic implementation for any vertex type

**Graph Algorithms:**

#### Depth-First Search (DFS)
- Explores as deep as possible before backtracking
- Uses recursion
- Applications: Dependency resolution, cycle detection

#### Breadth-First Search (BFS)
- Explores level by level
- Uses queue
- Applications: Shortest path, connected components

#### Topological Sort
- Linear ordering of vertices
- Only for Directed Acyclic Graphs (DAG)
- Applications: Task scheduling, build systems

#### Cycle Detection
- Detects circular dependencies
- Prevents infinite loops
- Critical for dependency management

#### Minimum Spanning Tree (Kruskal's Algorithm)
- Finds minimum-cost tree connecting all vertices
- Uses Union-Find data structure
- Applications: Network optimization

**Usage Example:**
```csharp
Graph<string> graph = new Graph<string>(directed: true);

// Add requests
graph.AddVertex("SR-001");
graph.AddVertex("SR-002");

// SR-002 depends on SR-001
graph.AddEdge("SR-001", "SR-002");

// Check for circular dependencies
bool hasCycle = graph.HasCycle();

// Get processing order
List<string> order = graph.TopologicalSort();

// Find dependencies
List<string> dependencies = graph.DFS("SR-001");
```

**Efficiency Contribution:**
- Models complex relationships between requests
- Prevents circular dependencies (e.g., A depends on B, B depends on A)
- Determines optimal processing order
- Identifies related request groups

**Real-World Example:**
```
Request SR-001: Fix water pipe
Request SR-002: Repair road damage (depends on SR-001)
Request SR-003: Repaint road lines (depends on SR-002)

Processing Order: SR-001 → SR-002 → SR-003
```

The graph ensures:
1. Water pipe fixed before road repair
2. Road repaired before line painting
3. No circular dependencies
4. Optimal resource allocation

---

### 6. Hash Tables (Dictionary)

**Built-in Type:** `Dictionary<TKey, TValue>`

**Purpose:** O(1) lookup of service requests by ID, category grouping, status grouping.

**Usage in Application:**
```csharp
// O(1) lookup by request ID
Dictionary<string, ServiceRequest> requestById;

// Group by category
Dictionary<string, List<ServiceRequest>> requestsByCategory;

// Group by status
Dictionary<RequestStatus, List<ServiceRequest>> requestsByStatus;
```

**Key Operations:**
- `Add(key, value)` - O(1) average
- `Get(key)` - O(1) average
- `Remove(key)` - O(1) average
- `ContainsKey(key)` - O(1) average

**Efficiency Contribution:**
- Instant access to any request by ID
- Fast filtering by category or status
- No search required

---

### 7. Priority Queue Implementation

**Combination of:** Min Heap + Custom Comparator

**Purpose:** Automatic ordering by multiple criteria.

**Implementation:**
```csharp
public class PriorityRequestWrapper : IComparable<PriorityRequestWrapper>
{
    public int CompareTo(PriorityRequestWrapper other)
    {
        // Primary: Priority (Critical first)
        int priorityCompare = other.Request.Priority.CompareTo(Request.Priority);
        if (priorityCompare != 0) return priorityCompare;

        // Secondary: Age (Older first)
        return Request.SubmittedDate.CompareTo(other.Request.SubmittedDate);
    }
}
```

**Usage:**
```csharp
MinHeap<PriorityRequestWrapper> priorityQueue = new MinHeap<PriorityRequestWrapper>();

// Automatically maintains order
var topRequest = priorityQueue.Peek(); // Always highest priority + oldest
```

---

## Service Request Status Feature

### ServiceRequestManager Class

**File:** `ServiceRequestManager.cs`

**Purpose:** Unified manager coordinating all data structures.

**Architecture:**
```
ServiceRequestManager
├── BinarySearchTree<ServiceRequest> bst
├── AVLTree<ServiceRequest> avlTree
├── RedBlackTree<ServiceRequest> rbTree
├── MinHeap<PriorityRequestWrapper> priorityHeap
├── Graph<string> dependencyGraph
├── Dictionary<string, ServiceRequest> requestById
├── Dictionary<string, List<ServiceRequest>> requestsByCategory
└── Dictionary<RequestStatus, List<ServiceRequest>> requestsByStatus
```

### Key Methods

#### AddRequest(ServiceRequest request)
```csharp
public void AddRequest(ServiceRequest request)
{
    // O(log n) - Add to trees
    bst.Insert(request);
    avlTree.Insert(request);
    rbTree.Insert(request);

    // O(log n) - Add to heap
    priorityHeap.Insert(new PriorityRequestWrapper(request));

    // O(1) - Add to graph
    dependencyGraph.AddVertex(request.RequestId);

    // O(1) - Add to dictionaries
    requestById[request.RequestId] = request;
    requestsByCategory[request.Category].Add(request);
    requestsByStatus[request.Status].Add(request);
}
```

**Time Complexity:** O(log n) due to tree operations

#### GetAllRequestsSorted()
```csharp
public List<ServiceRequest> GetAllRequestsSorted()
{
    return bst.GetInOrder(); // O(n)
}
```

Uses BST in-order traversal for chronologically sorted list.

#### GetHighestPriorityRequest()
```csharp
public ServiceRequest GetHighestPriorityRequest()
{
    return priorityHeap.Peek().Request; // O(1)
}
```

Instant access to most important request.

#### GetRequestDependencies(string requestId)
```csharp
public List<string> GetRequestDependencies(string requestId)
{
    return dependencyGraph.BFS(requestId); // O(V + E)
}
```

Uses BFS to find all related requests.

#### WouldCreateCircularDependency(string fromId, string toId)
```csharp
public bool WouldCreateCircularDependency(string fromId, string toId)
{
    dependencyGraph.AddEdge(fromId, toId);
    bool hasCycle = dependencyGraph.HasCycle();
    dependencyGraph.RemoveEdge(fromId, toId);
    return hasCycle; // O(V + E)
}
```

Prevents invalid dependencies.

### Statistics Method

```csharp
public Dictionary<string, object> GetStatistics()
{
    return new Dictionary<string, object>
    {
        ["Total Requests"] = requestById.Count,
        ["BST Height"] = bst.GetHeight(),
        ["AVL Height"] = avlTree.GetHeight(),
        ["AVL Is Balanced"] = avlTree.IsBalanced(),
        ["Red-Black Height"] = rbTree.GetHeight(),
        ["Red-Black Black Height"] = rbTree.GetBlackHeight(),
        ["Priority Heap Size"] = priorityHeap.Count,
        ["Heap Height"] = priorityHeap.GetHeight(),
        ["Graph Vertices"] = dependencyGraph.VertexCount,
        ["Graph Edges"] = dependencyGraph.EdgeCount,
        // ... status breakdowns
    };
}
```

Provides real-time metrics on data structure performance.

---

## Database Schema

### Tables

#### 1. issues
```sql
CREATE TABLE issues (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    location TEXT NOT NULL,
    category TEXT NOT NULL,
    description TEXT NOT NULL,
    media TEXT,  -- JSON array of file paths
    created_at TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ','now'))
);
```

#### 2. service_requests
```sql
CREATE TABLE service_requests (
    request_id TEXT PRIMARY KEY,
    title TEXT NOT NULL,
    description TEXT NOT NULL,
    category TEXT NOT NULL,
    location TEXT NOT NULL,
    status INTEGER NOT NULL,
    priority INTEGER NOT NULL,
    submitted_date TEXT NOT NULL,
    last_updated TEXT,
    resolved_date TEXT,
    submitted_by TEXT NOT NULL,
    assigned_to TEXT,
    updates TEXT,  -- JSON array of status updates
    depends_on TEXT  -- JSON array of dependency IDs
);
```

#### 3. user_profile (key-value)
```sql
CREATE TABLE user_profile (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL
);
```

### Database File Location
```
%APPDATA%\GovernmentServices\government.db
```

---

## Architecture Overview

### Project Structure
```
GovernmentServices/
├── Program.cs                     # Entry point
├── Form1.cs                       # Main window container
├── StartPage.cs                   # Navigation hub
├── PageBase.cs                    # Abstract base for pages
├── PageOne.cs                     # Report Issues
├── PageTwo.cs                     # Events & Announcements
├── PageThree.cs                   # Service Request Status
├── PageFour.cs                    # About
├── PageFive.cs                    # Account
├── EventModels.cs                 # Event data models
├── EventManager.cs                # Event data structures
├── ServiceRequestModels.cs        # Service request models
├── ServiceRequestManager.cs       # Service request data structures
├── BinarySearchTree.cs            # BST implementation
├── AVLTree.cs                     # AVL Tree implementation
├── RedBlackTree.cs                # Red-Black Tree implementation
├── MinHeap.cs                     # Heap implementation
├── Graph.cs                       # Graph implementation
├── Class1.cs (DBHandler)          # Database operations
└── GovernmentServices.csproj      # Project configuration
```

### Design Patterns

#### 1. Singleton Pattern
- Database connection maintained as singleton
- One instance per application lifetime

#### 2. Observer Pattern
- Page navigation uses event-driven architecture
- `NavigateRequested` and `BackRequested` events

#### 3. Factory Pattern
- Button creation in StartPage
- UserControl instantiation in Form1

#### 4. Template Method Pattern
- PageBase provides common structure
- Derived pages implement specific content

---

## Troubleshooting

### Common Issues

#### Issue: Application won't start
**Solution:**
1. Ensure .NET Framework 4.8 is installed
2. Check if antivirus is blocking the executable
3. Run as Administrator
4. Check Windows Event Viewer for errors

#### Issue: Database errors
**Solution:**
1. Delete `%APPDATA%\GovernmentServices` folder
2. Restart application
3. Database will be recreated automatically

#### Issue: No service requests showing
**Solution:**
1. Click "Add Test Data" to generate sample requests
2. Check if database file exists
3. Verify read/write permissions on AppData folder

#### Issue: Search not working
**Solution:**
1. Check if you have any service requests added
2. Ensure search term is not empty
3. Try clearing filters

#### Issue: Build errors
**Solution:**
1. Clean solution (`Build > Clean Solution`)
2. Restore NuGet packages
3. Rebuild solution (`Build > Rebuild Solution`)
4. Check if all .cs files are included in project

### Performance Optimization

For large datasets (> 10,000 requests):

1. **Use AVL Tree** for frequent searches
   - Guaranteed O(log n) performance

2. **Use Hash Table** for ID lookups
   - O(1) access time

3. **Use Min Heap** for priority processing
   - O(1) peek time

4. **Batch Updates**
   - Update database in batches
   - Reduces I/O overhead

### Debug Mode

To enable debug information:

1. Open `App.config`
2. Add `<add key="DebugMode" value="true" />`
3. Check debug output in Visual Studio Output window

---

## Support & Contact

For technical support or questions:

1. Check this README first
2. Review the code comments in source files
3. Contact your course instructor
4. Submit issues via the course platform

---

## License

This application is developed for educational purposes as part of the PROG7312 course.

© 2024 - South African University Project

---

## Changelog

### Version 3.0 (Current)
- ✨ Added Service Request Status feature
- ✨ Implemented 7 advanced data structures
- ✨ Added About page
- ✨ Added Account page
- ✨ Added dependency management
- ✨ Added comprehensive statistics
- ✨ Added test data generation
- 📊 Improved performance with balanced trees

### Version 2.0
- Added Local Events and Announcements feature
- Implemented multiple data structures (Dictionary, Queue, Stack)
- Added search and filtering capabilities

### Version 1.0
- Initial release
- Report Issues functionality
- SQLite database integration

---

## Acknowledgments

Special thanks to:
- Microsoft for .NET Framework and Visual Studio
- SQLite team for the excellent embedded database
- All contributors and testers

---

**End of README**
