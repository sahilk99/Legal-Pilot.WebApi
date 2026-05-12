using Microsoft.EntityFrameworkCore;
using Compliance_Hub.api.Data;
using Compliance_Hub.api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Compliance_Hub.api.Services
{
    public class DocumentStatusService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DocumentStatusService> _logger;

        public DocumentStatusService(IServiceScopeFactory scopeFactory, ILogger<DocumentStatusService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Document Status Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking document statuses...");
                    await UpdateDocumentStatuses();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating document statuses.");
                }

                // Wait for 24 hours or until the service is stopped
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }

            _logger.LogInformation("Document Status Service is stopping.");
        }

        private async Task UpdateDocumentStatuses()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var now = DateTime.UtcNow;
                var thirtyDaysFromNow = now.AddDays(30);

                var documents = await context.Documents
                    .Where(d => !d.IsDeleted)
                    .ToListAsync();

                bool changesMade = false;

                foreach (var doc in documents)
                {
                    if (doc.ExpiryDate == null) continue;

                    string oldStatus = doc.Status;
                    string newStatus = oldStatus;

                    if (doc.ExpiryDate < now)
                    {
                        newStatus = "Overdue";
                    }
                    else if (doc.ExpiryDate <= thirtyDaysFromNow)
                    {
                        newStatus = "ExpiringSoon";
                    }
                    else
                    {
                        newStatus = "Compliant";
                    }

                    if (oldStatus != newStatus)
                    {
                        doc.Status = newStatus;
                        doc.UpdatedAt = now;
                        changesMade = true;

                        if (newStatus == "ExpiringSoon" || newStatus == "Overdue")
                        {
                            await CreateNotificationsForDepartment(context, doc);
                        }
                    }
                }

                if (changesMade)
                {
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task CreateNotificationsForDepartment(AppDbContext context, Document doc)
        {
            if (!doc.DepartmentId.HasValue) return;

            var users = await context.Users
                .Where(u => u.DepartmentId == doc.DepartmentId && !u.IsDeleted)
                .ToListAsync();

            string message = doc.Status == "ExpiringSoon"
                ? $"Document '{doc.Title}' is expiring on {doc.ExpiryDate:dd MMM yyyy}"
                : $"Document '{doc.Title}' has expired and is now Overdue";

            foreach (var user in users)
            {
                // Check if an unread notification already exists for this user and document
                bool exists = await context.Notifications.AnyAsync(n => 
                    n.UserId == user.Id && 
                    n.DocumentId == doc.Id && 
                    !n.IsRead && 
                    !n.IsDeleted);

                if (!exists)
                {
                    context.Notifications.Add(new Notification
                    {
                        UserId = user.Id,
                        DocumentId = doc.Id,
                        Message = message,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }
    }
}
