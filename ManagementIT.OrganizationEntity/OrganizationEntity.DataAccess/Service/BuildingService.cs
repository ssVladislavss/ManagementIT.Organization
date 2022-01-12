using AutoMapper;
using Contracts.Enums;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Constants;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.BuildingModels;
using OrganizationEntity.Core.Models.LogMessageModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrganizationEntity.DataAccess.Service
{
    public class BuildingService : IBuildingService
    {
        private readonly IGenericRepository<Building> _buildingRepository;
        private readonly IMapper _mapper;
        private readonly ILogService _service;

        public BuildingService(IGenericRepository<Building> buildingRepository, IMapper mapper, ILogService service)
        {
            _buildingRepository = buildingRepository ?? throw new ArgumentNullException(nameof(buildingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<OrganizationEntityActionResult> AddAsync(BuildingDTO model, ClaimsPrincipal principal)
        {
            var entity = _mapper.Map<Building>(model);
            return await _buildingRepository.AddEntityAsync(entity, principal?.Identity?.Name);
        }

        public async Task<OrganizationEntityActionResult> DeleteAsync(int buildingId, ClaimsPrincipal principal)
        {
            var entity = await _buildingRepository.GetEntityByIdAsync(buildingId, principal?.Identity?.Name);
            
            if (entity.AspNetException != null)
                return OrganizationEntityActionResult.Fail(entity.Errors, entity.AspNetException);
            
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при удалении модели || Модель не найдена || Модель: < {typeof(Building)} > || ID: < {buildingId} >");
            
            return await _buildingRepository.DeleteEntityAsync(entity.Data, principal?.Identity?.Name);
        }

        public bool ExistEntityByName(string name, int? Tid = null) => _buildingRepository.ExistEntityByName(name, Tid);
        
        public async Task<OrganizationEntityActionResult<IEnumerable<BuildingDTO>>> GetAllAsync(ClaimsPrincipal principal)
        {
            var buildings = await _buildingRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (buildings.AspNetException != null)
                return OrganizationEntityActionResult<IEnumerable<BuildingDTO>>.Fail(null, buildings.Errors, buildings.AspNetException);
            
            if (!buildings.Data.Any())
                return OrganizationEntityActionResult<IEnumerable<BuildingDTO>>
                    .Fail(null, new[] { TypeOfErrors.NoContent }, $"Не найдено ниодной модели || Модель: < {typeof(Building)} >");
            
            var response = _mapper.Map<IEnumerable<BuildingDTO>>(buildings.Data);
            return OrganizationEntityActionResult<IEnumerable<BuildingDTO>>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<BuildingDTO>> GetByIdAsync(int buildingId, ClaimsPrincipal principal)
        {
            var result = await _buildingRepository.GetEntityByIdAsync(buildingId, principal?.Identity?.Name);
            if (result.AspNetException != null)
                return OrganizationEntityActionResult<BuildingDTO>.Fail(null, result.Errors, result.AspNetException);

            if (result.Data == null)
                return OrganizationEntityActionResult<BuildingDTO>
                    .Fail(null, new[] { TypeOfErrors.NotFound }, $"Модель не найдена || Модель: < {typeof(Building)} > || ID: < {buildingId} >");
            
            var response = _mapper.Map<BuildingDTO>(result.Data);
            return OrganizationEntityActionResult<BuildingDTO>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult> UpdateAsync(BuildingDTO model, ClaimsPrincipal principal)
        {
            var entity = await _buildingRepository.GetEntityByIdAsync(model.Id, principal?.Identity?.Name);
            if (entity.AspNetException != null)
                return OrganizationEntityActionResult.Fail(entity.Errors, entity.AspNetException);
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при изменении модели, данные не найдены || Модель: < {typeof(Building)} > || ID: < {model.Id} > || Name: < {model.Name} >");
            
            entity.Data.Floor = model.Floor;
            entity.Data.Address = model.Address;
            entity.Data.Name = model.Name;

            return await _buildingRepository.UpdateEntityAsync(entity.Data, principal?.Identity?.Name);
        }
    }
}
