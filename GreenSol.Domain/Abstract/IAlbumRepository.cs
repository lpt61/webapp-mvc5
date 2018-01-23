using GreenSol.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSol.Domain.Abstract
{
    public interface IAlbumRepository
    {
        IEnumerable<Artist> Artists { get; }
        IEnumerable<Genre> Genres { get; }
        IEnumerable<Album> Albums { get; }
        IEnumerable<Cart> Carts { get;}
        IEnumerable<Order> Orders { get; }
        IEnumerable<OrderDetail> OrderDetails { get;}

        void SaveAlbum(Album album);
        Album DeleteAlbum(int albumId);

        void AddLine(Cart cartline);
        void AddLine(int lineId, int quantity);
        void AddLine(string cartId, int albumId, int quantity);
        void UpdateLine(Cart line, string userName);
        //IEnumerable<Cart> GetCartLines(string shoppingCartId);
        //void RemoveLine(Cart cartLine);
        int RemoveLine(string cartId, int lineId, int? quantity);
        void EmptyCart(string id);
        void CreateOrderDetail(OrderDetail orderDetail);
        string ProcessOrder(int id, bool isAdmin);
        void CreateOrder(Order order);
    }
}
