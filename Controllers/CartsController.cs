using EcommerceCartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ECommerceContext _context;
        public CartsController(ECommerceContext context)
        {
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Cart>> Get(int userId)
        {
            List<Cart> carts = _context.Carts.Where(x => x.UserId == userId).ToList();
            return carts;
        }

        [HttpPost]
        public string insert(int Prdid, int userId)
        {
            if (Prdid == 0)
            {
                return "";
            }
            try
            {
                var product = _context.Products.Find(Prdid);
                if (product == null)
                {
                    return "";
                }
                Cart c = new Cart();
                c.UserId = userId;
                c.Price = product.Price;
                c.ProductId = product.ProductId;
                c.ProductName = product.ProductName;
                var update =
             _context.Carts.Where(x => x.ProductId == product.ProductId && x.UserId == userId)
              .FirstOrDefault();
                if (update == null)
                {
                    c.Quantity = 1;
                    c.SubTotal = c.Price * c.Quantity;
                    _context.Carts.Add(c);
                    _context.SaveChanges();
                    return c.ProductName+" added";
                }
                else
                {
                    c.Quantity = update.Quantity + 1;
                    c.SubTotal = c.Price * c.Quantity;
                    var updatequery = _context.Carts.Where(x => x.ProductId == product.ProductId && x.UserId == userId)
                  .FirstOrDefault();
                    updatequery.Quantity = c.Quantity;
                    updatequery.SubTotal = c.SubTotal;
                    _context.SaveChanges();
                    return c.ProductName+" already exist so Quantity increased";
                }
            }
            catch (Exception e)
            {
                return "invalid";
            }
        }
        //Removing 
        [HttpPut]
        public string Remove(int Prdid, int userId)
        {
            var cart = _context.Carts.Where(x => x.ProductId == Prdid && x.UserId == userId).FirstOrDefault(); ;

            var update =
         _context.Carts.Where(x => x.ProductId == Prdid && x.UserId == userId)
          .FirstOrDefault();


            if (update != null && update.Quantity == 1)
            {
                _context.Carts.Remove(cart);
                _context.SaveChangesAsync();
                return "success";
            }
            else
            {
                cart.Quantity = update.Quantity - 1;
                cart.SubTotal = cart.Quantity * cart.Price;
                var updatequery = _context.Carts.Where(x => x.ProductId == cart.ProductId && x.UserId == userId)
              .FirstOrDefault();
                updatequery.Quantity = cart.Quantity;
                updatequery.SubTotal = cart.SubTotal;
                _context.SaveChanges();
                return "success";
            }

        }



    }
}
