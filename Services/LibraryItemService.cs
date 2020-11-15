using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ConsidTest.Validators;
using FluentValidation.Results;

namespace ConsidTest.Services
{
    public class LibraryItemService
    {
        private ConsidDBEntities db = new ConsidDBEntities();

        public IQueryable<LibraryItem> GetLibraryItems()
        {
            var libraryItems = db.LibraryItems.Include(l => l.Category);

            libraryItems = libraryItems.OrderBy(s => s.Category.CategoryName);

            GetTitleAndTitleAcronym(libraryItems);

            return libraryItems;
        }

        private IQueryable<LibraryItem> GetTitleAndTitleAcronym(IQueryable<LibraryItem> libraryItems)
        {
            string Convert(IEnumerable<char> s) => new string(s.ToArray());

            foreach (LibraryItem Titles in libraryItems)
            {
                string AcronymedTitel = Convert(Titles.Title.Where((c, i) => !char.IsWhiteSpace(c) && (i == 0 || char.IsWhiteSpace(Titles.Title[i - 1]))));
                Titles.SetAcronymedTitle(Titles.Title, AcronymedTitel.ToUpper());
            }

            return libraryItems;
        }

        public LibraryItem SaveLibraryItem(LibraryItem libraryItem)
        {
            db.LibraryItems.Add(libraryItem);
            db.SaveChanges();

            return libraryItem;
        }
    }
}