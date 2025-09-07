using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Dtos.Creditor
{
    public class CreditorResponse
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string CreditorName { get; set; }
        public string CreditorPhone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public List<CreditorPayment> Payments { get; set; } = new List<CreditorPayment>();
        public decimal TotalPaid { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string? Status { get; set; }
    }
}
