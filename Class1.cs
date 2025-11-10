using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GovernmentServices
{
    internal class DBHandler : SqliteConnection
    {
        private static readonly Regex TableName = new(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

        public DBHandler(string dbPath)
            : base(new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString())
        {
            Open();

            Console.WriteLine($"Are we even her {dbPath}");
            // Optional but recommended for robustness
            using var pragma = CreateCommand();
            pragma.CommandText = "PRAGMA foreign_keys=ON; PRAGMA journal_mode=WAL;";
            pragma.ExecuteNonQuery();
        }

        // INSERT/UPDATE (UPSERT) a key/value
        public void add_item(string table, string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key must not be empty.", nameof(key));

            var qTable = QuoteAndValidate(table);
            EnsureTableExists(qTable);

            using var cmd = CreateCommand();
            cmd.CommandText = $@"
                INSERT INTO {qTable} (key, value) VALUES ($key, $value)
                ON CONFLICT(key) DO UPDATE SET value = excluded.value;";
            cmd.Parameters.AddWithValue("$key", key);
            cmd.Parameters.AddWithValue("$value", Serialize(value));
            cmd.ExecuteNonQuery();
        }

        // Get value for key; if missing, return the provided default (value?.ToString() ?? "")
        public string return_item(string table, string key, object value)
        {
            var qTable = QuoteAndValidate(table);
            EnsureTableExists(qTable);

            using var cmd = CreateCommand();
            cmd.CommandText = $"SELECT value FROM {qTable} WHERE key = $key LIMIT 1;";
            cmd.Parameters.AddWithValue("$key", key);

            var result = cmd.ExecuteScalar() as string;
            return result ?? (value?.ToString() ?? string.Empty);
        }

        // Reverse lookup: find a key by an exact value match (JSON-equality)
        public string search_item(string table, string key, object value)
        {
            _ = key; // kept for signature compatibility

            var qTable = QuoteAndValidate(table);
            EnsureTableExists(qTable);

            using var cmd = CreateCommand();
            cmd.CommandText = $"SELECT key FROM {qTable} WHERE value = $value LIMIT 1;";
            cmd.Parameters.AddWithValue("$value", Serialize(value));

            var result = cmd.ExecuteScalar() as string;
            return result ?? string.Empty;
        }

        // --- helpers ---
        private static string Serialize(object value) =>
            value is null ? "null" : JsonSerializer.Serialize(value);

        private static string QuoteAndValidate(string table)
        {
            if (!TableName.IsMatch(table))
                throw new ArgumentException("Invalid table name. Use letters, digits, underscore; start with letter/_", nameof(table));
            return $"\"{table}\""; // quote identifier
        }

        private void EnsureTableExists(string qTable)
        {
            using var cmd = CreateCommand();
            cmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS {qTable} (
                    key   TEXT PRIMARY KEY,
                    value TEXT NOT NULL
                );";
            cmd.ExecuteNonQuery();
        }

        // Create the 'issues' table if it doesn't exist (idempotent)
        public void EnsureIssuesTable()
        {
            using var cmd = CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS issues (
                  id INTEGER PRIMARY KEY AUTOINCREMENT,
                  location    TEXT NOT NULL,
                  category    TEXT NOT NULL,
                  description TEXT NOT NULL,
                  media       TEXT,  -- JSON array of file paths
                  created_at  TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ','now'))
                );";
            cmd.ExecuteNonQuery();
        }

        // Insert a new issue row
        public void InsertIssue(string location, string category, string description, IEnumerable<string>? mediaPaths)
        {
            using var cmd = CreateCommand();
            cmd.CommandText = @"
                INSERT INTO issues (location, category, description, media)
                VALUES ($loc, $cat, $desc, $media);";
            cmd.Parameters.AddWithValue("$loc", location);
            cmd.Parameters.AddWithValue("$cat", category);
            cmd.Parameters.AddWithValue("$desc", description);

            string? mediaJson = (mediaPaths != null && mediaPaths.Any())
                ? JsonSerializer.Serialize(mediaPaths)
                : null;

            if (mediaJson is null)
                cmd.Parameters.AddWithValue("$media", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("$media", mediaJson);

            cmd.ExecuteNonQuery();
        }

        // Create the 'service_requests' table if it doesn't exist
        public void EnsureServiceRequestsTable()
        {
            using var cmd = CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS service_requests (
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
                  updates TEXT,  -- JSON array of updates
                  depends_on TEXT  -- JSON array of dependency IDs
                );";
            cmd.ExecuteNonQuery();
        }

        // Insert a new service request
        public void InsertServiceRequest(ServiceRequest request)
        {
            using var cmd = CreateCommand();
            cmd.CommandText = @"
                INSERT INTO service_requests
                (request_id, title, description, category, location, status, priority,
                 submitted_date, last_updated, resolved_date, submitted_by, assigned_to, updates, depends_on)
                VALUES ($id, $title, $desc, $cat, $loc, $status, $priority,
                        $submitted, $updated, $resolved, $submitter, $assigned, $updates, $depends);";

            cmd.Parameters.AddWithValue("$id", request.RequestId);
            cmd.Parameters.AddWithValue("$title", request.Title);
            cmd.Parameters.AddWithValue("$desc", request.Description);
            cmd.Parameters.AddWithValue("$cat", request.Category);
            cmd.Parameters.AddWithValue("$loc", request.Location);
            cmd.Parameters.AddWithValue("$status", (int)request.Status);
            cmd.Parameters.AddWithValue("$priority", (int)request.Priority);
            cmd.Parameters.AddWithValue("$submitted", request.SubmittedDate.ToString("o"));

            cmd.Parameters.AddWithValue("$updated",
                request.LastUpdated.HasValue ? request.LastUpdated.Value.ToString("o") : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$resolved",
                request.ResolvedDate.HasValue ? request.ResolvedDate.Value.ToString("o") : (object)DBNull.Value);

            cmd.Parameters.AddWithValue("$submitter", request.SubmittedBy ?? "Anonymous");
            cmd.Parameters.AddWithValue("$assigned",
                string.IsNullOrEmpty(request.AssignedTo) ? (object)DBNull.Value : request.AssignedTo);

            cmd.Parameters.AddWithValue("$updates", JsonSerializer.Serialize(request.Updates));
            cmd.Parameters.AddWithValue("$depends", JsonSerializer.Serialize(request.DependsOn));

            cmd.ExecuteNonQuery();
        }

        // Update an existing service request
        public void UpdateServiceRequest(ServiceRequest request)
        {
            using var cmd = CreateCommand();
            cmd.CommandText = @"
                UPDATE service_requests
                SET title = $title, description = $desc, category = $cat, location = $loc,
                    status = $status, priority = $priority, last_updated = $updated,
                    resolved_date = $resolved, assigned_to = $assigned, updates = $updates,
                    depends_on = $depends
                WHERE request_id = $id;";

            cmd.Parameters.AddWithValue("$id", request.RequestId);
            cmd.Parameters.AddWithValue("$title", request.Title);
            cmd.Parameters.AddWithValue("$desc", request.Description);
            cmd.Parameters.AddWithValue("$cat", request.Category);
            cmd.Parameters.AddWithValue("$loc", request.Location);
            cmd.Parameters.AddWithValue("$status", (int)request.Status);
            cmd.Parameters.AddWithValue("$priority", (int)request.Priority);

            cmd.Parameters.AddWithValue("$updated",
                request.LastUpdated.HasValue ? request.LastUpdated.Value.ToString("o") : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$resolved",
                request.ResolvedDate.HasValue ? request.ResolvedDate.Value.ToString("o") : (object)DBNull.Value);

            cmd.Parameters.AddWithValue("$assigned",
                string.IsNullOrEmpty(request.AssignedTo) ? (object)DBNull.Value : request.AssignedTo);

            cmd.Parameters.AddWithValue("$updates", JsonSerializer.Serialize(request.Updates));
            cmd.Parameters.AddWithValue("$depends", JsonSerializer.Serialize(request.DependsOn));

            cmd.ExecuteNonQuery();
        }

        // Get all service requests
        public List<ServiceRequest> GetAllServiceRequests()
        {
            List<ServiceRequest> requests = new List<ServiceRequest>();

            using var cmd = CreateCommand();
            cmd.CommandText = "SELECT * FROM service_requests ORDER BY submitted_date DESC;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var request = new ServiceRequest
                {
                    RequestId = reader.GetString(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Category = reader.GetString(3),
                    Location = reader.GetString(4),
                    Status = (RequestStatus)reader.GetInt32(5),
                    Priority = (RequestPriority)reader.GetInt32(6),
                    SubmittedDate = DateTime.Parse(reader.GetString(7)),
                    LastUpdated = reader.IsDBNull(8) ? null : DateTime.Parse(reader.GetString(8)),
                    ResolvedDate = reader.IsDBNull(9) ? null : DateTime.Parse(reader.GetString(9)),
                    SubmittedBy = reader.GetString(10),
                    AssignedTo = reader.IsDBNull(11) ? null : reader.GetString(11),
                    Updates = JsonSerializer.Deserialize<List<string>>(reader.GetString(12)) ?? new List<string>(),
                    DependsOn = JsonSerializer.Deserialize<List<string>>(reader.GetString(13)) ?? new List<string>()
                };

                requests.Add(request);
            }

            return requests;
        }

        // Get a specific service request by ID
        public ServiceRequest GetServiceRequestById(string requestId)
        {
            using var cmd = CreateCommand();
            cmd.CommandText = "SELECT * FROM service_requests WHERE request_id = $id LIMIT 1;";
            cmd.Parameters.AddWithValue("$id", requestId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new ServiceRequest
                {
                    RequestId = reader.GetString(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Category = reader.GetString(3),
                    Location = reader.GetString(4),
                    Status = (RequestStatus)reader.GetInt32(5),
                    Priority = (RequestPriority)reader.GetInt32(6),
                    SubmittedDate = DateTime.Parse(reader.GetString(7)),
                    LastUpdated = reader.IsDBNull(8) ? null : DateTime.Parse(reader.GetString(8)),
                    ResolvedDate = reader.IsDBNull(9) ? null : DateTime.Parse(reader.GetString(9)),
                    SubmittedBy = reader.GetString(10),
                    AssignedTo = reader.IsDBNull(11) ? null : reader.GetString(11),
                    Updates = JsonSerializer.Deserialize<List<string>>(reader.GetString(12)) ?? new List<string>(),
                    DependsOn = JsonSerializer.Deserialize<List<string>>(reader.GetString(13)) ?? new List<string>()
                };
            }

            return null;
        }
    }
}
