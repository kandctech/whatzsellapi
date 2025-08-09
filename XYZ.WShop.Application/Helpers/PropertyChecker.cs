using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Helpers
{
    public static class PropertyChecker
    {
        public static bool DoesPropertyExist<T>(string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            return property != null;
        }
    }
}
