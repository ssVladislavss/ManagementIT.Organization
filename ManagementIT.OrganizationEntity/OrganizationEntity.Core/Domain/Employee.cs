namespace OrganizationEntity.Core.Domain
{
    public class Employee : BaseEntity
    {
        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public Department Departament { get; set; }

        public Position Position { get; set; }

        public string WorkTelephone { get; set; }

        public string MobileTelephone { get; set; }

        public string Mail { get; set; }

        public string User { get; set; }

        public EmployeePhoto Photo { get; set; }
    }
}