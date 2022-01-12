namespace OrganizationEntity.Core.Models.EmployeePhotoModels
{
    public class EmployeePhotoDTO : BaseEntity
    {
        public byte[] Photo { get; set; }

        public EmployeePhotoDTO() { }
        public EmployeePhotoDTO(byte[] photo, int id = 0)
        {
            Id = id;
            Photo = photo;
        }

        public static EmployeePhotoDTO GetEmployeePhotoDTO(byte[] photo, int id = 0) =>
            new EmployeePhotoDTO(photo, id);
    }
}