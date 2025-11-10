using System;
using System.Collections.Generic;

namespace GovernmentServices
{
    /// <summary>
    /// Represents the status of a service request
    /// </summary>
    public enum RequestStatus
    {
        Submitted = 0,      // Request has been submitted
        Pending = 1,        // Request is pending review
        InProgress = 2,     // Request is being worked on
        OnHold = 3,         // Request is temporarily on hold
        Resolved = 4,       // Request has been resolved
        Closed = 5          // Request has been closed
    }

    /// <summary>
    /// Represents the priority level of a service request
    /// </summary>
    public enum RequestPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Urgent = 3,
        Critical = 4
    }

    /// <summary>
    /// Represents a service request submitted by a citizen
    /// </summary>
    public class ServiceRequest : IComparable<ServiceRequest>
    {
        public string RequestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public RequestStatus Status { get; set; }
        public RequestPriority Priority { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string SubmittedBy { get; set; }
        public string AssignedTo { get; set; }
        public List<string> Updates { get; set; }
        public List<string> DependsOn { get; set; }  // Other request IDs this depends on

        public ServiceRequest()
        {
            RequestId = GenerateRequestId();
            Updates = new List<string>();
            DependsOn = new List<string>();
            SubmittedDate = DateTime.Now;
            Status = RequestStatus.Submitted;
            Priority = RequestPriority.Normal;
        }

        /// <summary>
        /// Generates a unique request ID
        /// </summary>
        private static string GenerateRequestId()
        {
            return $"SR-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        /// <summary>
        /// Compares service requests by submission date (for sorting)
        /// </summary>
        public int CompareTo(ServiceRequest other)
        {
            if (other == null) return 1;
            return SubmittedDate.CompareTo(other.SubmittedDate);
        }

        /// <summary>
        /// Gets the status display text with emoji
        /// </summary>
        public string GetStatusDisplay()
        {
            return Status switch
            {
                RequestStatus.Submitted => "📝 Submitted",
                RequestStatus.Pending => "⏳ Pending",
                RequestStatus.InProgress => "🔄 In Progress",
                RequestStatus.OnHold => "⏸️ On Hold",
                RequestStatus.Resolved => "✅ Resolved",
                RequestStatus.Closed => "🔒 Closed",
                _ => Status.ToString()
            };
        }

        /// <summary>
        /// Gets the priority display text with emoji
        /// </summary>
        public string GetPriorityDisplay()
        {
            return Priority switch
            {
                RequestPriority.Low => "🟢 Low",
                RequestPriority.Normal => "🟡 Normal",
                RequestPriority.High => "🟠 High",
                RequestPriority.Urgent => "🔴 Urgent",
                RequestPriority.Critical => "🚨 Critical",
                _ => Priority.ToString()
            };
        }

        /// <summary>
        /// Gets a formatted display text for list view
        /// </summary>
        public string GetDisplayText()
        {
            string statusEmoji = Status switch
            {
                RequestStatus.Submitted => "📝",
                RequestStatus.Pending => "⏳",
                RequestStatus.InProgress => "🔄",
                RequestStatus.OnHold => "⏸️",
                RequestStatus.Resolved => "✅",
                RequestStatus.Closed => "🔒",
                _ => "📋"
            };

            string priorityEmoji = Priority switch
            {
                RequestPriority.Low => "🟢",
                RequestPriority.Normal => "🟡",
                RequestPriority.High => "🟠",
                RequestPriority.Urgent => "🔴",
                RequestPriority.Critical => "🚨",
                _ => "⚪"
            };

            return $"{statusEmoji} {RequestId} - {Title} [{priorityEmoji} {Priority}]";
        }

        /// <summary>
        /// Gets detailed information about the request
        /// </summary>
        public string GetDetailedInfo()
        {
            var info = $"Request ID: {RequestId}\n";
            info += $"Title: {Title}\n";
            info += $"Category: {Category}\n";
            info += $"Location: {Location}\n";
            info += $"Status: {GetStatusDisplay()}\n";
            info += $"Priority: {GetPriorityDisplay()}\n";
            info += $"Submitted: {SubmittedDate:yyyy-MM-dd HH:mm}\n";

            if (LastUpdated.HasValue)
                info += $"Last Updated: {LastUpdated.Value:yyyy-MM-dd HH:mm}\n";

            if (ResolvedDate.HasValue)
                info += $"Resolved: {ResolvedDate.Value:yyyy-MM-dd HH:mm}\n";

            info += $"Submitted By: {SubmittedBy}\n";

            if (!string.IsNullOrEmpty(AssignedTo))
                info += $"Assigned To: {AssignedTo}\n";

            info += $"\nDescription:\n{Description}\n";

            if (DependsOn.Count > 0)
            {
                info += $"\nDepends On:\n";
                foreach (var depId in DependsOn)
                    info += $"  - {depId}\n";
            }

            if (Updates.Count > 0)
            {
                info += $"\nStatus Updates ({Updates.Count}):\n";
                for (int i = Updates.Count - 1; i >= 0; i--)
                {
                    info += $"  {i + 1}. {Updates[i]}\n";
                }
            }

            return info;
        }

        /// <summary>
        /// Adds a status update to the request
        /// </summary>
        public void AddUpdate(string update)
        {
            Updates.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] {update}");
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Gets the age of the request in days
        /// </summary>
        public int GetAgeDays()
        {
            return (DateTime.Now - SubmittedDate).Days;
        }

        /// <summary>
        /// Checks if the request is overdue (more than 30 days and not resolved)
        /// </summary>
        public bool IsOverdue()
        {
            return GetAgeDays() > 30 && Status != RequestStatus.Resolved && Status != RequestStatus.Closed;
        }
    }
}
