using GreenSol.Domain.Abstract;
using GreenSol.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;
using GreenSol.Domain.Entities;

namespace GreenSol.UI.Controllers
{
    public class OrderController : Controller
    {
        private IAlbumRepository repository;

        public OrderController(IAlbumRepository repo)
        {
            this.repository = repo;
        } 

        // GET: Order
        public ActionResult OrderHistory(string id)
        {
            //var model = new OrderHistoryModel()
            //{
            //    Orders = repository.Orders.Where(o => o.CusId == cusId).OrderBy(o => o.OrderDetails.OrderDate)
            //};
            var model = new List<OrderHistoryModel>() { }; 
            var orderDetail = repository.OrderDetails.Where(od => od.CustomerId == id).OrderBy(od => od.OrderDate);
            foreach (var od in orderDetail)
            {
                //var orders = repository.Orders.Where(o => o.OrderDetailId == od.OrderDetailId);
                model.Add(new OrderHistoryModel()
                {
                    OrderDetailsId = od.OrderDetailId,
                    OrderDate = od.OrderDate,
                    Total = od.Total,
                    Status = od.Status,
                    //Orders = orders
                });
            }
          
            return View(model);
        }

        public ActionResult OrderDetail(int id)
        {
            var orders = repository.Orders.Where(o => o.OrderDetailId == id);
            return View(orders);           
        }

        public JsonResult OrderDetailJSON(int id)
        {
            var orders = repository.Orders.Where(o => o.OrderDetailId == id);

            var model = new List<object>();

            foreach (var item in orders)
            {
                var album = repository.Albums.Where(a => a.AlbumId == item.AlbumId).Select(a => a.Name);
                model.Add(new { Album = album, Quantity = item.Quantity });
            }

            var result = JsonConvert.SerializeObject(
                model, 
                Formatting.Indented, 
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(result, "application/json");
        }

        public JsonResult CancelOrder(int id)
        {
            string status = repository.ProcessOrder(id, false);

            return Json(status, JsonRequestBehavior.AllowGet);
        }
    }
}