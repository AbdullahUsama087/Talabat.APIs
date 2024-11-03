using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications.Employee_Specs;

namespace Talabat.APIs.Controllers
{

    public class EmployeesController : BaseAPIController
    {
        private readonly IGenericRepository<Employee> _employeeRepo;

        public EmployeesController(IGenericRepository<Employee> employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        [HttpGet] // GET : api/Employees
        public async Task<ActionResult<IReadOnlyList<Employee>>> GetEmployees()
        {
            var spec = new EmployeeWithDepartmentSpecification();

            var employees = await _employeeRepo.GetAllWithSpecAsync(spec);

            return Ok(employees);
        }

        [HttpGet("{id}")] // GET : api/Employees/1
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var spec = new EmployeeWithDepartmentSpecification(id);

            var employee = await _employeeRepo.GetEntityWithSpecAsync(spec);

            return Ok(employee);
        }
    }
}
