using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain;

namespace XZY.WShop.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyGlobalFilters(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsActive);
        }
    }
}
