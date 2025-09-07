using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos.Creditor
{
    public class CreateCreditRequest
    {
        public Guid BusinessId { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class UpdateCreditRequest: CreateCreditRequest
    {
        public Guid Id { get; set; }
    }
}
