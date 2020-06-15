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

namespace CarTool.Controllers
{
    public class CarModelsController : Controller
    {
        private CarToolEntities1 db = new CarToolEntities1();

        // GET: Models
        public ActionResult Index()
        {
           return View(GetCarModelViewModels(db.Models));
        }

        public ActionResult ModelsByLine(int? lineID)
        {
            if (lineID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var models = db.Models
                .Where(m => m.LineID == lineID);

            return View("Index", GetCarModelViewModels(models));
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
            List<Model> carModel = new List<Model>();
            carModel.Add(model);

            return View(GetCarModelViewModels(carModel).FirstOrDefault());
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
        public ActionResult Create([Bind(Include = "ModelID,LineID,Name,Description,Price,ImagePath")] Model model)
        {
            if (ModelState.IsValid)
            {
                db.Models.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            List<Model> carModel = new List<Model>();
            carModel.Add(model);

            return View(GetCarModelViewModels(carModel).FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CarModelViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Model model = db.Models.Find(viewModel.ModelID);
                    if (!String.IsNullOrEmpty(viewModel.ImagePath))
                    {
                        string path = Server.MapPath("~/App_Data/UploadedFiles/");
                        viewModel.ImagePath = path + viewModel.ImagePath;
                    }

                    UpdateModelFromViewModel(model, viewModel);

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    
                }
                catch(Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }

            
            Model updatedModel = db.Models.Find(viewModel.ModelID);

            List<Model> carModel = new List<Model>();
            carModel.Add(updatedModel);

            return View(GetCarModelViewModels(carModel).FirstOrDefault());
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

        [HttpPost]
        public JsonResult UploadImage()
        {
            try
            {
                if(Request.Files != null)
                {
                    var postedFile = Request.Files["uploadedFile"];
                    if (postedFile != null)
                    {
                        string path = Server.MapPath("~/App_Data/UploadedFiles/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = path + postedFile.FileName;
                        postedFile.SaveAs(path);
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
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

            //foreach(var vm in result)
            //{
                
            //    // save image to server and set ImagePathe
            //    using (MemoryStream ms = new MemoryStream(vm.Picture))
            //    {
            //        Image img = Image.FromStream(ms);
            //        string path = Server.MapPath("~/App_Data/UploadedFiles/foo.jpg");
            //        //vm.ImagePath = path + "foo.jpg";
            //        using (FileStream file = new FileStream(path, FileMode.Create, System.IO.FileAccess.Write))
            //        {
            //            byte[] bytes = new byte[ms.Length];
            //            ms.Read(bytes, 0, (int)ms.Length);
            //            file.Write(bytes, 0, bytes.Length);
            //            ms.Close();
            //        }
            //    }
            //}

            return result;
        }

        public static String ConvertByteArrayToBase64(byte[] picture)
        {
            return Convert.ToBase64String(picture);
        }
    }
}
