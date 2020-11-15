using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ConsidTest.Services;
using ConsidTest.Validators;
using FluentValidation.Results;

namespace ConsidTest.Controllers
{
    public class LibraryItemsController : Controller
    {
        private const string CreateView = "Create";

        private const string Book = "Book";
        private const string DVD = "DVD";
        private const string AudioBook = "Audio Book";
        private const string ReferenceBook = "Reference Book";

        private readonly ConsidDBEntities db = new ConsidDBEntities();
        private readonly LibraryItemValidator LibraryItemValidator = new LibraryItemValidator();
        private readonly LibraryItemService LibraryItemService = new LibraryItemService();

        public ActionResult Index(string sortOrder)
        {
            var libraryItems = LibraryItemService.GetLibraryItems();

            ViewBag.TypeSortParm = String.IsNullOrEmpty(sortOrder) ? "type_desc" : "";

            switch (sortOrder)
            {
                case "type_desc":
                    libraryItems = libraryItems.OrderBy(s => s.Type);
                    break;
            }

            return View(libraryItems.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LibraryItem libraryItem = db.LibraryItems.Find(id);
            if (libraryItem == null)
            {
                return HttpNotFound();
            }
            return View(libraryItem);
        }

        public ActionResult Create()
        {
            ViewBag.Type = GetViewBagForType("");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName");
            return View();
        }

        private List<SelectListItem> GetViewBagForType(string Type)
        {
            var Types = new List<SelectListItem>
            {
                new SelectListItem() { Text = Book, Value = Book },
                new SelectListItem() { Text = DVD, Value = DVD },
                new SelectListItem() { Text = AudioBook, Value = AudioBook },
                new SelectListItem() { Text = ReferenceBook, Value = ReferenceBook },
            };
            return Types;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,BorrowDate,Type")] LibraryItem libraryItem)
        {
            if (ModelState.IsValid)
            {
                if (libraryItem.Type == null)
                {
                    ModelState.AddModelError(string.Empty, "Type can not be empty");
                    Create();
                    return View(libraryItem);
                }

                if (LibraryItemValidator.ValidateLibraryItem(libraryItem).IsValid == false)
                {
                    GetErrorMessages(libraryItem);

                    ViewBag.Type = GetViewBagForType(libraryItem.Type);
                    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", libraryItem.CategoryId);
                    return View(libraryItem);
                }

                LibraryItemValidator.IsBorrowable(libraryItem);

                LibraryItemService.SaveLibraryItem(libraryItem);

                return RedirectToAction("Index");
            }

            ViewBag.Type = GetViewBagForType(libraryItem.Type);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", libraryItem.CategoryId);
            return View(libraryItem);
        }

        private void GetErrorMessages(LibraryItem libraryItem)
        {
            foreach (ValidationFailure item in LibraryItemValidator.ValidateLibraryItem(libraryItem).Errors)
            {
                ModelState.AddModelError(string.Empty, $"ErrorMessage: {item.ErrorMessage}");
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LibraryItem libraryItem = db.LibraryItems.Find(id);
            if (libraryItem == null)
            {
                return HttpNotFound();
            }

            if (LibraryItemValidator.IsBorrowable(libraryItem) == true)
            {
                ViewBag.Borrower = true;
            }

            ViewBag.Type = GetViewBagForType(libraryItem.Type);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", libraryItem.CategoryId);
            return View(libraryItem);
        }

        public ActionResult CheckIn(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LibraryItem libraryItem = db.LibraryItems.Find(id);
            if (libraryItem == null)
            {
                return HttpNotFound();
            }

            ViewBag.Type = GetViewBagForType(libraryItem.Type);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", libraryItem.CategoryId);
            return View(libraryItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckIn([Bind(Include = "Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,BorrowDate,Type")] LibraryItem libraryItem)
        {
            if (ModelState.IsValid)
            {
                libraryItem.Borrower = "";
                libraryItem.BorrowDate = null;

                db.Entry(libraryItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(libraryItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,BorrowDate,Type")] LibraryItem libraryItem)
        {
            if (ModelState.IsValid)
            {
                if (LibraryItemValidator.ValidateLibraryItem(libraryItem).IsValid == false)
                {
                    GetErrorMessages(libraryItem);

                    ViewBag.Type = GetViewBagForType(libraryItem.Type);
                    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", libraryItem.CategoryId);
                    return View(libraryItem);
                }

                ViewBag.Borrower = false;

                if (libraryItem.Borrower != null)
                {
                    libraryItem.BorrowDate = DateTime.Now;
                }

                db.Entry(libraryItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", libraryItem.CategoryId);
            return View(libraryItem);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LibraryItem libraryItem = db.LibraryItems.Find(id);
            if (libraryItem == null)
            {
                return HttpNotFound();
            }
            return View(libraryItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LibraryItem libraryItem = db.LibraryItems.Find(id);
            db.LibraryItems.Remove(libraryItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
