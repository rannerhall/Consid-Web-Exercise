using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConsidTest.Validators;
using FluentValidation.Results;

namespace ConsidTest.Validators
{
    public class LibraryItemValidator
    {
        private const string Book = "Book";
        private const string DVD = "DVD";
        private const string AudioBook = "Audio Book";
        private const string ReferenceBook = "Reference Book";

        private BookValidator BookValidator = new BookValidator();
        private DigitalValidator DigitalValidator = new DigitalValidator();

        public ValidationResult ValidateLibraryItem(LibraryItem libraryItem)
        {
            ValidationResult results = null;

            if (libraryItem.Type != null)
            {
                if (libraryItem.Type.Equals(Book) || libraryItem.Type.Equals(ReferenceBook))
                {
                    results = BookValidator.Validate(libraryItem);
                }
                else if (libraryItem.Type.Equals(DVD) || libraryItem.Type.Equals(AudioBook))
                {
                    results = DigitalValidator.Validate(libraryItem);
                }
            }
            return results;
        }

        public bool IsBorrowable(LibraryItem libraryItem)
        {
            bool IsValid = false;

            if (libraryItem.Type.Equals(Book) || libraryItem.Type.Equals(AudioBook) || libraryItem.Type.Equals(DVD))
            {
                IsValid = (bool)(libraryItem.IsBorrowable = true);
            }
            else if (libraryItem.Type.Equals(ReferenceBook))
            {
                IsValid = (bool)(libraryItem.IsBorrowable = false);
            }
            return IsValid;
        }
    }
}