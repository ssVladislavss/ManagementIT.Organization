namespace OrganizationEntity.Core.Domain
{
    public class Department : BaseEntity
    {
        public Subdivision Subdivision { get; set; }
    }
}