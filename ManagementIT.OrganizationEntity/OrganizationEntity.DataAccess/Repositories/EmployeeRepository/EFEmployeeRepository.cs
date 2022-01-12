using Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.OrganizationEntityRepository;
using OrganizationEntity.Core.Constants;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.LogMessageModels;
using OrganizationEntity.Core.ResponseModels;
using OrganizationEntity.DataAccess.Data;
using OrganizationEntity.DataAccess.Repositories.TEntityRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.DataAccess.Repositories.EmployeeRepository
{
    public class EFEmployeeRepository : EFGenericRepository<Employee>, IEmployeeRepository
    {
        public EFEmployeeRepository(AppDbContext context, ILogService service) : base(context, service)
        {
            Includes = new Expression<Func<Employee, object>>[]
            {
                dependency => dependency.Departament,
                dependency => dependency.Departament.Subdivision,
                dependency => dependency.Position,
                dependency => dependency.Photo
            };
        }

        public async Task<OrganizationEntityActionResult<IEnumerable<Employee>>> GetEmployeesByDepartamentAsync(int departamentId, string iniciator)
        {
            try
            {
                var response = await _context.Employees.Include(x => x.Departament).Include(x => x.Position).Where(x => x.Departament.Id == departamentId).ToListAsync();
                return OrganizationEntityActionResult<IEnumerable<Employee>>.IsSuccess(response);
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult<IEnumerable<Employee>>.Fail(null, new[] { TypeOfErrors.InternalServerError},
                    $"Ошибка при поиске списка всех сотрудников департамента || DepartamentID < {departamentId} > || < {typeof(Employee)} > || {e.InnerException} >");
            }
        }

        public async Task<OrganizationEntityActionResult<IEnumerable<Employee>>> GetAllEmployeeAsync()
        {
            try
            {
                var response = await _context.Employees.Include(x => x.Departament).Include(x => x.Position).ToListAsync();
                return OrganizationEntityActionResult<IEnumerable<Employee>>.IsSuccess(response);
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult<IEnumerable<Employee>>.Fail(null, new[] { TypeOfErrors.InternalServerError },
                    $"Ошибка при поиске списка всех сотрудников || < {typeof(Employee)} > || {e.InnerException} >");
            }
        }

        //Ищет сотрудника по уникальному UserName
        public override async Task<OrganizationEntityActionResult<Employee>> GetEntityByNameAsync(string name, string iniciator)
        {
            try
            {
                IQueryable<Employee> set = _context.Set<Employee>();
                if (Includes != null) set = Includes.Aggregate(set, (current, IncludeProp) => current.Include(IncludeProp));
                var result = await set.FirstOrDefaultAsync(x => x.User == name);
                return OrganizationEntityActionResult<Employee>.IsSuccess(result);
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult<Employee>.Fail(null, new[] { TypeOfErrors.InternalServerError },
                    $"Ошибка при поиске модели || Модель: < {typeof(Employee)} > || Входной параметр UserName: < {name} > || Описание: < {e.InnerException} >");
            }
        }
    }
}
