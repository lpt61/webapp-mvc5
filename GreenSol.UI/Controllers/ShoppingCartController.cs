using GreenSol.Domain.Abstract;
using GreenSol.Domain.Entities;
using GreenSol.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace GreenSol.UI.Controllers
{
    public class ShoppingCartController : Controller
    {
        private IAlbumRepository repository;
        private IOrderProcessor orderProcessor;       

        private ShoppingCart Cart
        {
            get
            {
                return ShoppingCart.GetCart(repository, this.HttpContext);
            }
        }

        public ShoppingCartController(IAlbumRepository repo, IOrderProcessor proc)
        {
            this.repository = repo;
            orderProcessor = proc;
        }

        public ViewResult Index(string returnUrl)
        {          
            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                Lines = this.Cart.CartLines(),
                TotalPrice = this.Cart.TotalPrice(),
                ReturnUrl = returnUrl
            };

            // Return the view
            return View(viewModel);
        }

        //public RedirectToRouteResult AddToCart(int albumId , string returnUrl)
        //{
        //    Album album = repository.Albums.FirstOrDefault(p => p.AlbumId == albumId);
        //    if (album != null)
        //    {
        //        this.Cart.AddLine(album, 1);
        //    }
        //    return RedirectToAction("Index", new { returnUrl });
        //}

        /*If the lineId = null, which means adding a new line,
         * If lineId != null, which means inscrease the count of that line.
         * I use this to increase the quantity in ShoppingCart/Index
        */
        //public ActionResult AddToCartJSON(int albumId, int quantity = 1)
        //{
        //    Album album = repository.Albums.FirstOrDefault(p => p.AlbumId == albumId);
        //    var result = "";
        //    if (album != null)
        //    {
        //        this.Cart.AddLine(album, quantity);

        //        Cart line = repository.Carts.SingleOrDefault(
        //            c => c.CartId == this.Cart.GetCartId(this.HttpContext)
        //            && c.AlbumId == album.AlbumId);
        //        result = JsonConvert.SerializeObject(
        //            new
        //            {
        //                Quantity = line.Count,
        //                Subtotal = line.Count * line.Album.Price,
        //                TotalItem = Cart.TotalItems(),
        //                TotalPrice = Cart.TotalPrice()
        //            },
        //            Formatting.Indented,
        //            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        //    }
        //    return Content(result, "application/json");
        //}

        public ActionResult AddToCartJSON(int albumId, int quantity = 1)
        {
            Album album = repository.Albums.FirstOrDefault(p => p.AlbumId == albumId);
            var result = "";
            if (album != null)
            {
                this.Cart.AddLine(albumId, quantity);

                Cart line = repository.Carts.SingleOrDefault(
                    c => c.CartId == this.Cart.GetCartId(this.HttpContext)
                    && c.AlbumId == albumId);
                result = JsonConvert.SerializeObject(
                    new
                    {
                        Quantity = line.Count,
                        Subtotal = line.Count * line.Album.Price,                       
                        TotalItem = Cart.TotalItems(),
                        TotalPrice = Cart.TotalPrice()
                    },
                    Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            return Content(result, "application/json");
        }


        /*Remove the entire line*/
        public RedirectToRouteResult RemoveCart(int lineId, string returnUrl)
        {
            Cart line = repository.Carts.FirstOrDefault(c => c.LineId == lineId);
            this.Cart.RemoveFromLine(line.LineId, null);
            return RedirectToAction("Index", new { returnUrl });
        }

        /*Remove one item at a time, I use this to decrease the quantity in ShoppingCart/Index*/
        public ActionResult RemoveFromCart(int lineId, int quantity = 1)
        {
            Cart line = repository.Carts.FirstOrDefault(c => c.LineId == lineId);
            this.Cart.RemoveFromLine(line.LineId, quantity);
            var result = JsonConvert.SerializeObject(
                new { Quantity = line.Count,
                      Subtotal = line.Count * line.Album.Price,
                      TotalItem = Cart.TotalItems(), 
                      TotalPrice = Cart.TotalPrice() 
                },
                Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Content(result, "application/json");
        }

        public PartialViewResult Summary()
        {
            var cartLines = this.Cart.CartLines()
                .Select(a => a.Album.Name)
                .OrderBy(x => x);

            ViewBag.CartCount = cartLines.Count();
            ViewBag.CartSummary = string.Join("\n", cartLines.Distinct());

            return PartialView(this.Cart);
        }

        public ViewResult Checkout()
        {
            return View(new OrderDetail());
        }

        [HttpPost]
        public ViewResult Checkout(OrderDetail orderDetail, string cusId)
        {

            if (this.Cart.CartLines().Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }

            if (ModelState.IsValid)
            {
                this.Cart.CreateOrder(orderDetail, cusId);
                orderProcessor.ProcessOrder(this.Cart, orderDetail);
                this.Cart.EmptyCart();
                return View("Completed");
            }
            else
            {
                return View(orderDetail);
            }
        }
    }
}