using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Dtos.Debtor
{
    public class DebtorResponse
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public List<DebtorPayment> Payments { get; set; } = new List<DebtorPayment>();
        public decimal TotalPaid { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string? Status { get; set; }
    }
}
