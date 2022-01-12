using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Enums;
using OrganizationEntity.Core.Abstractions.OrganizationEntityRepository;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.Models.EmployeeModels;
using OrganizationEntity.Core.Models.ForApplicationModels;
using OrganizationEntity.Core.Models.RoomModels;
using OrganizationEntity.Core.ResponseModels;

namespace OrganizationEntity.DataAccess.Service
{
    public class DependencyEntityService : IDependencyEntityService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public DependencyEntityService(IDepartmentRepository departmentRepository, IRoomRepository roomRepository, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>> GetDependencyForApplication(ClaimsPrincipal principal)
        {
            var employees = await _employeeRepository.GetAllEmployeeAsync();
            if(employees.AspNetException != null)
                return OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>.Fail(null, employees.Errors, employees.AspNetException);
            if(!employees.Data.Any())
                return OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>.Fail(null, new[] { TypeOfErrors.NotExistEmployee },
                    $"Невозможно выполнить запрос || Не найдено ниодного сотрудника || Запрос на создание заявки");

            var depts = await _departmentRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (depts.AspNetException != null)
                return OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>.Fail(null, depts.Errors, depts.AspNetException);
            if (!depts.Data.Any())
                return OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>.Fail(null, new[] { TypeOfErrors.NotExistDepartament },
                    $"Невозможно выполнить запрос || Не найдено ниодного отделения || Запрос на создание заявки");

            var rooms = await _roomRepository.GetAllEntitiesAsync(principal?.Identity?.Name);
            if (rooms.AspNetException != null)
                return OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>.Fail(null, rooms.Errors, rooms.AspNetException);
            if (!rooms.Data.Any())
                return OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>.Fail(null, new[] { TypeOfErrors.NotExistRoom },
                    $"Невозможно выполнить запрос || Не найдено ниодной комнаты || Запрос на создание заявки");

            var employeeDTO = _mapper.Map<List<EmployeeDTO>>(employees.Data);
            var deptDTO = _mapper.Map<List<DepartmentDTO>>(depts.Data);
            var roomDTO = _mapper.Map<List<RoomDTO>>(rooms.Data);

            var response = new GetCreateOrUpdateApplicationDTO(roomDTO, deptDTO, employeeDTO);
            return OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>.IsSuccess(response);
        }
    }
}