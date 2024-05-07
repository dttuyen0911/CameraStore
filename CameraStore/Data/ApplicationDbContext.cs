using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CameraStore.Models;

namespace CameraStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>().HasKey(m => new { m.orderID, m.proID });
            modelBuilder.Entity<CartDetails>().HasKey(cd => new { cd.cartID, cd.proID });  // Define composite primary key

            modelBuilder.Entity<CartDetails>()
                .HasOne(c => c.Cart)
                .WithMany(c => c.CartDetails)
                .HasForeignKey(c => c.cartID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartDetails>()
                .HasOne(c => c.Product)
                .WithMany(c => c.CartDetails)
                .HasForeignKey(c => c.proID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
               .HasOne(c => c.Customer)
               .WithMany(cust => cust.Carts)
               .HasForeignKey(c => c.customerID)
               .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.orderdetails)
                .HasForeignKey(od => od.orderID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.orderdetails)
                .HasForeignKey(od => od.proID)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Feedback>()
                 .HasOne(f => f.Order)
                 .WithMany()
                 .HasForeignKey(f => f.orderID)
                 .HasConstraintName("FK_Feedbacks_Orders_orderID") 
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}