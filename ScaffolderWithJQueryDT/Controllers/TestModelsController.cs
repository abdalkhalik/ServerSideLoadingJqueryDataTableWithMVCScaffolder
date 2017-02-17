
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

using System.Net;
using System.Web;
using System.Web.Mvc;
using ScaffolderWithJqueryDT.Helper;
using ScaffolderWithJQueryDT.Models;


namespace ScaffolderWithJQueryDT.Controllers
{

    public class TestModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

		// Title: 
        // GET: TestModels

       
        public ActionResult Index()
        {
            return View(db.testTable.ToList());
        }

        // GET: TestModels/Details/5
      
       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestModel testModel = db.testTable.Find(id);
            if (testModel == null)
            {
                return HttpNotFound();
            }
            return View(testModel);
        }

        // GET: TestModels/Create

       
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public ActionResult Create([Bind(Include = "ID,firstName,lastName,Hobby,Degree")] TestModel testModel)
        {
            if (ModelState.IsValid)
            {
                db.testTable.Add(testModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testModel);
        }

        // GET: TestModels/Edit/5
      
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestModel testModel = db.testTable.Find(id);
            if (testModel == null)
            {
                return HttpNotFound();
            }
            return View(testModel);
        }

        // POST: TestModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,firstName,lastName,Hobby,Degree")] TestModel testModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testModel);
        }

        // GET: TestModels/Delete/5
       
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestModel testModel = db.testTable.Find(id);
            if (testModel == null)
            {
                return HttpNotFound();
            }
            return View(testModel);
        }

        // POST: TestModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		
        public ActionResult DeleteConfirmed(int id)
        {
            TestModel testModel = db.testTable.Find(id);
            db.testTable.Remove(testModel);
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


		public ActionResult AjaxGetJsonData(int draw, int start, int length)
        {
            DataTableModels<TestModel>._data = db.testTable.ToList();
            string search = Request.QueryString["search[value]"];
            int sortColumn = -1;
            string sortDirection = "asc";
            if (length == -1)
            {
                length = DataTableModels<TestModel>._data.Count;
            }

            // note: we only sort one column at a time
            if (Request.QueryString["order[0][column]"] != null)
            {
                sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            }
            if (Request.QueryString["order[0][dir]"] != null)
            {
                sortDirection = Request.QueryString["order[0][dir]"];
            }

            DataTableData<TestModel> dataTableData = new DataTableData<TestModel>();
            dataTableData.draw = draw;
            dataTableData.recordsTotal =  DataTableModels<TestModel>._data.Count;
            int recordsFiltered = 0;
            dataTableData.data = DataTableModels<TestModel>.FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
            dataTableData.recordsFiltered = recordsFiltered;

            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }

    }
}
