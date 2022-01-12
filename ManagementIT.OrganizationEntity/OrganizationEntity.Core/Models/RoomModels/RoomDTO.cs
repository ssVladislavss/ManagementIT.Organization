using OrganizationEntity.Core.Models.BuildingModels;
using OrganizationEntity.Core.Models.DepartmentModels;

namespace OrganizationEntity.Core.Models.RoomModels
{
    public class RoomDTO : BaseEntity
    {
        public int BuildingId { get; set; }
        public int DepartamentId { get; set; }
        public BuildingDTO Building { get; set; }
        public DepartmentDTO Departament { get; set; }
        public int Floor { get; set; }
        public int RequiredCountSocket { get; set; }
        public int CurrentCountSocket { get; set; }

        public RoomDTO() { }
        public RoomDTO(string name, int floor, int requiredCountSocket, int currentCountSocket,
            BuildingDTO building = null, DepartmentDTO dept = null, int id = 0)
        {
            Id = id;
            Name = name;
            Departament = dept;
            Building = building;
            Floor = floor;
            RequiredCountSocket = requiredCountSocket;
            CurrentCountSocket = currentCountSocket;
        }
        public RoomDTO(string name, int floor, int requiredCountSocket, int currentCountSocket,
            int buildingId, int deptId, int id = 0)
        {
            Id = id;
            Name = name;
            DepartamentId = deptId;
            BuildingId = buildingId;
            Floor = floor;
            RequiredCountSocket = requiredCountSocket;
            CurrentCountSocket = currentCountSocket;
        }

        public static RoomDTO GetRoomDTO(string name, int floor, int requiredCountSocket, int currentCountSocket,
            BuildingDTO building = null, DepartmentDTO dept = null, int id = 0) =>
            new RoomDTO(name, floor, requiredCountSocket, currentCountSocket, building, dept, id);
    }
}