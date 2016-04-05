using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using MvcApp.Models;
using EntityLayer;
using System.Text;

namespace MvcApp.Controllers
{
    public class StocksController : Controller
    {
        //
        // GET: /Stock/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult BindProductName()
        {
            List<SelectListItem> objProductNamesList = Common.GetProductNames();
            return Json(objProductNamesList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProductQuantity(string id)
        {
            string productQuantity = Common.GetProductQuantity(id);
            return Json(productQuantity, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddStock(StockModel model, string productid, string stockid)
        {
            List<Error> error = new List<Error>();
            if (ModelState.IsValid)
            {
                try
                {
                    using (var ctx = new DataAccessLayer.AppEntities())
                    {

                        if (stockid == null)
                        {
                            var StockEntity = new DataAccessLayer.Stock();
                            StockEntity.IsActive = true;
                            StockEntity.IsDeleted = false;
                            StockEntity.TotalStock = Convert.ToInt32(model.TotalStock);
                            StockEntity.ProductId = Convert.ToInt32(productid);
                            StockEntity.DateModified = DateTime.Now;                          
                            ctx.Stocks.Add(StockEntity);
                            ctx.SaveChanges();
                            error.Add(new Error { Key = "Success", Message = "Saved Successfully" });

                        }
                        else
                        {
                            int stockId = Convert.ToInt32(stockid);
                            var stockData = (from A in ctx.Stocks where A.StockId == stockId select A).FirstOrDefault();
                            stockData.IsActive = true;
                            stockData.IsDeleted = false;
                            stockData.TotalStock = Convert.ToInt32(model.TotalStock);
                            stockData.DateModified = DateTime.Now;                       
                            error.Add(new Error { Key = "Success", Message = "Updated Successfully" });
                            stockData.IsActive = true;
                            stockData.IsDeleted = false;
                            ctx.SaveChanges();
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
        public JsonResult BindStockTable()
        {
            dynamic StockData = null;

            StringBuilder sb = new StringBuilder();
            using (var ctx = new DataAccessLayer.AppEntities())
            {
                StockData = (from A in ctx.GetStockDetails select A).ToList();
            }
            /////////////
            /// JSON output
            /// - build JSON output from DB results
            /// ///////////
            sb.Clear();
            string outputJson = string.Empty;
            int totalDisplayRecords = 0;
            int totalRecords = 0;
            foreach (var item in StockData)
            {
                if (totalRecords == 0)
                    totalRecords = StockData.Count;
                if (totalDisplayRecords == 0)
                    totalDisplayRecords = StockData.Count;
                sb.Append("[");
                sb.Append("\"" + item.StockId + "\",");
                sb.Append("\"" + item.ProductId + "\",");
                sb.Append("\"" + item.ProductName + "\",");
                sb.Append("\"" + item.ProductQuantity + "\",");
                sb.Append("\"" + item.TotalStock + "\",");
                sb.Append("\"" + item.DateModified + "\",");
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
        public JsonResult DeleteStock(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var ctx = new DataAccessLayer.AppEntities())
                {
                    int stockId = Convert.ToInt32(id);
                    var Data = (from A in ctx.Stocks where A.StockId == stockId select A).FirstOrDefault();
                    Data.IsActive = false;
                    Data.IsDeleted = true;
                    ctx.SaveChanges();
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}
