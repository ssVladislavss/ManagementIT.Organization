using OrganizationEntity.Core.Models.SubdivisionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Models.DepartmentModels
{
    public class CreateOrEditDeptModel
    {
        public string Name { get; set; }
        public int SubdivisionId { get; set; }

        public List<SubdivisionDTO> SelectedSubdivision { get; set; }

        public CreateOrEditDeptModel() { }

        public CreateOrEditDeptModel(List<SubdivisionDTO> model, string name = "", int subdivisionId = 0)
        {
            Name = name;
            SubdivisionId = subdivisionId;
            SelectedSubdivision = model;
        }
    }
}
