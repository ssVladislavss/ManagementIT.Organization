using AutoMapper;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Constants;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.LogMessageModels;
using OrganizationEntity.Core.Models.SubdivisionModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Contracts.Enums;

namespace OrganizationEntity.DataAccess.Service
{
    public class SubdivisionService : ISubdivisionService
    {
        private readonly IGenericRepository<Subdivision> _subRepository;
        private readonly IMapper _mapper;
        private readonly ILogService _service;

        public SubdivisionService(IGenericRepository<Subdivision> subRepository, IMapper mapper, ILogService service)
        {
            _subRepository = subRepository ?? throw new ArgumentNullException(nameof(subRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<OrganizationEntityActionResult> AddAsync(SubdivisionDTO model, ClaimsPrincipal principal)
        {
            var entity = _mapper.Map<Subdivision>(model);
            return await _subRepository.AddEntityAsync(entity, principal?.Identity?.Name);
        }

        public async Task<OrganizationEntityActionResult> DeleteAsync(int subId, ClaimsPrincipal principal)
        {
            var entity = await _subRepository.GetEntityByIdAsync(subId, principal?.Identity?.Name);
            
            if (entity.AspNetException != null) return entity;
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при удалении модели, модель не найдена || Модель: < {typeof(Subdivision)} > || ID: < {subId} >");
            
            return await _subRepository.DeleteEntityAsync(entity.Data, principal?.Identity?.Name);
        }

        public bool ExistEntityByName(string name, int? Tid = null) => _subRepository.ExistEntityByName(name, Tid);

        public async Task<OrganizationEntityActionResult<IEnumerable<SubdivisionDTO>>> GetAllAsync(ClaimsPrincipal principal)
        {
            var subdivisions = await _subRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            
            if (subdivisions.AspNetException != null) 
                return OrganizationEntityActionResult<IEnumerable<SubdivisionDTO>>.Fail(null, subdivisions.Errors, subdivisions.AspNetException);
            if (!subdivisions.Data.Any())
                return OrganizationEntityActionResult<IEnumerable<SubdivisionDTO>>
                    .Fail(null, new[] { TypeOfErrors.NoContent }, $"Не найдено ниодной модели || Модель: < {typeof(Subdivision)} >");
            
            var response = _mapper.Map<IEnumerable<SubdivisionDTO>>(subdivisions.Data);
            return OrganizationEntityActionResult<IEnumerable<SubdivisionDTO>>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<SubdivisionDTO>> GetByIdAsync(int id, ClaimsPrincipal principal)
        {
            var result = await _subRepository.GetEntityByIdAsync(id, principal?.Identity?.Name);
            if (result.AspNetException != null)
                return OrganizationEntityActionResult<SubdivisionDTO>.Fail(null, result.Errors, result.AspNetException);
            if (result.Data == null)
                return OrganizationEntityActionResult<SubdivisionDTO>
                    .Fail(null, new[] { TypeOfErrors.NotFound }, $"Модель не найдена || Модель: < {typeof(Subdivision)} > || ID: < {id} >");
            
            var response = _mapper.Map<SubdivisionDTO>(result.Data);
            return OrganizationEntityActionResult<SubdivisionDTO>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult> UpdateAsync(SubdivisionDTO model, ClaimsPrincipal principal)
        {
            var entity = await _subRepository.GetEntityByIdAsync(model.Id, principal?.Identity?.Name);
            if(entity.AspNetException != null)
                return OrganizationEntityActionResult.Fail(entity.Errors, entity.AspNetException);
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при изменении модели|| Модель не найдена || Модель: < {typeof(Subdivision)} > || ID: < {model.Id} >");
            
            entity.Data.Name = model.Name;
            return await _subRepository.UpdateEntityAsync(entity.Data, principal?.Identity?.Name);
        }
    }
}
