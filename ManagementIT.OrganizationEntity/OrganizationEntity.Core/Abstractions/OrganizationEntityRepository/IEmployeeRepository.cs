using System.Collections.Generic;
using System.Threading.Tasks;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.ResponseModels;

namespace OrganizationEntity.Core.Abstractions.OrganizationEntityRepository
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<OrganizationEntityActionResult<IEnumerable<Employee>>> GetEmployeesByDepartamentAsync(int departamentId, string iniciator);
        Task<OrganizationEntityActionResult<IEnumerable<Employee>>> GetAllEmployeeAsync();
    }
}