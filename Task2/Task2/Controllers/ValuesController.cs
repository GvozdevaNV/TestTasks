using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using Task2.Models;

namespace Task2.Controllers
{
    public class ValuesController : ApiController
    {
        OrderContext db = new OrderContext();
        OrderDBServices dbServices = new OrderDBServices();

        public IEnumerable<Order> GetOrders()
        {
            return db.Orders;
        }

        [Route("api/values/getorder/{id}")]
        public Order GetOrder(int id)
        {
            return db.Orders.Find(id);
        }

        [Route("api/values/getorderitems/{id}")]
        public IEnumerable<OrderItem> GetOrderItems(int id)
        {
            return db.OrderItems.Where(oi => oi.OrderId == id);
        }

        [Route("api/values/getorderitem/{id}")]
        public OrderItem GetOrderItem(int id)
        {
            OrderItem orderItem = db.OrderItems.Find(id);
            return orderItem;
        }

        [HttpPost]
        [Route("api/values/createorderitem/")]
        public HttpResponseMessage CreateOrderItem([FromBody]OrderItem orderItem)
        {
            if (orderItem.Title == "")
                ModelState.AddModelError("orderItem.Count", "Название товара не должно быть пустым");

            if (orderItem.Count <= 0)
                ModelState.AddModelError("orderItem.Count", "Количество товара должно быть больше 0");

            if (orderItem.Price <= 0)
                ModelState.AddModelError("orderItem.Price", "Цена товара должна быть больше 0");

            if (ModelState.IsValid)
            {
                dbServices.CreateOrderItem(db, orderItem);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [HttpPut]
        public HttpResponseMessage EditOrderItem(int id, [FromBody]OrderItem orderItem)
        {
            if (orderItem.Title == "")
                ModelState.AddModelError("orderItem.Count", "Название товара не должно быть пустым");

            if (orderItem.Count <= 0)
                ModelState.AddModelError("orderItem.Count", "Количество товара должно быть больше 0");

            if (orderItem.Price <= 0)
                ModelState.AddModelError("orderItem.Price", "Цена товара должна быть больше 0");

            if (ModelState.IsValid)
            {
                dbServices.EditOrderItem(db, orderItem);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/values/deleteorderitem/{id}")]
        public void DeleteOrderItem(int id)
        {
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem != null)
            {
                dbServices.DeleteOrderItem(db, orderItem);
            }
        }

        [HttpPost]
        [Route("api/values/createorder/")]
        public void CreateOrder()
        {
            dbServices.CreateOrder(db);
        }

        [Route("api/values/deleteorder/{id}")]
        public void DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order != null)
            {
                dbServices.DeleteOrder(db, order);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
