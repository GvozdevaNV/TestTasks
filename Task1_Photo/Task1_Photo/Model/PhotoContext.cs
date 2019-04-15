using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Task1_Photo.Model
{
    public class PhotoContext : DbContext
    {
        public PhotoContext()
            : base("DefConnection")
        { }

        public virtual DbSet<Photo> Photos { get; set; }
    }
}
