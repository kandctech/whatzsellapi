using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain;

namespace XZY.WShop.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public static int AmountPerDay => 50;
        public DbSet<Business> Busineses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<SaleActivity> SaleActivities { get; set; }
        public DbSet<FollowUp> FollowUps { get; set; }
        public DbSet<XYZ.WShop.Domain.TaskPlanner> Tasks { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<CreditorRecord> CreditorRecords { get; set; }
        public DbSet<CreditorPayment> CreditorPayments { get; set; }
        public DbSet<DebtorRecord> DebtorRecords { get; set; }
        public DbSet<DebtorPayment> DebtorPayments { get; set; }
    }
}
