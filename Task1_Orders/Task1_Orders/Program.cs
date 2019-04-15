using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.Entity;
using Task1_Orders.Models;
using System.Threading.Tasks;

namespace Task1_Orders
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (OrderContext db = new OrderContext())
            //{
            //    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
            //      string.Format("ALTER DATABASE [{0}] SET READ_COMMITTED_SNAPSHOT ON ",
            //      db.Database.Connection.Database));

            //    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
            //      string.Format("ALTER DATABASE [{0}] SET ALLOW_SNAPSHOT_ISOLATION ON",
            //      db.Database.Connection.Database));
            //}

            PrintOrderItemsByOrder();

            Task task1 = Task.Factory.StartNew(UpdateOrderPrimary);
            Task task2 = Task.Factory.StartNew(UpdateOrderSecondary);
            Task.WaitAll(task1, task2);

            PrintOrderItemsByOrder();

            Console.Read();
        }

        private static void UpdateOrderPrimary()
        {
            using (OrderContext db = new OrderContext())
            {
                Thread.CurrentThread.Name = "Primary";
                Console.WriteLine("Запущен поток {0}\n", Thread.CurrentThread.Name);
                var order = db.Orders.Include(x => x.Items).Single(o => o.Id == 1);
                using (var tranPrimary = db.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
                {
                    try
                    {
                        new OrderDBServices().InsertOrderItem(db, new OrderItem { Title = "Primary", Count = 1, Price = 1.0M, Order = order });
                        Console.WriteLine("Поток {0} завершил работу\n", Thread.CurrentThread.Name);
                        tranPrimary.Commit();
                        db.SaveChanges();
                    }
                    catch
                    {
                        tranPrimary.Rollback();
                    }
                }
            }
        }

        private static void UpdateOrderSecondary()
        {
            using (OrderContext db = new OrderContext())
            {
                Thread.CurrentThread.Name = "Secondary";
                Console.WriteLine("Запущен поток {0}\n", Thread.CurrentThread.Name);
                var order = db.Orders.Include(x => x.Items).Single(o => o.Id == 1);
                using (var tranSecondary = db.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
                {
                    try
                    {
                        new OrderDBServices().InsertOrderItem(db, new OrderItem { Title = "Secondary", Count = 1, Price = 1.0M, Order = order });
                        Console.WriteLine("Поток {0} завершил работу\n", Thread.CurrentThread.Name);
                        tranSecondary.Commit();
                        db.SaveChanges();
                    }
                    catch
                    {
                        tranSecondary.Rollback();
                    }
                }
            }
        }

        private static void PrintOrderItems()
        {
            using (OrderContext db = new OrderContext())
            {
                var orderItems = new OrderDBServices().GetOrderItems(db);
                foreach (OrderItem oi in orderItems)
                {
                    Console.WriteLine("Id: {0}, Title: {1}, Count: {2}, Price: {3} Amount: {4}, OrderId: {5}", oi.Id, oi.Title, oi.Count, oi.Price, oi.Amount, oi.OrderId);
                }
                Console.WriteLine();
            }
        }

        private static void PrintOrders()
        {
            using (OrderContext db = new OrderContext())
            {
                var orders = new OrderDBServices().GetOrderItemsByOrder(db);
                foreach (Order o in orders)
                {
                    Console.Write("Заказ №" + o.Id + "\t");
                    Console.WriteLine("Сумма по итемам: " + o.Items.Sum(oi => oi.Amount));
                    Console.WriteLine("Total amount: " + o.Amount);
                }
            }
            Console.WriteLine();
        }

        private static void PrintOrderItemsByOrder()
        {
            using (OrderContext db = new OrderContext())
            {
                var orders = new OrderDBServices().GetOrderItemsByOrder(db);
                foreach (Order o in orders)
                {
                    Console.WriteLine("Заказ №" + o.Id + "\n");
                    Console.WriteLine("Сумма по итемам: " + o.Items.Sum(oi => oi.Amount));
                    Console.WriteLine("Total amount: " + o.Amount);
                    foreach (OrderItem oi in o.Items)
                    {
                        Console.WriteLine("Id: {0} Title: {1} Count: {2} Price: {3} Amount: {4}", oi.Id, oi.Title, oi.Count, oi.Price, oi.Amount);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
    }
}
