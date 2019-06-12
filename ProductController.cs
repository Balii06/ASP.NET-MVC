using ImageFile_Upload.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO; 

namespace ImageFile_Upload.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ProductAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProductAdd(Product product, IEnumerable<HttpPostedFileBase> file, int?id)
        {
            if (ModelState.IsValid)
            {
                var supportedPicTypes = new[] { "jpg", "jpeg", "png" };
                var supportedPdfTypes = new[] { "pdf", "doc", "docx"};
                var PicFileSize = 200000;
                var PdfFileSize = 1000000;
                

                if (file.ElementAt(0) != null && file.ElementAt(1) != null)
                {
                    if (file.ElementAt(0).ContentLength > (PicFileSize))
                    {
                        ViewBag.Message = "Image Size should be less than 2mb";
                    }
                    else if(!supportedPicTypes.Contains(System.IO.Path.GetExtension(file.ElementAt(0).FileName).Substring(1)))
                    {
                        ViewBag.Message = "Image Extension is not valid";
                    }
                    else if(file.ElementAt(1).ContentLength > (PdfFileSize))
                    {
                        ViewBag.Message = "File Size should be less than 10mb";
                    }
                    else if (!supportedPdfTypes.Contains(System.IO.Path.GetExtension(file.ElementAt(1).FileName).Substring(1)))
                    {
                        ViewBag.Message = "File Extension is not valid";
                    }
                    else
                    {
                         using (MyDbContext db = new MyDbContext())
                         {
                             

                             var produc = db.products.Where(x => x.ProductId == id).FirstOrDefault();
                             if (produc != null)
                             {
                                 ViewBag.Message = "Record Already Exist!";
                                 string Upath = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.ElementAt(0).FileName));
                                 file.ElementAt(0).SaveAs(Upath);

                                 string Upth = Path.Combine(Server.MapPath("~/PdfFiles"), Path.GetFileName(file.ElementAt(1).FileName));
                                 file.ElementAt(1).SaveAs(Upth);

                                 Product p = new Product
                                 {
                                     ProductId = (int)id,
                                     ProductName = product.ProductName,
                                     Image = "~/Images/" + file.ElementAt(0).FileName,
                                     Pdf = "~/PdfFiles/" + file.ElementAt(1).FileName
                                 };
                                 db.Entry(produc).CurrentValues.SetValues(p);
                                 db.SaveChanges();
                                 ViewBag.Message = "Record Already Exist!";
                                
                             }
                             else
                             {
                                 string path = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.ElementAt(0).FileName));
                                 file.ElementAt(0).SaveAs(path);

                                 string pth = Path.Combine(Server.MapPath("~/PdfFiles"), Path.GetFileName(file.ElementAt(1).FileName));
                                 file.ElementAt(1).SaveAs(pth);

                                 db.products.Add(new Product
                                 {
                                     ProductId = product.ProductId,
                                     ProductName = product.ProductName,
                                     Image = "~/Images/" + file.ElementAt(0).FileName,
                                     Pdf = "~/PdfFiles/" + file.ElementAt(1).FileName
                                 });
                                 db.SaveChanges();
                                 ViewBag.Message = "Uploaded Successfully";
                             }
                         }
                    }
                }
                else
                   {
                        ViewBag.Message = "File Not Found";
                   }
            }
            return View();
        }
