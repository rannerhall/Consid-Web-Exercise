using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsidTest.Utils
{
    public class CalculateSalary
    {
        private const decimal EmployeeSalaryCofficient = 1.125m;
        private const decimal ManagerSalaryCofficient = 1.725m;
        private const decimal CEOSalaryCofficient = 2.725m;

        public decimal Calculate(Employee employee, int rank)
        {
            decimal CofficientSalary = 0m;
            

            if (employee.Role.Equals("Employee"))
            {
                CofficientSalary = (decimal)(employee.Salary * (EmployeeSalaryCofficient * rank));

            }
            else if (employee.Role.Equals("Manager"))
            {
                CofficientSalary = (decimal)(employee.Salary * (ManagerSalaryCofficient * rank));

            }
            else if (employee.Role.Equals("CEO"))
            {
                CofficientSalary = (decimal)(employee.Salary * (CEOSalaryCofficient * rank));
            }
            return CofficientSalary;
        }
    }
}