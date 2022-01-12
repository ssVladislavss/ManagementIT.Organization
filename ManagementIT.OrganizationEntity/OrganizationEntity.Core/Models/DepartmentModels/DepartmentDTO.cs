using OrganizationEntity.Core.Models.SubdivisionModels;

namespace OrganizationEntity.Core.Models.DepartmentModels
{
    public class DepartmentDTO : BaseEntity
    {
        public int SubdivisionId { get; set; }
        public SubdivisionDTO Subdivision { get; set; }

        public DepartmentDTO(string name, SubdivisionDTO model = null, int id = 0)
        {
            Id = id;
            Name = name;
            Subdivision = model;
        }

        public DepartmentDTO(string name, int subdivisionId = 0, int id = 0)
        {
            Id = id;
            Name = name;
            SubdivisionId = subdivisionId;
        }

        public DepartmentDTO() { }

        public static DepartmentDTO GetDepartamentDTO(string name, SubdivisionDTO model = null, int id = 0) =>
            new DepartmentDTO(name, model, id);
    }
}