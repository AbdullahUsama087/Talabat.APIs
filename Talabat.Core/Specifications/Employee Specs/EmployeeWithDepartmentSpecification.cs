using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Employee_Specs
{
    public class EmployeeWithDepartmentSpecification : BaseSpecification<Employee>
    {
        public EmployeeWithDepartmentSpecification()
        {
            Includes.Add(E => E.Department);
        }

        public EmployeeWithDepartmentSpecification(int id) : base(E => E.Id == id)
        {
            Includes.Add(E => E.Department);
        }
    }
}
