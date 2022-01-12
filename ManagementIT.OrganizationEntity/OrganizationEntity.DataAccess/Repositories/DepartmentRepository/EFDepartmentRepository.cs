using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.OrganizationEntityRepository;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.DataAccess.Data;
using OrganizationEntity.DataAccess.Repositories.TEntityRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.DataAccess.Repositories.DepartmentRepository
{
    public class EFDepartmentRepository : EFGenericRepository<Department>, IDepartmentRepository
    {
        public EFDepartmentRepository(AppDbContext context, ILogService service) : base(context, service)
        {
            Includes = new Expression<Func<Department, object>>[]
            {
                dependency => dependency.Subdivision
            };
        }
    }
}
