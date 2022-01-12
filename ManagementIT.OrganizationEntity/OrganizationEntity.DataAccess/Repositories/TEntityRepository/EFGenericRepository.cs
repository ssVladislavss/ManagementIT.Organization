using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using OrganizationEntity.Core;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Constants;
using OrganizationEntity.Core.Models.LogMessageModels;
using OrganizationEntity.Core.ResponseModels;
using OrganizationEntity.DataAccess.Data;

namespace OrganizationEntity.DataAccess.Repositories.TEntityRepository
{
    public class EFGenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected Expression<Func<T, object>>[] Includes;
        protected readonly ILogService _service;

        public EFGenericRepository(AppDbContext context, ILogService service)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<OrganizationEntityActionResult<IEnumerable<T>>> GetAllEntitiesAsync(string iniciator)
        {
            try
            {
                IQueryable<T> set = _context.Set<T>();
                if (Includes != null) set = Includes.Aggregate(set, (current, IncludeProp) => current.Include(IncludeProp));
                var response = await set.ToListAsync();
                return OrganizationEntityActionResult<IEnumerable<T>>.IsSuccess(response);
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult<IEnumerable<T>>.Fail(null, new[] { TypeOfErrors.InternalServerError},
                    $"Ошибка при поиске списка моделей || Модель: < {typeof(T)} > || Описание < {e.InnerException} >");
            }
        }

        public async Task<OrganizationEntityActionResult<IEnumerable<T>>> GetEntitiesByNameAsync(string name, string iniciator)
        {
            try
            {
                IQueryable<T> set = _context.Set<T>();
                if (Includes != null) set = Includes.Aggregate(set, (current, IncludeProp) => current.Include(IncludeProp));
                var response = await set.Where(x => x.Name == name).ToListAsync();
                return OrganizationEntityActionResult<IEnumerable<T>>.IsSuccess(response);
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult<IEnumerable<T>>.Fail(null, new[] { TypeOfErrors.InternalServerError },
                    $"Ошибка при поиске списка моделей || Модель: < {typeof(T)} > || Входной параметр name: < {name} > || Описание: < {e.InnerException} >");
            }
        }

        public virtual async Task<OrganizationEntityActionResult<T>> GetEntityByNameAsync(string name, string iniciator)
        {
            try
            {
                IQueryable<T> set = _context.Set<T>();
                if (Includes != null) set = Includes.Aggregate(set, (current, IncludeProp) => current.Include(IncludeProp));
                var result = await set.FirstOrDefaultAsync(x => x.Name == name);
                return OrganizationEntityActionResult<T>.IsSuccess(result);
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult<T>.Fail(null, new[] { TypeOfErrors.InternalServerError },
                    $"Ошибка при поиске модели || Модель: < {typeof(T)} > || Входной параметр name: < {name} > || Описание: < {e.InnerException} >");
            }
        }

        public async Task<OrganizationEntityActionResult<T>> GetEntityByIdAsync(int id, string iniciator)
        {
            try
            {
                IQueryable<T> set = _context.Set<T>();
                if (Includes != null) set = Includes.Aggregate(set, (current, IncludeProp) => current.Include(IncludeProp));
                var response = await set.FirstOrDefaultAsync(x => x.Id == id);
                return OrganizationEntityActionResult<T>.IsSuccess(response);
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult<T>.Fail(null, new[] { TypeOfErrors.InternalServerError},
                    $"Ошибка при поиске модели || Модель: < {typeof(T)} > || Входной параметр ID: < {id} > || Описание: < {e.InnerException} >");
            }
        }

        public async Task<OrganizationEntityActionResult> AddEntityAsync(T entity, string iniciator)
        {
            try
            {
                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();
                return OrganizationEntityActionResult.IsSuccess();
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.InternalServerError},
                    $"Ошибка добавления модели || Модель < {typeof(T)} > || ID < {entity?.Id} > || Name < {entity?.Name} > || {e.InnerException} >");
            }
        }

        public async Task<OrganizationEntityActionResult> UpdateEntityAsync(T entity, string iniciator)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return OrganizationEntityActionResult.IsSuccess();
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.InternalServerError},
                    $"Ошибка при изменении модели || Модель: < {typeof(T)} >  || ID < {entity.Id} > || Name < {entity.Name} > || Описание: < {e.InnerException} >");
            }
        }

        public async Task<OrganizationEntityActionResult> DeleteEntityAsync(T entity, string iniciator)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                return OrganizationEntityActionResult.IsSuccess();
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.DeletionEntityError },
                    $"Ошибка удаления модели || Модель < {typeof(T)} > || ID < {entity?.Id} > || Name < {entity?.Name} > || Описание: < {e.InnerException} >");
            }
        }

        public async Task<OrganizationEntityActionResult> DeleteRangeAsync(IEnumerable<T> entities, string iniciator)
        {
            try
            {
                _context.Set<T>().RemoveRange(entities);
                await _context.SaveChangesAsync();
                return OrganizationEntityActionResult.IsSuccess();
            }
            catch (Exception e)
            {
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.DeletionEntityError },
                    $"Ошибка при удалении списка моделей || Модель: < {typeof(T)} > || Входной параметр entities: < {typeof(IEnumerable<T>)} > || Описание: < {e.InnerException} >"); ;
            }
        }

        public bool ExistEntityByName(string name, int? Tid = null)
        {
            var response = false;
            response = Tid.HasValue ? _context.Set<T>().Where(x => x.Id != Tid).Any(x => x.Name == name) : _context.Set<T>().Any(x => x.Name == name);
            return response;
        }
    }
}