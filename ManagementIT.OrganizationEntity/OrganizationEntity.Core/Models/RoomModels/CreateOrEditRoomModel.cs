using OrganizationEntity.Core.Domain;
using OrganizationEntity.Core.Models.BuildingModels;
using OrganizationEntity.Core.Models.DepartmentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Models.RoomModels
{
    public class CreateOrEditRoomModel
    {
        public string Name { get; set; }
        public int Floor { get; set; }
        public int BuildingId { get; set; }
        public int DepartamentId { get; set; }
        public int RequiredCountSocket { get; set; }
        public int CurrentCountSocket { get; set; }

        public List<BuildingDTO> SelectedBuilding { get; set; }
        public List<DepartmentDTO> SelectedDepartament { get; set; }

        public CreateOrEditRoomModel() { }

        public CreateOrEditRoomModel(List<BuildingDTO> buildings, List<DepartmentDTO> depts, Room entity = null)
        {
            SelectedDepartament = depts;
            SelectedBuilding = buildings;
            if (entity != null)
            {
                Name = entity.Name;
                Floor = entity.Floor;
                if (entity.Building != null) BuildingId = entity.Building.Id;
                if (entity.Departament != null) DepartamentId = entity.Departament.Id;
                RequiredCountSocket = entity.RequiredCountSocket;
                CurrentCountSocket = entity.CurrentCountSocket;
            }
        }
    }
}
