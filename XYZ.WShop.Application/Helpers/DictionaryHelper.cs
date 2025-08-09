using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Helpers
{
    public class DictionaryHelper
    {
        public static Dictionary<string, string> GetSortableColumnList()
        {
            return new Dictionary<string, string>()
            {
                {"createddate", "createddate" },
            };
        }
    }
}
