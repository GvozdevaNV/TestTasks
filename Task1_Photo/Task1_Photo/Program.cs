using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Task1_Photo.Model;

namespace Task1_Photo
{
    class Program
    {
        static void Main(string[] args)
        {
            Photo photo;
            using (PhotoContext db = new PhotoContext())
            {
                photo = db.Photos.Single(p => p.Id == 1);
                PrintPhotosInfo();
                Console.WriteLine("Кол-во лайков: {0}", photo.GetLikesCount());
            }

            Task task1 = Task.Run(() => UpdatePhotoPrimary());
            Task task2 = Task.Run(() => UpdatePhotoSecondary());
            Task.WaitAll(task1, task2);
            PrintPhotosInfo();
            Console.WriteLine("Кол-во лайков: {0}", photo.GetLikesCount());
            Console.Read();
        }

        private static void UpdatePhotoPrimary()
        {
            using (PhotoContext db = new PhotoContext())
            {
                Thread.CurrentThread.Name = "Primary";
                Console.WriteLine("Запущен поток {0}\n", Thread.CurrentThread.Name);
                Photo photo = db.Photos.Single(p => p.Id == 1);
                using (var tran1 = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        photo.AddLike(db);
                        Console.WriteLine("Поток {0} завершил работу\n", Thread.CurrentThread.Name);
                        tran1.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        tran1.Rollback();
                    }
                }
            }
        }
        private static void UpdatePhotoSecondary()
        {
            using (PhotoContext db = new PhotoContext())
            {
                Thread.CurrentThread.Name = "Secondary";
                Console.WriteLine("Запущен поток {0}\n", Thread.CurrentThread.Name);
                Photo photo = db.Photos.Single(p => p.Id == 1);
                using (var tran2 = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        photo.AddLike(db);
                        Console.WriteLine("Поток {0} завершил работу\n", Thread.CurrentThread.Name);
                        tran2.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        tran2.Rollback();
                    }
                }
            }
        }

        private static void PrintPhotoInfo(Photo p)
        {
            Console.WriteLine("Фото №{0} Кол-во лайков: {1}", p.Id, p.Likes);
        }

        private static void PrintPhotosInfo()
        {
            using (PhotoContext db = new PhotoContext())
            {
                foreach (Photo p in db.Photos)
                {
                    PrintPhotoInfo(p);
                }
            }
        }
    }
}
