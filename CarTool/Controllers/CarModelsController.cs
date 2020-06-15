using System;
using System.Drawing;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarTool.Models;
using CarTool.ViewModels;
using System.IO;
using System.Threading.Tasks;
using System.Linq.Expressions;
using CarTool.Utlities;
using System.Web.Helpers;

namespace CarTool.Controllers
{
    [Authorize]
    public class CarModelsController : Controller
    {
        private CarToolEntities1 db = new CarToolEntities1();

        public ActionResult Index()
        {
            return View();
        }
        // GET: Models
        public ActionResult Models(int page = 1, string search = "")
        {
            int pageSize = 10;
            int totalRecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pageSize) - pageSize;
            var models = GetModels(search, pageSize, out totalRecord);
            ViewBag.TotalRows = totalRecord;
            ViewBag.Search = search;
            
            return View("Index", GetCarModelViewModels(models));
        }

        public ActionResult ModelsByLine(int? lineID)
        {
            if (lineID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int totalRecord = 0;
            var models = GetModels("", 10, out totalRecord).Where(m => m.LineID == lineID);
            ViewBag.TotalRows = totalRecord;

            return View("Index", GetCarModelViewModels(models));
        }

        public List<Model> GetModels(string search, int pageSize, out int totalRecord)
        {
            var models = (from m in db.Models
                          where
                              m.Description.Contains(search) ||
                              m.Name.Contains(search)
                          select m
                            );
            totalRecord = models.Count();

            return models.ToList();
        }

        // GET: Models/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // GET: Models/Create
        public ActionResult Create()
        {
            ViewBag.LineID = new SelectList(db.Lines, "LineID", "Name");
            return View();
        }

        // POST: Models/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ModelID,LineID,Name,Description,Price")] Model model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if(upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(upload.InputStream))
                    {
                        model.Picture = reader.ReadBytes(upload.ContentLength);
                    }
                }
                if(model.Picture == null)
                {
                    // default image
                    string path = Server.MapPath("~/Content/no_image_selected.png");
                    model.Picture = System.IO.File.ReadAllBytes(path);
                }
                db.Models.Add(model);
                db.SaveChanges();
                return RedirectToAction("Models");
            }

            ViewBag.LineID = new SelectList(db.Lines, "LineID", "Name", model.LineID);
            return View(model);
        }

        // GET: Models/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModelID,LineID,Name,Description,Price,Picture")] Model model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        using (var reader = new BinaryReader(upload.InputStream))
                        {
                            model.Picture = reader.ReadBytes(upload.ContentLength);
                        }
                    }

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Models");
                }
                catch(Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }

            return View(model);
        }

        public static void UpdateModelFromViewModel(Model model, CarModelViewModel vm)
        {
            model.Description = vm.ModelDescription;
            model.Name = vm.ModelName;
            model.Price = vm.Price;
            if(!String.IsNullOrEmpty(vm.ImagePath)) // new image selected
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Image img = Image.FromFile(vm.ImagePath);
                    byte[] newImageBytes;
                    img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    newImageBytes = stream.ToArray();
                    if(newImageBytes.Length > 0)
                    {
                        model.Picture = newImageBytes;
                    }
                }
            }
        }

        // GET: Models/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Models/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Model model = db.Models.Find(id);
            db.Models.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Models");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<CarModelViewModel> GetCarModelViewModels(IEnumerable<Model> carModels)
        {
            var result = new List<CarModelViewModel>();
            var lines = db.Lines.ToList();

            var query = from m in carModels
                        join l in lines
                            on m.LineID equals l.LineID
                        select new
                        {
                            LineName = l.Name,
                            l.LineID,
                            m.ModelID,
                            ModelName = m.Name,
                            ModelDescription = m.Description,
                            m.Picture,
                            m.Price
                        };

            foreach (var m in query)
            {
                result.Add(new CarModelViewModel
                {
                    ModelID = m.ModelID,
                    LineID = m.LineID,
                    LineName = m.LineName,
                    ModelName = m.ModelName,
                    ModelDescription = m.ModelDescription,
                    Picture = m.Picture,
                    Price = m.Price
                }); ;
            }
            return result;
        }

        public static String ConvertByteArrayToBase64(byte[] picture)
        {
            return Convert.ToBase64String(picture);
        }
    }
}
