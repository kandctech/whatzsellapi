using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    public class FeedBack
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Experience { get; set; }
        public string? Name { get; set; }
        public string? Platform { get; set; }
        public int Rating { get; set; }
        public string? Suggestions { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
