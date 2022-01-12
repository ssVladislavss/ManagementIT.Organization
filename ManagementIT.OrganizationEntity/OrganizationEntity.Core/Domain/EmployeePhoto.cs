using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationEntity.Core.Domain
{
    public class EmployeePhoto : BaseEntity
    {
        [NotMapped]
        public override string Name { get; set; }
        public byte[] Photo { get; set; }
    }
}