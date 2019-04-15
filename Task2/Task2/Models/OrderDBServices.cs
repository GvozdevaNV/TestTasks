using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace Task2.Models
{
    public class OrderDBServices
    {
        public List<OrderItem> CountOrderItemsAmount(OrderContext db, List<OrderItem> orderItems)
        {
            foreach (OrderItem oi in orderItems)
            {
                oi.Amount = oi.Price * oi.Count;
            }
            return orderItems;
        }

        public List<Order> CountOrderAmount(OrderContext db, List<Order> orders)
        {
            foreach (Order o in orders)
            {
                o.Amount = o.Items.Sum(i => i.Amount);
            }
            return orders;
        }

        public void CreateOrderItem(OrderContext db, OrderItem orderItem)
        {
            var orderItems = CountOrderItemsAmount(db, new List<OrderItem> { orderItem });
            db.OrderItems.AddRange(orderItems);

            UpdateOrderAmount(db, orderItem, orderItem.Amount);
            db.OrderItems.Add(orderItem);
            db.SaveChanges();
        }

        public void EditOrderItem(OrderContext db, OrderItem updatedOrderItem)
        {
            OrderItem oldOrderItem = db.OrderItems.Find(updatedOrderItem.Id);
            decimal oldPrice, oldAmount, newPrice, newAmount, divAmount;
            int oldCount, newCount;
            oldCount = oldOrderItem.Count;
            oldPrice = oldOrderItem.Price;
            oldAmount = oldOrderItem.Amount;
            newCount = updatedOrderItem.Count;
            newPrice = updatedOrderItem.Price;

            if (oldOrderItem.Title != updatedOrderItem.Title)
                oldOrderItem.Title = updatedOrderItem.Title;

            if (oldCount != newCount && oldPrice != newPrice)
            {
                oldOrderItem.Count = newCount;
                oldOrderItem.Price = newPrice;
                newAmount = CountOrderItemAmount(db, oldOrderItem);
            }
            else if(oldCount != newCount)
            {
                oldOrderItem.Count = newCount;
                newAmount = CountOrderItemAmount(db, oldOrderItem);
            }
            else if(oldPrice != newPrice)
            {
                oldOrderItem.Price = newPrice;
                newAmount = CountOrderItemAmount(db, oldOrderItem);
            }            
            else
            {
                newAmount = oldAmount;
            }

            divAmount = newAmount - oldAmount;
            UpdateOrderAmount(db, oldOrderItem, divAmount);
            db.SaveChanges();
        }

        public void DeleteOrderItem(OrderContext db, OrderItem orderItem)
        {
            decimal oldAmount, divAmount;
            var oldOrderItem = db.OrderItems.Find(orderItem.Id);
            oldAmount = oldOrderItem.Amount;
            divAmount = -oldAmount;
            db.OrderItems.Remove(orderItem);

            UpdateOrderAmount(db, orderItem, divAmount);
            db.SaveChanges();
        }

        public void CreateOrder(OrderContext db)
        {
            Order newOrder = new Order { Amount = 0 };
            db.Orders.Add(newOrder);
            db.SaveChanges();
        }

        public void DeleteOrder(OrderContext db, Order order)
        {
            db.Orders.Remove(order);
            db.SaveChanges();
        }

        private decimal CountOrderItemAmount(OrderContext db, OrderItem orderItem)
        {
            return db.OrderItems.Find(orderItem.Id).Amount = orderItem.Count * orderItem.Price;
        }

        private void UpdateOrderAmount(OrderContext db, OrderItem orderItem, decimal divAmount)
        {
            var order = db.Orders.Find(orderItem.OrderId);
            order.Amount += divAmount;
        }
    }
}