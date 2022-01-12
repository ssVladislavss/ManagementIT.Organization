namespace OrganizationEntity.Core.Models.SubdivisionModels
{
    public class SubdivisionDTO : BaseEntity
    {
        public SubdivisionDTO() { }
        public SubdivisionDTO(string name, int id = 0)
        {
            Id = id;
            Name = name;
        }

        public static SubdivisionDTO GetSubdivisionDTO(string name, int id = 0) =>
            new SubdivisionDTO(name, id);
    }
}