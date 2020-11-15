using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsidTest.Validators
{
    public class BookValidator : AbstractValidator<LibraryItem>
    {
        public BookValidator()
        {
            RuleFor(item => item.Title).NotEmpty();
            RuleFor(item => item.Author).NotEmpty();
            RuleFor(item => item.Pages).NotEmpty();
            RuleFor(item => item.Type).NotEmpty();
            RuleFor(item => item.CategoryId).NotEmpty();
        }
    }
}