using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Dtos
{
    public class PaginatedResponseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}
