using AutoMapper;
using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.BuildingModels;
using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.Models.EmployeeModels;
using OrganizationEntity.Core.Models.EmployeePhotoModels;
using OrganizationEntity.Core.Models.PositionModels;
using OrganizationEntity.Core.Models.RoomModels;
using OrganizationEntity.Core.Models.SubdivisionModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.BuildingViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.DepartmentViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.EmployeePhotoViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.EmployeeViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.PositionViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.RoomViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.SubdivisionViewModels;

namespace OrganizationEntity.WebHost.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Employee

            CreateMap<Employee, EmployeeDTO>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Surname", dest => dest.MapFrom(src => src.Surname))
                .ForMember("Patronymic", dest => dest.MapFrom(src => src.Patronymic))
                .ForMember("Departament", dest => dest.MapFrom(src => src.Departament))
                .ForMember("Position", dest => dest.MapFrom(src => src.Position))
                .ForMember("WorkTelephone", dest => dest.MapFrom(src => src.WorkTelephone))
                .ForMember("MobileTelephone", dest => dest.MapFrom(src => src.MobileTelephone))
                .ForMember("Mail", dest => dest.MapFrom(src => src.Mail))
                .ForMember("UserName", dest => dest.MapFrom(src => src.User))
                .ForMember("Photo", dest => dest.MapFrom(src => src.Photo));
            CreateMap<EmployeeDTO, Employee>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Surname", dest => dest.MapFrom(src => src.Surname))
                .ForMember("Patronymic", dest => dest.MapFrom(src => src.Patronymic))
                .ForMember("Departament", dest => dest.MapFrom(src => src.Departament))
                .ForMember("Position", dest => dest.MapFrom(src => src.Position))
                .ForMember("WorkTelephone", dest => dest.MapFrom(src => src.WorkTelephone))
                .ForMember("MobileTelephone", dest => dest.MapFrom(src => src.MobileTelephone))
                .ForMember("Mail", dest => dest.MapFrom(src => src.Mail))
                .ForMember("User", dest => dest.MapFrom(src => src.UserName))
                .ForMember("Photo", dest => dest.MapFrom(src => src.Photo));

            #endregion

            #region Departament

            CreateMap<Department, DepartmentDTO>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Subdivision", dest => dest.MapFrom(src => src.Subdivision));
            CreateMap<DepartmentDTO, Department>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Subdivision", dest => dest.MapFrom(src => src.Subdivision));

            #endregion

            #region Building

            CreateMap<Building, BuildingDTO>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Address", dest => dest.MapFrom(src => src.Address))
                .ForMember("Floor", dest => dest.MapFrom(src => src.Floor));
            CreateMap<BuildingDTO, Building>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Address", dest => dest.MapFrom(src => src.Address))
                .ForMember("Floor", dest => dest.MapFrom(src => src.Floor));

            #endregion

            #region EmployeePhoto

            CreateMap<EmployeePhoto, EmployeePhotoDTO>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Photo", dest => dest.MapFrom(src => src.Photo));
            CreateMap<EmployeePhotoDTO, EmployeePhoto>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Photo", dest => dest.MapFrom(src => src.Photo));

            #endregion

            #region Position

            CreateMap<Position, PositionDTO>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name));
            CreateMap<PositionDTO, Position>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name));

            #endregion

            #region Room

            CreateMap<Room, RoomDTO>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Building", dest => dest.MapFrom(src => src.Building))
                .ForMember("Departament", dest => dest.MapFrom(src => src.Departament))
                .ForMember("Floor", dest => dest.MapFrom(src => src.Floor))
                .ForMember("RequiredCountSocket", dest => dest.MapFrom(src => src.RequiredCountSocket))
                .ForMember("CurrentCountSocket", dest => dest.MapFrom(src => src.CurrentCountSocket));
            CreateMap<RoomDTO, Room>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name))
                .ForMember("Building", dest => dest.MapFrom(src => src.Building))
                .ForMember("Departament", dest => dest.MapFrom(src => src.Departament))
                .ForMember("Floor", dest => dest.MapFrom(src => src.Floor))
                .ForMember("RequiredCountSocket", dest => dest.MapFrom(src => src.RequiredCountSocket))
                .ForMember("CurrentCountSocket", dest => dest.MapFrom(src => src.CurrentCountSocket));

            #endregion

            #region Subdivision

            CreateMap<Subdivision, SubdivisionDTO>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name));
            CreateMap<SubdivisionDTO, Subdivision>()
                .ForMember("Id", dest => dest.MapFrom(src => src.Id))
                .ForMember("Name", dest => dest.MapFrom(src => src.Name));

            #endregion


            #region BuildingViewModel

            CreateMap<BuildingDTO, BuildingViewModel>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.Address, dest => dest.MapFrom(src => src.Address))
                .ForMember(x => x.Floor, dest => dest.MapFrom(src => src.Floor));

            #endregion

            #region DepartmentViewModel

            CreateMap<DepartmentDTO, DepartmentViewModel>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.Subdivision, dest => dest.MapFrom(src => src.Subdivision));

            CreateMap<CreateOrEditDeptModel, CreateDepartmentViewModel>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.SelectedSubdivision, dest => dest.MapFrom(src => src.SelectedSubdivision));

            CreateMap<CreateOrEditDeptModel, UpdateDepartmentViewModel>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.SelectedSubdivision, dest => dest.MapFrom(src => src.SelectedSubdivision));

            #endregion

            #region SubdivisionViewModel

            CreateMap<SubdivisionDTO, SubdivisionViewModel>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id));

            #endregion

            #region EmployeeViewModel

            CreateMap<EmployeeDTO, EmployeeViewModel>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.Departament, dest => dest.MapFrom(src => src.Departament))
                .ForMember(x => x.Mail, dest => dest.MapFrom(src => src.Mail))
                .ForMember(x => x.MobileTelephone, dest => dest.MapFrom(src => src.MobileTelephone))
                .ForMember(x => x.Patronymic, dest => dest.MapFrom(src => src.Patronymic))
                .ForMember(x => x.Photo, dest => dest.MapFrom(src => src.Photo))
                .ForMember(x => x.Position, dest => dest.MapFrom(src => src.Position))
                .ForMember(x => x.Surname, dest => dest.MapFrom(src => src.Surname))
                .ForMember(x => x.User, dest => dest.MapFrom(src => src.UserName))
                .ForMember(x => x.WorkTelephone, dest => dest.MapFrom(src => src.WorkTelephone));

            CreateMap<EmployeePhotoDTO, EmployeePhotoViewModel>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.Photo, dest => dest.MapFrom(src => src.Photo));

            CreateMap<CreateEmployeeViewModel, EmployeeDTO>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.DepartamentId, dest => dest.MapFrom(src => src.DepartamentId))
                .ForMember(x => x.Mail, dest => dest.MapFrom(src => src.Mail))
                .ForMember(x => x.MobileTelephone, dest => dest.MapFrom(src => src.MobileTelephone))
                .ForMember(x => x.Patronymic, dest => dest.MapFrom(src => src.Patronymic))
                .ForMember(x => x.PositionId, dest => dest.MapFrom(src => src.PositionId))
                .ForMember(x => x.Surname, dest => dest.MapFrom(src => src.Surname))
                .ForMember(x => x.UserName, dest => dest.MapFrom(src => src.User))
                .ForMember(x => x.WorkTelephone, dest => dest.MapFrom(src => src.WorkTelephone));

            CreateMap<UpdateEmployeeViewModel, EmployeeDTO>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.DepartamentId, dest => dest.MapFrom(src => src.DepartamentId))
                .ForMember(x => x.Mail, dest => dest.MapFrom(src => src.Mail))
                .ForMember(x => x.MobileTelephone, dest => dest.MapFrom(src => src.MobileTelephone))
                .ForMember(x => x.Patronymic, dest => dest.MapFrom(src => src.Patronymic))
                .ForMember(x => x.PositionId, dest => dest.MapFrom(src => src.PositionId))
                .ForMember(x => x.Surname, dest => dest.MapFrom(src => src.Surname))
                .ForMember(x => x.UserName, dest => dest.MapFrom(src => src.User))
                .ForMember(x => x.WorkTelephone, dest => dest.MapFrom(src => src.WorkTelephone));

            CreateMap<CreateOrEditEmployeeDTO, CreateEmployeeViewModel>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.DepartamentId, dest => dest.MapFrom(src => src.DepartamentId))
                .ForMember(x => x.Mail, dest => dest.MapFrom(src => src.Mail))
                .ForMember(x => x.MobileTelephone, dest => dest.MapFrom(src => src.MobileTelephone))
                .ForMember(x => x.Patronymic, dest => dest.MapFrom(src => src.Patronymic))
                .ForMember(x => x.PositionId, dest => dest.MapFrom(src => src.PositionId))
                .ForMember(x => x.Surname, dest => dest.MapFrom(src => src.Surname))
                .ForMember(x => x.User, dest => dest.MapFrom(src => src.UserName))
                .ForMember(x => x.SelectPosition, dest => dest.MapFrom(src => src.SelectPosition))
                .ForMember(x => x.SelectDepartment, dest => dest.MapFrom(src => src.SelectDepartment))
                .ForMember(x => x.WorkTelephone, dest => dest.MapFrom(src => src.WorkTelephone));

            CreateMap<CreateOrEditEmployeeDTO, UpdateEmployeeViewModel>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.DepartamentId, dest => dest.MapFrom(src => src.DepartamentId))
                .ForMember(x => x.Mail, dest => dest.MapFrom(src => src.Mail))
                .ForMember(x => x.MobileTelephone, dest => dest.MapFrom(src => src.MobileTelephone))
                .ForMember(x => x.Patronymic, dest => dest.MapFrom(src => src.Patronymic))
                .ForMember(x => x.PositionId, dest => dest.MapFrom(src => src.PositionId))
                .ForMember(x => x.Surname, dest => dest.MapFrom(src => src.Surname))
                .ForMember(x => x.User, dest => dest.MapFrom(src => src.UserName))
                .ForMember(x => x.SelectPosition, dest => dest.MapFrom(src => src.SelectPosition))
                .ForMember(x => x.SelectDepartment, dest => dest.MapFrom(src => src.SelectDepartment))
                .ForMember(x => x.WorkTelephone, dest => dest.MapFrom(src => src.WorkTelephone));

            #endregion

            #region PositionViewModel

            CreateMap<PositionDTO, PositionViewModel>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name));

            #endregion

            #region RoomViewModel

            CreateMap<RoomDTO, RoomViewModel>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.Building, dest => dest.MapFrom(src => src.Building))
                .ForMember(x => x.RequiredCountSocket, dest => dest.MapFrom(src => src.RequiredCountSocket))
                .ForMember(x => x.CurrentCountSocket, dest => dest.MapFrom(src => src.CurrentCountSocket))
                .ForMember(x => x.Floor, dest => dest.MapFrom(src => src.Floor))
                .ForMember(x => x.Departament, dest => dest.MapFrom(src => src.Departament));

            CreateMap<CreateRoomViewModel, RoomDTO>()
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.BuildingId, dest => dest.MapFrom(src => src.BuildingId))
                .ForMember(x => x.RequiredCountSocket, dest => dest.MapFrom(src => src.RequiredCountSocket))
                .ForMember(x => x.Floor, dest => dest.MapFrom(src => src.Floor))
                .ForMember(x => x.CurrentCountSocket, dest => dest.MapFrom(src => src.CurrentCountSocket))
                .ForMember(x => x.DepartamentId, dest => dest.MapFrom(src => src.DepartamentId));

            CreateMap<UpdateRoomViewModel, RoomDTO>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Id))
                .ForMember(x => x.Name, dest => dest.MapFrom(src => src.Name))
                .ForMember(x => x.BuildingId, dest => dest.MapFrom(src => src.BuildingId))
                .ForMember(x => x.RequiredCountSocket, dest => dest.MapFrom(src => src.RequiredCountSocket))
                .ForMember(x => x.Floor, dest => dest.MapFrom(src => src.Floor))
                .ForMember(x => x.CurrentCountSocket, dest => dest.MapFrom(src => src.CurrentCountSocket))
                .ForMember(x => x.DepartamentId, dest => dest.MapFrom(src => src.DepartamentId));

            CreateMap<CreateOrEditRoomModel, CreateRoomViewModel>()
                .ForMember(x => x.SelectBuilding, dest => dest.MapFrom(src => src.SelectedBuilding))
                .ForMember(x => x.SelectDepartment, dest => dest.MapFrom(src => src.SelectedDepartament))
                .ForMember(x => x.RequiredCountSocket, dest => dest.MapFrom(src => src.RequiredCountSocket))
                .ForMember(x => x.Floor, dest => dest.MapFrom(src => src.Floor))
                .ForMember(x => x.CurrentCountSocket, dest => dest.MapFrom(src => src.CurrentCountSocket))
                .ForMember(x => x.DepartamentId, dest => dest.MapFrom(src => src.DepartamentId));

            CreateMap<CreateOrEditRoomModel, UpdateRoomViewModel>()
                .ForMember(x => x.SelectBuilding, dest => dest.MapFrom(src => src.SelectedBuilding))
                .ForMember(x => x.SelectDepartment, dest => dest.MapFrom(src => src.SelectedDepartament))
                .ForMember(x => x.RequiredCountSocket, dest => dest.MapFrom(src => src.RequiredCountSocket))
                .ForMember(x => x.Floor, dest => dest.MapFrom(src => src.Floor))
                .ForMember(x => x.CurrentCountSocket, dest => dest.MapFrom(src => src.CurrentCountSocket))
                .ForMember(x => x.DepartamentId, dest => dest.MapFrom(src => src.DepartamentId));

            #endregion
        }
    }
}