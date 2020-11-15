using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ConsidTest;

namespace ConsidTest.Controllers
{
    public class CategoriesController : Controller
    {
        private ConsidDBEntities db = new ConsidDBEntities();

        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (ValidateNoDuplicatesAllowed(category) == false)
                {
                    ModelState.AddModelError(string.Empty, "No duplicates allowed in Category name, Please choose another name");
                    return View(category);
                }
                db.Categories.Add(category);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        private bool ValidateNoDuplicatesAllowed(Category category)
        {
            bool IsValid = true;
            var CategoryListFromDataBase = db.Categories.ToList();

            foreach (var Categories in CategoryListFromDataBase)
            {
                if (Categories.CategoryName.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase))
                {
                    return IsValid = false;
                }
            }
            return IsValid;
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //Sql fel när man editerar, har med entity att göra.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (ValidateNoDuplicatesAllowed(category) == false)
                {
                    ModelState.AddModelError(string.Empty, "No duplicates allowed in Category name, Please choose another name");
                    return View(category);
                }

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            ModelState.AddModelError(string.Empty, "This category is registered to an item and can not be removed until the item is removed");
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
