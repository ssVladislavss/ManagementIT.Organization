using AutoMapper;
using Contracts.Enums;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.OrganizationEntityRepository;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Constants;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.BuildingModels;
using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.Models.LogMessageModels;
using OrganizationEntity.Core.Models.RoomModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrganizationEntity.DataAccess.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IDepartmentRepository _departamentRepository;
        private readonly IGenericRepository<Building> _buildingRepository;
        private readonly IMapper _mapper;
        private readonly ILogService _service;

        public RoomService(IRoomRepository roomRepository,
                           IDepartmentRepository departamentRepository,
                           IGenericRepository<Building> buildingRepository,
                           IMapper mapper, ILogService service)
        {
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
            _departamentRepository = departamentRepository ?? throw new ArgumentNullException(nameof(departamentRepository));
            _buildingRepository = buildingRepository ?? throw new ArgumentNullException(nameof(buildingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<OrganizationEntityActionResult> AddAsync(RoomDTO model, ClaimsPrincipal principal)
        {
            var entity = _mapper.Map<Room>(model);

            var dept = await _departamentRepository.GetEntityByIdAsync(model.DepartamentId, principal?.Identity?.Name);
            var building = await _buildingRepository.GetEntityByIdAsync(model.BuildingId, principal?.Identity?.Name);

            if (dept.AspNetException != null) return dept;
            else if (dept.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistDepartament },
                    $"Произошла ошибка при добавлении модели, < {typeof(Room)} > || Не найден департамент || ID <{model.DepartamentId}>");
            else entity.Departament = dept.Data;

            if (building.AspNetException != null) return building;
            else if (building.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistBuilding },
                    $"Произошла ошибка при добавлении модели, < {typeof(Room)} > || Не найдены данные о здании || ID <{model.BuildingId}>");
            else entity.Building = building.Data;

            return await _roomRepository.AddEntityAsync(entity, principal?.Identity?.Name);
        }

        public async Task<OrganizationEntityActionResult> DeleteAsync(int roomId, ClaimsPrincipal principal)
        {
            var entity = await _roomRepository.GetEntityByIdAsync(roomId, principal?.Identity?.Name);
            
            if (entity.AspNetException != null) return entity;
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при удалении модели || Модель не найдена || Модель: < {typeof(Room)} > || ID: < {roomId} >");
            
            return await _roomRepository.DeleteEntityAsync(entity.Data, principal?.Identity?.Name);
        }

        public bool ExistEntityByName(string name, int? Tid = null) => _roomRepository.ExistEntityByName(name, Tid);

        public async Task<OrganizationEntityActionResult<IEnumerable<RoomDTO>>> GetAllAsync(ClaimsPrincipal principal)
        {
            var rooms = await _roomRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            
            if (rooms.AspNetException != null)
                return OrganizationEntityActionResult<IEnumerable<RoomDTO>>.Fail(null, rooms.Errors, rooms.AspNetException);
            if (!rooms.Data.Any())
                return OrganizationEntityActionResult<IEnumerable<RoomDTO>>
                    .Fail(null, new[] { TypeOfErrors.NoContent }, $"Запрос выполнен. Данных нет");
            
            var response = _mapper.Map<IEnumerable<RoomDTO>>(rooms.Data);
            return OrganizationEntityActionResult<IEnumerable<RoomDTO>>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<RoomDTO>> GetByIdAsync(int roomId, ClaimsPrincipal principal)
        {
            var result = await _roomRepository.GetEntityByIdAsync(roomId, principal?.Identity?.Name);
            
            if (result.AspNetException != null)
                return OrganizationEntityActionResult<RoomDTO>.Fail(null, result.Errors, result.AspNetException);
            if (result.Data == null)
                return OrganizationEntityActionResult<RoomDTO>
                    .Fail(null, new[] { TypeOfErrors.NotFound }, $"Модель не найдена || Модель: < {typeof(Room)} > || ID: < {roomId} >");
            
            var response = _mapper.Map<RoomDTO>(result.Data);
            return OrganizationEntityActionResult<RoomDTO>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<CreateOrEditRoomModel>> GetCreateRoomAsync(ClaimsPrincipal principal)
        {
            var buildings = await _buildingRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (buildings.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail(null, buildings.Errors, buildings.AspNetException);
            if (!buildings.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail
                    (null, new[] { TypeOfErrors.NotExistBuilding }, $"Произошла ошибка при запросе на получение всех подразделений || данные не найдены");
            
            var depts = await _departamentRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (depts.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail(null, depts.Errors, depts.AspNetException);
            if (!depts.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail
                    (null, new[] { TypeOfErrors.NotExistDepartament }, $"Произошла ошибка при запросе на получение всех отделений, данные не найдены");

            var buildingModels = _mapper.Map<List<BuildingDTO>>(buildings.Data);
            var deptModels = _mapper.Map<List<DepartmentDTO>>(depts.Data);

            var result = new CreateOrEditRoomModel(buildingModels, deptModels);
            return OrganizationEntityActionResult<CreateOrEditRoomModel>.IsSuccess(result);
        }

        public async Task<OrganizationEntityActionResult<CreateOrEditRoomModel>> GetUpdateRoomAsync(int roomId, ClaimsPrincipal principal)
        {
            var room = await _roomRepository.GetEntityByIdAsync(roomId, principal?.Identity?.Name);
            if (room.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail(null, room.Errors, room.AspNetException);
            if (room.Data == null)
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail
                    (null, new[] { TypeOfErrors.NotFound }, $"Произошла ошибка поиске комнаты || данные не найдены || ID < {roomId} >");
            
            var buildings = await _buildingRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (buildings.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail(null, buildings.Errors, buildings.AspNetException);
            if (!buildings.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail
                    (null, new[] { TypeOfErrors.NotExistBuilding }, $"Произошла ошибка при запросе на получение всех подразделений || данные не найдены");

            var depts = await _departamentRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (depts.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail(null, depts.Errors, depts.AspNetException);
            if (!depts.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditRoomModel>.Fail
                    (null, new[] { TypeOfErrors.NotExistDepartament }, $"Произошла ошибка при запросе на получение всех отделений, данные не найдены");

            var buildingModels = _mapper.Map<List<BuildingDTO>>(buildings.Data);
            var deptModels = _mapper.Map<List<DepartmentDTO>>(depts.Data);

            var result = new CreateOrEditRoomModel(buildingModels, deptModels, room.Data);
            return OrganizationEntityActionResult<CreateOrEditRoomModel>.IsSuccess(result);
        }

        public async Task<OrganizationEntityActionResult> UpdateAsync(RoomDTO model, ClaimsPrincipal principal)
        {
            var entity = await _roomRepository.GetEntityByIdAsync(model.Id, principal?.Identity?.Name);
            if (entity.AspNetException != null)
                return OrganizationEntityActionResult.Fail(entity.Errors, entity.AspNetException);
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при изменении модели, модель не найдена || Модель: < {typeof(Room)} > || ID: < {model.Id} >");
            
            var dept = await _departamentRepository.GetEntityByIdAsync(model.DepartamentId, principal?.Identity?.Name);
            var building = await _buildingRepository.GetEntityByIdAsync(model.BuildingId, principal?.Identity?.Name);

            if (dept.AspNetException != null) return dept;
            else if (dept.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistDepartament },
                    $"Произошла ошибка при добавлении модели, < {typeof(Room)} > || Не найден департамент || ID <{model.DepartamentId}>");
            else entity.Data.Departament = dept.Data;

            if (building.AspNetException != null) return building;
            else if (building.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistBuilding },
                    $"Произошла ошибка при добавлении модели, < {typeof(Room)} > || Не найдены данные о здании || ID <{model.BuildingId}>");
            else entity.Data.Building = building.Data;

            entity.Data.CurrentCountSocket = model.CurrentCountSocket;
            entity.Data.Floor = model.Floor;
            entity.Data.RequiredCountSocket = model.RequiredCountSocket;
            entity.Data.Name = model.Name;

            return await _roomRepository.UpdateEntityAsync(entity.Data, principal?.Identity?.Name);
        }
    }
}
