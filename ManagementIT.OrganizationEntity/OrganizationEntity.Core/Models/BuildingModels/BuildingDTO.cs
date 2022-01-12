namespace OrganizationEntity.Core.Models.BuildingModels
{
    public class BuildingDTO : BaseEntity
    {
        public string Address { get; set; }
        public int Floor { get; set; }

        public BuildingDTO() { }
        public BuildingDTO(string name, string address, int floor, int id = 0)
        {
            Id = id;
            Name = name;
            Address = address;
            Floor = floor;
        }

        public static BuildingDTO GetBuildingDTO(string name, string address, int floor, int id = 0) =>
            new BuildingDTO(name, address, floor, id);
    }
}