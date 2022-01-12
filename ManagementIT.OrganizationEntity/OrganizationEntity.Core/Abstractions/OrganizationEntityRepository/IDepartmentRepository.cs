using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Abstractions.OrganizationEntityRepository
{
    public interface IDepartmentRepository : IGenericRepository<Department> { }
}
