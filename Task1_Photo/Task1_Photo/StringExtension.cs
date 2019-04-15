using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1_Photo.Model;

namespace Task1_Photo
{
    public static class StringExtension
    {
        public static void AddLike(this Photo photo, PhotoContext db)
        {
            db.Photos.Single(p => p.Id == photo.Id).Likes++;
            db.SaveChanges();
        }

        public static int GetLikesCount(this Photo photo)
        {
            using (PhotoContext db = new PhotoContext())
            {
                return db.Photos.Single(p => p.Id == photo.Id).Likes;
            }
        }
    }
}
