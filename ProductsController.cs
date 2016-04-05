using EntityLayer;
using MvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class ProductsController : Controller
    {
        //
        // GET: /Products/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddProduct(ProductsModel model, string id)
        {
            List<Error> error = new List<Error>();
            if (ModelState.IsValid)
            {
                try
                {
                    using (var ctx = new DataAccessLayer.AppEntities())
                    {

                        if (id == null)
                        {

                            var productsEntity = new DataAccessLayer.Product();
                            productsEntity.IsActive = true;
                            productsEntity.IsDeleted = false;
                            productsEntity.ProductName = model.ProductName;
                            productsEntity.ProductQuantity = model.ProductQuantity;
                            productsEntity.ProductMRP = model.ProductMRP;
                            productsEntity.TradePrice = model.TradePrice;
                            ctx.Products.Add(productsEntity);
                            ctx.SaveChanges();
                            error.Add(new Error { Key = "Success", Message = "Saved Successfully" });

                        }
                        else
                        {
                            int productId = Convert.ToInt32(id);
                            var productData = (from A in ctx.Products where A.ProductId == productId select A).FirstOrDefault();
                            productData.ProductName = model.ProductName;
                            productData.ProductMRP = Convert.ToDouble(model.ProductMRP);
                            productData.ProductQuantity = model.ProductQuantity;
                            productData.TradePrice = model.TradePrice;
                            productData.IsActive = true;
                            productData.IsDeleted = false;
                            ctx.SaveChanges();
                            error.Add(new Error { Key = "Success", Message = "Updated Successfully" });
                           
                        }
                        return Json(error, JsonRequestBehavior.AllowGet);
                    }

                }

                catch (Exception ex)
                {
                    error.Add(new Error { Key = "Failure", Message = "Error occurred while saving." });
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(ModelStateExtensions.AllErrors(ModelState), JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        //[Authorize(Roles = "admin")]
        public JsonResult BindProductsTable()
        {
            List<DataAccessLayer.Product> ProductData = null;

            StringBuilder sb = new StringBuilder();
            using (var ctx = new DataAccessLayer.AppEntities())
            {
                ProductData = (from A in ctx.Products where A.IsDeleted == false && A.IsActive == true select A).ToList();
            }
            /////////////
            /// JSON output
            /// - build JSON output from DB results
            /// ///////////
            sb.Clear();
            string outputJson = string.Empty;
            int totalDisplayRecords = 0;
            int totalRecords = 0;
            foreach (var item in ProductData)
            {
                if (totalRecords == 0)
                    totalRecords = ProductData.Count();
                if (totalDisplayRecords == 0)
                    totalDisplayRecords = ProductData.Count();
                sb.Append("[");
                sb.Append("\"" + item.ProductId + "\",");
                sb.Append("\"" + item.ProductName + "\",");
                sb.Append("\"" + item.ProductQuantity + "\",");
                sb.Append("\"" + item.ProductMRP + "\",");
                sb.Append("\"" + item.TradePrice + "\",");
                sb.Append("\"" + "Active" + "\"");
                sb.Append("],");
            }
            outputJson = sb.ToString();
            if (!string.IsNullOrEmpty(outputJson))
            {
                outputJson = outputJson.Remove(outputJson.Length - 1);
            }
            sb.Clear();

            sb.Append("{");
            sb.Append("\"sEcho\": ");
            sb.Append("0");
            sb.Append(",");
            sb.Append("\"iTotalRecords\": ");
            sb.Append(totalRecords);
            sb.Append(",");
            sb.Append("\"iTotalDisplayRecords\": ");
            sb.Append(totalDisplayRecords);
            sb.Append(",");
            sb.Append("\"aaData\": [");
            sb.Append(outputJson);
            sb.Append("]}");
            outputJson = sb.ToString();

            /////////////
            /// Write to Response
            /// - clear other HTML elements
            /// - flush out JSON output
            /// ///////////
            return Json(outputJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteProduct(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var ctx = new DataAccessLayer.AppEntities())
                {
                    int productId = Convert.ToInt32(id);
                    var presentationData = (from A in ctx.Products where A.ProductId == productId select A).FirstOrDefault();
                    presentationData.IsActive = false;
                    presentationData.IsDeleted = true;
                    ctx.SaveChanges();
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}
