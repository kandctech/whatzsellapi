using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Interfaces.Data;
using XYZ.WShop.Domain;

namespace XZY.WShop.Infrastructure.Data.Repositories
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
