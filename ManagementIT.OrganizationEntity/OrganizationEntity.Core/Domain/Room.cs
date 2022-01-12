namespace OrganizationEntity.Core.Domain
{
    public class Room : BaseEntity
    {
        public Building Building { get; set; }
        public Department Departament { get; set; }
        public int Floor { get; set; }
        public int RequiredCountSocket { get; set; }
        public int CurrentCountSocket { get; set; }
    }
}