namespace OrganizationEntity.Core.Domain
{
    public class Building : BaseEntity
    {
        public string Address { get; set; }
        public int Floor { get; set; }
    }
}