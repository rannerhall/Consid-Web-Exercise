using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace ConsidTest.Validators
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(item => item.FirstName).NotEmpty();
            RuleFor(item => item.LastName).NotEmpty();
            RuleFor(item => item.Salary).NotEmpty();
        }
    }
}