namespace OrganizationEntity.Core.Models.PositionModels
{
    public class PositionDTO : BaseEntity
    {
        public PositionDTO() { }

        public PositionDTO(string name, int id = 0)
        {
            Id = id;
            Name = name;
        }

        public static PositionDTO GetPositionDTO(string name, int id = 0) =>
            new PositionDTO(name, id);
    }
}