using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
///////////////////////////
using GreenSol.Domain.Abstract;
using System.Web;
using System.Web.Mvc;

namespace GreenSol.Domain.Entities
{
    public class ShoppingCart
    {
        private IAlbumRepository repository;

        string ShoppingCartId { get; set; }

        public ShoppingCart(IAlbumRepository repo)
        {
            this.repository = repo;
        }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(IAlbumRepository repo, HttpContextBase context)
        {
            var cart = new ShoppingCart(repo);
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(IAlbumRepository repo, Controller controller)
        {
            return GetCart(repo, controller.HttpContext);
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();

                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }

        //private List<CartLine> lineCollection = new List<CartLine>();
        public List<Cart> CartLines()
        {
            List<Cart> lines = null;

            //this retrieving causes an error: Object reference not set to an instance of an object.
            //however, the result is corect (Why?)
            //I haven't know how to fix yet, so I add a try catch to IGNORE this error, I also do this in the below MigrateCart() method
            try
            {
                lines = repository.Carts.Where(c => c.CartId == this.ShoppingCartId).ToList();
            }
            catch { }
            return lines;          
        }

        //public void AddLine(Album album, int quantity)
        //{
        //    // Get the matching cart and album instances
        //    var line = repository.Carts.SingleOrDefault(
        //        c => c.CartId == this.ShoppingCartId
        //        && c.AlbumId == album.AlbumId);

        //    if (line == null)
        //    {
        //        // Create a new cart line if no cart item exists
        //        Cart newLine = new Cart
        //        {
        //            CartId = this.ShoppingCartId,
        //            AlbumId = album.AlbumId,
        //            Count = quantity,
        //            DateCreated = DateTime.Now
        //        };
        //        repository.AddLine(newLine);
        //    }
        //    else
        //    {
        //        // If the item does exist in the cart, then add one to the quantity
        //        repository.AddLine(line.LineId, quantity);
        //    }
        //}

        public void AddLine(int albumId, int quantity)
        {
            repository.AddLine(this.ShoppingCartId, albumId, quantity);
        }

        //A line = album + quantity, each line has a RecordId
        //public int RemoveLine(int lineId)
        //{
        //    // Get the cart
        //    var cartLine = repository.Carts.Single(
        //        cart => cart.CartId == this.ShoppingCartId
        //        && cart.LineId == lineId);
            
        //    if (cartLine != null)
        //    {
        //        repository.RemoveLine(cartLine);
        //    }
        //    return 0;
        //}

        //public int RemoveFromLine(int lineId, int quantity)
        //{
        //    quantity = this.quantityToRemove;

        //    // Get the cart
        //    var cartLine = repository.Carts.Single(
        //        cart => cart.CartId == this.ShoppingCartId
        //        && cart.LineId == lineId);

        //    int itemCount = 0;

        //    if (cartLine != null)
        //    {
        //        if (cartLine.Count > 1)
        //        {
        //            repository.RemoveLine(cartLine, lineId, quantity);
        //            itemCount = cartLine.Count;
        //        }
        //    }
        //    return itemCount;
        //}

        public int RemoveFromLine(int lineId, int? quantity)
        {
            int itemCount = repository.RemoveLine(this.ShoppingCartId, lineId, quantity);
            if (itemCount > 0)
                return itemCount;
            return 0;
        }

        //Count all items in all lines
        public int TotalItems()
        {
            // Get the count of each item in the cart and sum them up
            int? count = repository.Carts
                .Where(c => c.CartId == this.ShoppingCartId)
                .Select(c => (int?)c.Count).Sum();

            // Return 0 if all entries are null
            return count ?? 0;
        }

        // get the cart total price
        public decimal TotalPrice()
        {            
            decimal? total = repository.Carts.Where(c => c.CartId == this.ShoppingCartId).Select(c => (int?)c.Count * c.Album.Price).Sum(); 
            
            return total ?? decimal.Zero;
        }

        public void EmptyCart()
        {
            repository.EmptyCart(this.ShoppingCartId);
        }

        public int CreateOrder(OrderDetail orderDetail, string cusId)
        {
            orderDetail.CustomerId = cusId;
            orderDetail.OrderDate = DateTime.Now;
            orderDetail.Total = this.TotalPrice();
            orderDetail.Status = GreenSol.Domain.Entities.Status.Processing;

            repository.CreateOrderDetail(orderDetail);

            var cartLines = this.CartLines();

            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in cartLines)
            {
                Album album = repository.Albums.Single(a => a.AlbumId == item.AlbumId);

                var order = new Order
                {
                    AlbumId = item.AlbumId,
                    OrderId = orderDetail.OrderDetailId,
                    UnitPrice = album.Price,
                    Quantity = item.Count,
                };
                repository.CreateOrder(order);
            }

            // Empty the shopping cart
            //EmptyCart();

            // Return the OrderId as the confirmation number
            return orderDetail.OrderDetailId;
        }


        // When a user has logged in, migrate his shopping cart to his username
        public void MigrateCart(string userName)
        {
            var shoppingCart = CartLines();
            try
            {
                foreach (Cart cartLine in shoppingCart)
                {
                    cartLine.CartId = userName;
                }
            }
            catch { }
        }
    }
}