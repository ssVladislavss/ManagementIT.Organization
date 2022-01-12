using AutoMapper;
using Contracts.Enums;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.OrganizationEntityRepository;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Constants;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.Models.LogMessageModels;
using OrganizationEntity.Core.Models.SubdivisionModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.DataAccess.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _deptRepository;
        private readonly IGenericRepository<Subdivision> _subdivisionRepository;
        private readonly IMapper _mapper;
        private readonly ILogService _service;

        public DepartmentService(IDepartmentRepository deptRepository,
                                 IGenericRepository<Subdivision> subdivisionRepository,
                                 IMapper mapper,
                                 ILogService service)
        {
            _deptRepository = deptRepository ?? throw new ArgumentNullException(nameof(deptRepository));
            _subdivisionRepository = subdivisionRepository ?? throw new ArgumentNullException(nameof(subdivisionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }


        public async Task<OrganizationEntityActionResult> AddAsync(DepartmentDTO model, ClaimsPrincipal principal)
        {
            var entity = _mapper.Map<Department>(model);

            var subdivision = await _subdivisionRepository.GetEntityByIdAsync(model.SubdivisionId, principal?.Identity?.Name);

            if (subdivision.AspNetException != null) return OrganizationEntityActionResult.Fail(subdivision.Errors, subdivision.AspNetException);
            else if(subdivision.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistSubdivision },
                    $"Ошибка при добавлении модели, < {typeof(Department)} > || Не найдено подразделение || ID <{model.SubdivisionId}>");
            else entity.Subdivision = subdivision.Data;

            return await _deptRepository.AddEntityAsync(entity, principal?.Identity?.Name);
        }

        public async Task<OrganizationEntityActionResult> DeleteAsync(int deptId, ClaimsPrincipal principal)
        {
            var entity = await _deptRepository.GetEntityByIdAsync(deptId, principal?.Identity?.Name);
            if (entity.AspNetException != null)
                return OrganizationEntityActionResult.Fail(entity.Errors, entity.AspNetException);
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при удаление модели, модель не найдена || Модель: < {typeof(Department)} > || ID: < {deptId} >");
            
            return await _deptRepository.DeleteEntityAsync(entity.Data, principal?.Identity?.Name);
        }

        public bool ExistEntityByName(string name, int? Tid = null) => _deptRepository.ExistEntityByName(name, Tid);

        public async Task<OrganizationEntityActionResult<IEnumerable<DepartmentDTO>>> GetAllAsync(ClaimsPrincipal principal)
        {
            var dept = await _deptRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (dept.AspNetException != null)
                return OrganizationEntityActionResult<IEnumerable<DepartmentDTO>>.Fail(null, dept.Errors, dept.AspNetException);

            if (!dept.Data.Any())
                return OrganizationEntityActionResult<IEnumerable<DepartmentDTO>>
                    .Fail(null, new[] { TypeOfErrors.NoContent }, $"Модели не найдены || Модель: < {typeof(Department)} >");
            
            var response = _mapper.Map<IEnumerable<DepartmentDTO>>(dept.Data);
            return OrganizationEntityActionResult<IEnumerable<DepartmentDTO>>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<DepartmentDTO>> GetByIdAsync(int id, ClaimsPrincipal principal)
        {
            var result = await _deptRepository.GetEntityByIdAsync(id, principal?.Identity?.Name);
            
            if (result.AspNetException != null)
                return OrganizationEntityActionResult<DepartmentDTO>.Fail(null, result.Errors, result.AspNetException);
            if (result.Data == null)
                return OrganizationEntityActionResult<DepartmentDTO>
                    .Fail(null, new[] { TypeOfErrors.NotFound }, $"Модель не найдена || Модель: < {typeof(Department)} > || ID: < {id} >");
            
            var response = _mapper.Map<DepartmentDTO>(result.Data);
            return OrganizationEntityActionResult<DepartmentDTO>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<CreateOrEditDeptModel>> GetCreateDeptAsync(ClaimsPrincipal principal)
        {
            var subdivisions = await _subdivisionRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            
            if (subdivisions.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditDeptModel>.Fail(null, subdivisions.Errors, subdivisions.AspNetException);

            if (!subdivisions.Data.Any())return OrganizationEntityActionResult<CreateOrEditDeptModel>.Fail(null, new[] { TypeOfErrors.NotExistSubdivision },
                    $"Невозможно выполнить запрос на создание отделения || Не найдено ниодного подразделения");
            
            var subdivisionModels = _mapper.Map<List<SubdivisionDTO>>(subdivisions.Data);

            var response = new CreateOrEditDeptModel(subdivisionModels);
            return OrganizationEntityActionResult<CreateOrEditDeptModel>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<CreateOrEditDeptModel>> GetUpdateDeptAsync(int deptId, ClaimsPrincipal principal)
        {
            var dept = await _deptRepository.GetEntityByIdAsync(deptId, principal?.Identity?.Name);
            if (dept.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditDeptModel>.Fail(null, dept.Errors, dept.AspNetException);
            if (dept.Data == null)
                return OrganizationEntityActionResult<CreateOrEditDeptModel>.Fail
                    (null, new[] { TypeOfErrors.NotFound }, $"Ошибка при поиске департамента || ID < {deptId} >");
            
            var subDivisions = await _subdivisionRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if(subDivisions.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditDeptModel>.Fail(null, subDivisions.Errors, subDivisions.AspNetException);

            if (!subDivisions.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditDeptModel>.Fail
                    (null, new[] { TypeOfErrors.NotExistSubdivision }, $"В базе нет подразделений");
            
            var subDivisionModels = _mapper.Map<List<SubdivisionDTO>>(subDivisions.Data);

            var response = new CreateOrEditDeptModel(subDivisionModels, dept.Data.Name, dept.Data.Subdivision.Id);
            return OrganizationEntityActionResult<CreateOrEditDeptModel>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult> UpdateAsync(DepartmentDTO model, ClaimsPrincipal principal)
        {
            var entity = await _deptRepository.GetEntityByIdAsync(model.Id, principal?.Identity?.Name);
            if (entity.AspNetException != null)
                return OrganizationEntityActionResult.Fail(entity.Errors, entity.AspNetException);
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при изменении данных модели, модель не найдена || Модель: < {typeof(Department)} > || ID: < {model.Id} >");
            

            var subdivision = await _subdivisionRepository.GetEntityByIdAsync(model.SubdivisionId, principal?.Identity?.Name);
            entity.Data.Name = model.Name;

            if (subdivision.AspNetException != null) return subdivision;
            else if (subdivision.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistSubdivision },
                    $"Ошибка при изменении данных модели || < {typeof(Department)} > || Не найдено подразделение || ID: < {model.Id} >");
            else entity.Data.Subdivision = subdivision.Data;

            return await _deptRepository.UpdateEntityAsync(entity.Data, principal?.Identity?.Name);
        }
    }
}
