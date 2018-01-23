using GreenSol.Domain.Abstract;
using GreenSol.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Entity;

namespace GreenSol.Domain.Concrete
{
    public class EFAlbumRepository : IAlbumRepository
    {
        private EFDbContext context = new EFDbContext();

        #region Album, Genre, Artist
        public IEnumerable<Album> Albums
        {
            get { return context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Order)
                .OrderBy(a => a.AlbumId)
                ; }
        }

        public IEnumerable<Artist> Artists
        {
            get { return context.Artists; }
        }

        public IEnumerable<Genre> Genres
        {
            get { return context.Genres; }
        }

        #region Save
        //Explicitly save
        public void SaveAlbum(Album album)
        {
            //Creating a new album
            if (album.AlbumId == 0)
            {
                //check if the genre and artist exist
                Genre existedGenre = context.Genres.FirstOrDefault(g => (g.Name.ToLower() == album.Genre.Name.ToLower()));
                Artist existedArtist = context.Artists.FirstOrDefault(g => (g.Name.ToLower() == album.Artist.Name.ToLower()));

                Album newAlbum = new Album()
                {
                    Name = album.Name,
                    Date = album.Date,
                    Price = album.Price,
                    TrackList = album.TrackList,
                    ImageData = album.ImageData,
                    ImageMimeType = album.ImageMimeType
                };

                if (existedGenre != null)
                    newAlbum.Genre = existedGenre;
                else
                    newAlbum.Genre = new Genre() { Name = album.Genre.Name };

                if (existedArtist != null)
                    newAlbum.Artist = existedArtist;
                else
                    newAlbum.Artist = new Artist() { Name = album.Artist.Name };

                context.Albums.Add(newAlbum);
            }

            //Editing existed album
            else
            {
                Album dbEntry = context.Albums.Find(album.AlbumId);
                if (dbEntry != null)
                {
                    dbEntry.Name = album.Name;
                    dbEntry.Artist.Name = album.Artist.Name;
                    dbEntry.Genre.Name = album.Genre.Name;
                    dbEntry.Date = album.Date;
                    dbEntry.ImageData = album.ImageData;
                    dbEntry.ImageMimeType = album.ImageMimeType;
                    dbEntry.Price = album.Price;
                    dbEntry.TrackList = album.TrackList;
                }
            }
            //context.Entry(album).State = EntityState.Modified;
            context.SaveChanges();
        }

        //Saving relies on the default model binder
        //public void SaveAlbum(Album album)
        //{
        //    if (album.AlbumId == 0)
        //    {
        //        context.Albums.Add(album);
        //    }            
        //    context.Entry(album).State = EntityState.Modified;
        //    context.SaveChanges();
        //}
        #endregion

        #region Delete
        public Album DeleteAlbum(int albumId)
        {
            Album dbEntry = context.Albums.Find(albumId);
            if (dbEntry != null)
            {
                context.Albums.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        #endregion
        #endregion Album, Genre, Artist

        #region Cart, Order, OrderDetail
        public IEnumerable<Cart> Carts
        {
            get 
            { 
                return context.Carts
                .Include(c => c.Album); 
            }
        }

        public IEnumerable<Order> Orders
        {
            get { return context.Orders; }
        }

        public IEnumerable<OrderDetail> OrderDetails
        {
            get { return context.OrderDetails; }
        }

        public void AddLine(Cart cartLine)
        {
            context.Carts.Add(cartLine);
            context.SaveChanges();
        }

        /*If a line has already existed*/
        public void AddLine(int lineId, int quantity)
        {
            var line = context.Carts.FirstOrDefault(c => c.LineId == lineId);
            line.Count += quantity;
            context.SaveChanges();
        }

        public void AddLine(string shoppingCartId, int albumId, int quantity)
        {           
            // Get the matching cart and album instances
            var line = context.Carts.SingleOrDefault(
                c => c.CartId == shoppingCartId
                && c.AlbumId == albumId);

            if (line == null)
            {
                // Create a new cart line if no cart item exists
                Cart newLine = new Cart
                {
                    CartId = shoppingCartId,
                    AlbumId = albumId,
                    Count = quantity,
                    DateCreated = DateTime.Now
                };
                context.Carts.Add(newLine);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity
                line.Count += quantity;
            }
            context.SaveChanges();            
        }


        public void UpdateLine(Cart line, string userName)
        {
            line.CartId = userName;
            context.SaveChanges();
        }

        public void RemoveLine(Cart cartLine)
        {
            context.Carts.Remove(cartLine);
            context.SaveChanges();
        }

        //public void RemoveLine(Cart cartLine, int lineId, int quantity)
        //{
        //    var line = context.Carts.FirstOrDefault(c => c.CartId == cartLine.CartId && c.LineId == lineId);
        //    line.Count -= quantity;
        //    context.SaveChanges();
        //}

        public int RemoveLine(string shoppingCartId, int lineId, int? quantity)
        {
            var cartLine = context.Carts.Single(cart => cart.CartId == shoppingCartId && cart.LineId == lineId);
            if (cartLine != null)
            {
                if (cartLine.Count == 1 || quantity == null || quantity.Value <= 0)
                {
                    context.Carts.Remove(cartLine);
                }
                else
                {
                    //var line = context.Carts.FirstOrDefault(c => c.CartId == cartLine.CartId && c.LineId == lineId);
                    if (cartLine.Count > 1)
                    {
                        cartLine.Count -= quantity.Value;
                        context.SaveChanges();
                    }
                    return cartLine.Count;
                }
                context.SaveChanges();
            }
            return 0;
        }

        public void EmptyCart(string id)
        {
            IEnumerable<Cart> carts = context.Carts.Where(c => c.CartId == id);
            
            foreach (Cart cart in carts)
                context.Carts.Remove(cart);
            context.SaveChanges();
        }

        public void CreateOrderDetail(OrderDetail orderDetail)
        {
            OrderDetail newOrderDetail = new OrderDetail()
            {
                CustomerId = orderDetail.CustomerId,
                Status = orderDetail.Status,

                OrderDate = orderDetail.OrderDate,
                Total = orderDetail.Total,
                Name = orderDetail.Name,
                Email = orderDetail.Email,
                PhoneNumber = orderDetail.PhoneNumber,
                Adrress = orderDetail.Adrress,
                City = orderDetail.City,
                Country = orderDetail.Country,
                State = orderDetail.State,
                Zip = orderDetail.Zip,
                GiftWrap = orderDetail.GiftWrap
            };
            context.OrderDetails.Add(newOrderDetail);
            context.SaveChanges();
        }

        public string ProcessOrder(int id, bool isAdmin)
        {
            var orderDetail = context.OrderDetails.Single(od => od.OrderDetailId == id);

            //If the user is not Admin, he can only cancel his orders
            if (!isAdmin)
            {
                orderDetail.Status = Status.Canceled;
            }
            //Admin will check the inventory and decide to deliver the items
            else
            {
                if (orderDetail.Status == Status.Processing)
                    orderDetail.Status = Status.Delivering;
                else if (orderDetail.Status == Status.Delivering)
                    orderDetail.Status = Status.Delivered;
            }
                      
            context.SaveChanges();

            return orderDetail.Status.ToString();
        }

        public void CreateOrder(Order order)
        {          
            // Iterate over the items in the cart, adding the order details for each
            //(Other fields in ShippingDetails were added in CartController - Checkout() action)

            int newOrderDetailId = context.OrderDetails.Max(o => o.OrderDetailId);

            Order newOrder = new Order
            {
                AlbumId = order.AlbumId,
                OrderDetailId = newOrderDetailId,
                UnitPrice = order.UnitPrice,
                Quantity = order.Quantity,
            };
            context.Orders.Add(newOrder);         
            context.SaveChanges();
        }
        #endregion

    }      
    public enum SearchCriteria
    {
        Titles,
        Genres,
        Artists
    }
}

//get cart lines based on CartId
//public IEnumerable<Cart> GetCartLines(string shoppingCartId)
//{
//    return context.Carts.Where(cart => cart.CartId == shoppingCartId);
//}