using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Enums;
using Microsoft.AspNetCore.Http;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.OrganizationEntityRepository;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.Core.Constants;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.Models.EmployeeModels;
using OrganizationEntity.Core.Models.LogMessageModels;
using OrganizationEntity.Core.Models.PositionModels;
using OrganizationEntity.Core.ResponseModels;

namespace OrganizationEntity.DataAccess.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogService _logService;
        private IMapper _mapper;
        private readonly IGenericRepository<EmployeePhoto> _photoRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IGenericRepository<Position> _positionRepository;

        public EmployeeService(IEmployeeRepository employeeRepository,
                               ILogService logService, IMapper mapper,
                               IGenericRepository<EmployeePhoto> photoRepository,
                               IDepartmentRepository departmentRepository,
                               IGenericRepository<Position> positionRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _photoRepository = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));
            _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
            _positionRepository = positionRepository ?? throw new ArgumentNullException(nameof(positionRepository));
        }

        public async Task<OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>> GetAllAsync(ClaimsPrincipal principal)
        {
            var employees = await _employeeRepository.GetAllEmployeeAsync();
            if (employees.AspNetException != null)
                return OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>
                    .Fail(null, employees.Errors, employees.AspNetException);

            if (!employees.Data.Any())
                return OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>
                    .Fail(null, new[] { TypeOfErrors.NoContent }, $"Не найдено ниодной модели || Модель: < {typeof(Employee)} >");
            
            var response = _mapper.Map<IEnumerable<EmployeeDTO>>(employees.Data);
            return OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>> GetByDeptIdAsync(int deptId, ClaimsPrincipal principal)
        {
            var employees = await _employeeRepository.GetEmployeesByDepartamentAsync(deptId, principal?.Identity?.Name);
            if (employees.AspNetException != null)
                return OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>.Fail(null, employees.Errors, employees.AspNetException);

            if (!employees.Data.Any())
                return OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>
                    .Fail(null, new[] { TypeOfErrors.NoContent },
                    $"Не найдено ниодной модели || Модель: < {typeof(Employee)} > || Входной параметр: int deptId < {deptId} >");
            
            var response = _mapper.Map<IEnumerable<EmployeeDTO>>(employees.Data);
            return OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<EmployeeDTO>> GetByIdAsync(int employeeId, ClaimsPrincipal principal)
        {
            var result = await _employeeRepository.GetEntityByIdAsync(employeeId, principal?.Identity?.Name);
            
            if (result.AspNetException != null)
                return OrganizationEntityActionResult<EmployeeDTO>.Fail(null, result.Errors, result.AspNetException);
            if (result.Data == null)
                return OrganizationEntityActionResult<EmployeeDTO>
                    .Fail(null, new[] { TypeOfErrors.NotFound }, $"Модель не найдена || Модель: < {typeof(Employee)} > || Входной параметр: int employeeId < {employeeId} >");
            
            var response = _mapper.Map<EmployeeDTO>(result.Data);
            return OrganizationEntityActionResult<EmployeeDTO>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult> AddAsync(EmployeeDTO model, ClaimsPrincipal principal)
        {
            var entity = _mapper.Map<Employee>(model);

            var position = await _positionRepository.GetEntityByIdAsync(model.PositionId, principal?.Identity?.Name);
            var dept = await _departmentRepository.GetEntityByIdAsync(model.DepartamentId, principal?.Identity?.Name);

            if (position.AspNetException != null) return position;
            else if (position.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistPosition },
                    $"Произошла ошибка при добавлении сотрудника || Не найдена модель || Модель: < {typeof(Position)} > || ID: < {model.PositionId} >");
            else entity.Position = position.Data;

            if (dept.AspNetException != null) return dept;
            else if (dept.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistDepartament },
                    $"Произошла ошибка при добавлении сотрудника || Не найдена модель || Модель: < {typeof(Department)} > || ID: < {model.DepartamentId} >");
            else entity.Departament = dept.Data;

            return await _employeeRepository.AddEntityAsync(entity, principal?.Identity?.Name); 
        }

        public async Task<OrganizationEntityActionResult> UpdatePhoto(int employeeId, byte[] photo, ClaimsPrincipal principal)
        {
            var employee = await _employeeRepository.GetEntityByIdAsync(employeeId, principal?.Identity?.Name);
            if (employee.AspNetException != null)
                return OrganizationEntityActionResult.Fail(employee.Errors, employee.AspNetException);
            if (employee.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при добавлении фотографии || Модель не найдена || Модель: < {typeof(Employee)} > || ID: < {employeeId} >");

            EmployeePhoto employeePhoto = new EmployeePhoto();
            employeePhoto.Photo = photo;

            var resultPhoto = await _photoRepository.AddEntityAsync(employeePhoto, principal?.Identity?.Name);

            if (!resultPhoto.Success)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.ErrorAddingPhoto }, "Ошибка при добавлении фотографии сотрудника");

            EmployeePhoto nowPhoto = null;
            if (employee.Data.Photo != null)
                nowPhoto = employee.Data.Photo;
            
            employee.Data.Photo = employeePhoto;

            var result = await _employeeRepository.UpdateEntityAsync(employee.Data, principal?.Identity?.Name);
            if (!result.Success)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.UpdateEntityError },
                    $"Ошибка при добавлении фотографии || Модель: < {typeof(EmployeePhoto)} > || photoId: < {employee.Data.Photo.Id} >");
            
            if(nowPhoto != null) await _photoRepository.DeleteEntityAsync(nowPhoto, principal?.Identity?.Name);

            return OrganizationEntityActionResult.IsSuccess();
        }

        public async Task<OrganizationEntityActionResult> UpdateAsync(EmployeeDTO model, ClaimsPrincipal principal)
        {
            var entity = await _employeeRepository.GetEntityByIdAsync(model.Id, principal?.Identity?.Name);
            
            if (entity.AspNetException != null)
                return OrganizationEntityActionResult.Fail(entity.Errors, entity.AspNetException);
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при изменении модели || Модель не найдена || Модель: < {typeof(Employee)} > || ID: < {model.Id} >");
            
            var position = await _positionRepository.GetEntityByIdAsync(model.PositionId, principal?.Identity?.Name);
            var dept = await _departmentRepository.GetEntityByIdAsync(model.DepartamentId, principal?.Identity?.Name);

            if (position.AspNetException != null) return position;
            else if (position.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistPosition },
                    $"Ошибка при обновлении Модели || Не найдена позиция || Модель: < {typeof(Position)} > || positionID < {model.PositionId} >");
            else entity.Data.Position = position.Data;

            if (dept.AspNetException != null) return dept;
            else if (dept.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistDepartament },
                    $"Ошибка при обновлении Модели || Не найдено отделение || Модель: < {typeof(Department)} > || departmentID < {model.DepartamentId} >");
            else entity.Data.Departament = dept.Data;

            entity.Data.Surname = model.Surname;
            entity.Data.Name = model.Name;
            entity.Data.Patronymic = model.Patronymic;
            entity.Data.Mail = model.Mail;
            entity.Data.User = model.UserName;
            entity.Data.MobileTelephone = model.MobileTelephone;
            entity.Data.WorkTelephone = model.WorkTelephone;

            return await _employeeRepository.UpdateEntityAsync(entity.Data, principal?.Identity?.Name);
        }

        public async Task<OrganizationEntityActionResult> DeleteAsync(int employeeId, ClaimsPrincipal principal)
        {
            var entity = await _employeeRepository.GetEntityByIdAsync(employeeId, principal?.Identity?.Name);
            if (entity.AspNetException != null)
                return OrganizationEntityActionResult.Fail(entity.Errors, entity.AspNetException);
            if (entity.Data == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotFound },
                    $"Ошибка при удалении модели || Модель не найдена || Модель: < {typeof(Employee)} > || ID: < {employeeId} >");
            
            EmployeePhoto photo = null;
            if (entity.Data.Photo != null) photo = entity.Data.Photo;

            var result = await _employeeRepository.DeleteEntityAsync(entity.Data, principal?.Identity?.Name);
            if (!result.Success)
                return result;

            if (photo != null) await _photoRepository.DeleteEntityAsync(photo, principal?.Identity?.Name);
            return OrganizationEntityActionResult.IsSuccess();
        }

        public async Task<OrganizationEntityActionResult> DeletePhotoAsync(int employeeId, ClaimsPrincipal principal)
        {
            var employee = await _employeeRepository.GetEntityByIdAsync(employeeId, principal?.Identity?.Name);
            
            if (employee.AspNetException != null)
                return OrganizationEntityActionResult.Fail(employee.Errors, employee.AspNetException);
            if (employee.Data == null)
                return OrganizationEntityActionResult.Fail(new [] {TypeOfErrors.NotFound},
                    $"Модель не найдена || Модель: < {typeof(Employee)} > || ID: < {employeeId} >");
            
            if (employee.Data.Photo == null)
                return OrganizationEntityActionResult.Fail(new[] { TypeOfErrors.NotExistFile },
                    $"У сотрудника нет привязанной фотографии || Модель: < {typeof(Employee)} > || ID: < {employeeId} > || employee.Photo = null");
            
            return await _photoRepository.DeleteEntityAsync(employee.Data.Photo, principal?.Identity?.Name);
        }

        public async Task<OrganizationEntityActionResult<CreateOrEditEmployeeDTO>> GetCreateAsync(ClaimsPrincipal principal)
        {
            var depts = await _departmentRepository.GetAllEntitiesAsync(principal?.Identity?.Name);

            if (depts.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, depts.Errors, depts.AspNetException);
            if (!depts.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, new[] { TypeOfErrors.NotExistDepartament },
                    $"Невозможно выполнить запрос || Не найдено ниодного отделения || Запрос на создание сотрудника");
            
            var positions = await _positionRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (positions.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, positions.Errors, positions.AspNetException);
            if (!positions.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, new[] { TypeOfErrors.NotExistPosition },
                    $"Невозможно выполнить запрос || Не найдено ниодной позиции || Запрос на создание сотрудника");
            
            var deptModel = _mapper.Map<List<DepartmentDTO>>(depts.Data);
            var positionsModel = _mapper.Map<List<PositionDTO>>(positions.Data);
            var response = new CreateOrEditEmployeeDTO(deptModel, positionsModel);
            
            return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<CreateOrEditEmployeeDTO>> GetUpdateAsync(int employeeId, ClaimsPrincipal principal)
        {
            var employee = await _employeeRepository.GetEntityByIdAsync(employeeId, principal?.Identity?.Name);
            if (employee.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, employee.Errors, employee.AspNetException);
            if (employee.Data == null)
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, new[] { TypeOfErrors.NotFound },
                    $"Невозможно выполнить запрос || Сотрудник не найден || Id < {employeeId} > || Запрос на изменение сотрудника");

            var depts = await _departmentRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (depts.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, depts.Errors, depts.AspNetException);
            if (!depts.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, new[] { TypeOfErrors.NotExistDepartament },
                    $"Невозможно выполнить запрос || Не найдено ниодного отделения || Запрос на изменение сотрудника");

            var positions = await _positionRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (positions.AspNetException != null)
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, positions.Errors, positions.AspNetException);
            if (!positions.Data.Any())
                return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.Fail(null, new[] { TypeOfErrors.NotExistPosition },
                    $"Невозможно выполнить запрос || Не найдено ниодной позиции || Запрос на изменение сотрудника");

            var deptModel = _mapper.Map<List<DepartmentDTO>>(depts.Data);
            var positionsModel = _mapper.Map<List<PositionDTO>>(positions.Data);
            var employeeModel = _mapper.Map<EmployeeDTO>(employee.Data);
            var response = new CreateOrEditEmployeeDTO(deptModel, positionsModel, employeeModel);

            return OrganizationEntityActionResult<CreateOrEditEmployeeDTO>.IsSuccess(response);
        }

        public async Task<OrganizationEntityActionResult<EmployeeDTO>> GetByUserNameAsync(string userName)
        {
            var employee = await _employeeRepository.GetEntityByNameAsync(userName, null);

            if (employee.AspNetException != null) return OrganizationEntityActionResult<EmployeeDTO>
                     .Fail(null, employee.Errors, employee.AspNetException);
            else if (employee.Data == null) return OrganizationEntityActionResult<EmployeeDTO>
                     .Fail(null, new[] { TypeOfErrors.NotFound },
                     $"Поиск модели по UserName || Модель: < {typeof(Employee)} > || Входной параметр UserName: < {userName} > || Модель не найдена");

            var response = _mapper.Map<EmployeeDTO>(employee.Data);
            return OrganizationEntityActionResult<EmployeeDTO>.IsSuccess(response);
        }
    }
}
