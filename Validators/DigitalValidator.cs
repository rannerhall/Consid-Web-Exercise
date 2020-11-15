using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsidTest.Validators
{
    public class DigitalValidator : AbstractValidator<LibraryItem>
    {
        public DigitalValidator()
        {
            RuleFor(item => item.Title).NotEmpty();
            RuleFor(item => item.RunTimeMinutes).NotEmpty();
            RuleFor(item => item.Type).NotEmpty();
            RuleFor(item => item.CategoryId).NotEmpty();
        }
    }
}