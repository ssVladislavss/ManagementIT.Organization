using AutoMapper;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Constants;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.LogMessageModels;
using OrganizationEntity.Core.Models.PositionModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Contracts.Enums;

namespace OrganizationEntity.DataAccess.Service
{
    public class PositionService : IPositionService
    {
        private readonly IGenericRepository<Position> _positionRepository;
        private readonly IMapper _mapper;
        private readonly ILogService _service;

        public PositionService(IGenericRepository<Position> positionRepository, IMapper mapper, ILogService service)
        {
            _positionRepository = positionRepository ?? throw new ArgumentNullException(nameof(positionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<OrganizationEntityActionResult> AddAsync(PositionDTO model, ClaimsPrincipal principal)
        {
            var entity = _mapper.Map<Position>(model);
            return await _positionRepository.AddEntityAsync(entity, principal?.Identity?.Name);
        }

        public async Task<OrganizationEntityActionResult> DeleteAsync(int positionId, ClaimsPrincipal principal)
        {
            var entity = await _positionRepository.GetEntityByIdAsync(positionId, principal?.Identity?.Name);
            
            if (entity.AspNetException != null) return entity;
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NoContent },
                    $"Ошибка при удалении модели, данные не найдены || Модель < {typeof(Position)} > || Входной параметр < {positionId} >");
            
            return await _positionRepository.DeleteEntityAsync(entity.Data, principal?.Identity?.Name);
        }

        public bool ExistEntityByName(string name, int? Tid = null) => _positionRepository.ExistEntityByName(name, Tid);

        public async Task<OrganizationEntityActionResult<IEnumerable<PositionDTO>>> GetAllAsync(ClaimsPrincipal principal)
        {
            var position = await _positionRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            
            if (position.AspNetException != null) return OrganizationEntityActionResult<IEnumerable<PositionDTO>>
                    .Fail(null, position.Errors, position.AspNetException);
            
            if (!position.Data.Any())
                return OrganizationEntityActionResult<IEnumerable<PositionDTO>>
                    .Fail(null, new[] { TypeOfErrors.NoContent }, $"Не найдено ниодной модели || Запрос списка моделей < {typeof(Position)} >");
            
            var response = _mapper.Map<IEnumerable<PositionDTO>>(position.Data);
            return OrganizationEntityActionResult<IEnumerable<PositionDTO>>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<PositionDTO>> GetByIdAsync(int id, ClaimsPrincipal principal)
        {
            var result = await _positionRepository.GetEntityByIdAsync(id, principal?.Identity?.Name);
            if (result.AspNetException != null) return OrganizationEntityActionResult<PositionDTO>
                    .Fail(null, result.Errors, result.AspNetException);
            if (result.Data == null)
                return OrganizationEntityActionResult<PositionDTO>
                    .Fail(null, new[] { TypeOfErrors.NotFound }, $"Модель не найдена || Запрос модели < {typeof(Position)} > || Входной параметр ID < {id} >");
            
            var response = _mapper.Map<PositionDTO>(result.Data);
            return OrganizationEntityActionResult<PositionDTO>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult> UpdateAsync(PositionDTO model, ClaimsPrincipal principal)
        {
            var entity = await _positionRepository.GetEntityByIdAsync(model.Id, principal?.Identity?.Name);
            if (entity.AspNetException != null) return OrganizationEntityActionResult
                    .Fail(entity.Errors, entity.AspNetException);
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NoContent },
                    $"Ошибка при изменении модели || Модель не найдена || Модель < {typeof(Position)} > || ID < {model.Id} >");
            
            entity.Data.Name = model.Name;
            return await _positionRepository.UpdateEntityAsync(entity.Data, principal?.Identity?.Name);
        }
    }
}
