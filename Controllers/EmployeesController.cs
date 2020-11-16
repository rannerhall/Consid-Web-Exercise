using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ConsidTest;
using ConsidTest.Utils;
using ConsidTest.Validators;
using FluentValidation.Results;

namespace ConsidTest.Controllers
{
    public class EmployeesController : Controller
    {
        private const string EMPLOYEE = "Employee";
        private const string MANAGER = "Manager";
        private const string CEO = "CEO";

        private readonly ConsidDBEntities db = new ConsidDBEntities();
        private readonly CalculateSalary CalculateSalary = new CalculateSalary();
        private readonly EmployeeValidator EmployeeValidator = new EmployeeValidator();

        public ActionResult Index()
        {
            var Employees = db.Employees;

            return View(Employees.ToList().OrderBy(s => s.Role));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        private List<SelectListItem> GetViewBagForRoles(string Role)
        {
            var RoleType = new List<SelectListItem>
            {
                new SelectListItem() { Text = Role, Value = Role },
                new SelectListItem() { Text = EMPLOYEE, Value = EMPLOYEE },
                new SelectListItem() { Text = MANAGER, Value = MANAGER },
                new SelectListItem() { Text = CEO, Value = CEO },
            };
            return RoleType;
        }

        public ActionResult Create()
        {
            ViewBag.Role = GetViewBagForRoles("");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Role,ManagerId")] string Role)
        {
            if (Role == string.Empty)
            {
                ViewBag.Role = GetViewBagForRoles("");
                ModelState.AddModelError(string.Empty, "Please choose a role");
                return View();
            }

            Employee employee = new Employee
            {
                Role = Role,
            };

            if (employee.Role.Equals(CEO))
            {
                var result = db.Employees.FirstOrDefault(x => x.IsCEO == true);

                if (result != null && (bool)(result.IsCEO = true))
                {
                    ModelState.AddModelError(string.Empty, "There can be only one!");
                    ViewBag.Role = GetViewBagForRoles(employee.Role);
                    return View();
                }
            }

            SetRoleParameters(employee);

            db.Employees.Add(employee);
            db.SaveChanges();

            return RedirectToAction("EmployeeCreate", employee);
        }

        private void SetRoleParameters(Employee employee)
        {
            if (employee.Role.Equals(EMPLOYEE))
            {
                employee.IsCEO = false;
                employee.IsManager = false;
            }
            else if (employee.Role.Equals(MANAGER))
            {
                employee.IsCEO = false;
                employee.IsManager = true;
            }
            else if (employee.Role.Equals(CEO))
            {
                employee.IsCEO = true;
                employee.IsManager = false;
            }
        }

        public ActionResult EmployeeCreate(Employee employee)
        {
            ViewBag.Manager = GetManagerViewBag();
            return View(employee);
        }

        private SelectList GetManagerViewBag()
        {
            List<int?> EmployeeId = new List<int?>();
            var Employees = db.Employees.ToList();
            foreach (var Employee in Employees)
            {
                if (Employee.IsManager == true && Employee.ManagerId.GetValueOrDefault(0) != 0)
                {
                    EmployeeId.Add(Employee.Id);
                }
            }
            return new SelectList(EmployeeId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeCreate([Bind(Include = "Id,FirstName,LastName,Salary,IsCEO,IsManager,ManagerId,Role")] int? Rank, Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (EmployeeValidator.Validate(employee).IsValid == false)
                {
                    foreach (ValidationFailure employeeError in EmployeeValidator.Validate(employee).Errors)
                    {
                        ModelState.AddModelError(string.Empty, $"ErrorMessage: {employeeError.ErrorMessage}");
                    }
                    ViewBag.Manager = GetManagerViewBag();
                    return View();
                }

                if (Rank.GetValueOrDefault(0) == 0)
                {
                    ModelState.AddModelError(string.Empty, "Please choose rank");
                }

                if (employee.Role.Equals(MANAGER))
                {
                    if (employee.ManagerId.GetValueOrDefault(0) == 0)
                    {
                        employee.ManagerId = employee.Id;
                    }
                }

                decimal CalculatedSalary = CalculateSalary.Calculate(employee, (int)Rank);
                employee.Salary = CalculatedSalary;

                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Manager = GetManagerViewBag();
            return View(employee);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Salary,IsCEO,IsManager,ManagerId,ReportsTo,Role")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
