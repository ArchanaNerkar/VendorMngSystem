using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using VendorMngSystem.Models;

namespace VendorMngSystem.DbData
{
    public class DataDbContext:DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext>options):base(options) 
        {
        }
         public DbSet<VendorsCategory> VendorsCategories { get; set; }
         public DbSet<Vendors> Vendors { get; set; }
         public DbSet<VendorDocuments> VendorDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //define Relationship
           modelBuilder.Entity<Vendors>().HasOne(x=>x.vendorsCategories).WithMany(x=>x.vendors).HasForeignKey(x=>x.VendorsCategoryId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VendorDocuments>()
                .HasOne(d => d.vendors).WithMany(d => d.vendorDocuments)
                .HasForeignKey(d => d.VendorId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
