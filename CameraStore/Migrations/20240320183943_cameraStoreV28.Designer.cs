﻿// <auto-generated />
using System;
using CameraStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CameraStore.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240320183943_cameraStoreV28")]
    partial class cameraStoreV28
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CameraStore.Models.Cart", b =>
                {
                    b.Property<int>("cartID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("cartID"));

                    b.Property<decimal>("cartPriceTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("cartQuantityTotal")
                        .HasColumnType("int");

                    b.Property<int>("customerID")
                        .HasColumnType("int");

                    b.Property<DateTime>("timeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("cartID");

                    b.HasIndex("customerID");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("CameraStore.Models.CartDetails", b =>
                {
                    b.Property<int>("cartID")
                        .HasColumnType("int");

                    b.Property<int>("proID")
                        .HasColumnType("int");

                    b.Property<decimal>("price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("quantity")
                        .HasColumnType("int");

                    b.HasKey("cartID", "proID");

                    b.HasIndex("proID");

                    b.ToTable("CartDetails");
                });

            modelBuilder.Entity("CameraStore.Models.Category", b =>
                {
                    b.Property<int>("cateID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("cateID"));

                    b.Property<string>("cateDescription")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("cateName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("cateUrlImage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("cateID");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CameraStore.Models.Customer", b =>
                {
                    b.Property<int>("customerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("customerID"));

                    b.Property<DateTime?>("createAt")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fullname")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("roleID")
                        .HasColumnType("int");

                    b.Property<string>("telephone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("customerID");

                    b.HasIndex("roleID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("CameraStore.Models.Feedback", b =>
                {
                    b.Property<int>("feedID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("feedID"));

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("customerID")
                        .HasColumnType("int");

                    b.Property<string>("feedDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("proID")
                        .HasColumnType("int");

                    b.HasKey("feedID");

                    b.HasIndex("customerID");

                    b.HasIndex("proID");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("CameraStore.Models.Order", b =>
                {
                    b.Property<int>("orderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("orderID"));

                    b.Property<int>("customerID")
                        .HasColumnType("int");

                    b.Property<string>("orderAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("orderDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("orderDelivery")
                        .HasColumnType("datetime2");

                    b.Property<string>("orderPhone")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("orderStatus")
                        .HasColumnType("bit");

                    b.Property<string>("paymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("totalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("orderID");

                    b.HasIndex("customerID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("CameraStore.Models.OrderDetail", b =>
                {
                    b.Property<int>("orderID")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("proID")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<int>("quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("unitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("orderID", "proID");

                    b.HasIndex("proID");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("CameraStore.Models.Product", b =>
                {
                    b.Property<int>("proID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("proID"));

                    b.Property<int>("cateID")
                        .HasColumnType("int");

                    b.Property<DateTime>("proDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("proDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("proName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("proPercent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("proPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("proQuantity")
                        .HasColumnType("int");

                    b.Property<decimal>("proSale")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("proStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("proUrlImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("supID")
                        .HasColumnType("int");

                    b.HasKey("proID");

                    b.HasIndex("cateID");

                    b.HasIndex("supID");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("CameraStore.Models.Role", b =>
                {
                    b.Property<int>("roleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("roleID"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("roleID");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("CameraStore.Models.Supplier", b =>
                {
                    b.Property<int>("supID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("supID"));

                    b.Property<string>("supAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("supDescription")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("supName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("supTelephone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("supID");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("CameraStore.Models.Cart", b =>
                {
                    b.HasOne("CameraStore.Models.Customer", "Customer")
                        .WithMany("Carts")
                        .HasForeignKey("customerID")
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("CameraStore.Models.CartDetails", b =>
                {
                    b.HasOne("CameraStore.Models.Cart", "Cart")
                        .WithMany("CartDetails")
                        .HasForeignKey("cartID")
                        .IsRequired();

                    b.HasOne("CameraStore.Models.Product", "Product")
                        .WithMany("CartDetails")
                        .HasForeignKey("proID")
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("CameraStore.Models.Customer", b =>
                {
                    b.HasOne("CameraStore.Models.Role", "Role")
                        .WithMany("Customers")
                        .HasForeignKey("roleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("CameraStore.Models.Feedback", b =>
                {
                    b.HasOne("CameraStore.Models.Customer", "Customer")
                        .WithMany("Feedbacks")
                        .HasForeignKey("customerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CameraStore.Models.Product", "Product")
                        .WithMany("Feedbacks")
                        .HasForeignKey("proID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("CameraStore.Models.Order", b =>
                {
                    b.HasOne("CameraStore.Models.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("customerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("CameraStore.Models.OrderDetail", b =>
                {
                    b.HasOne("CameraStore.Models.Order", "Order")
                        .WithMany("orderdetails")
                        .HasForeignKey("orderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CameraStore.Models.Product", "Product")
                        .WithMany("orderdetails")
                        .HasForeignKey("proID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("CameraStore.Models.Product", b =>
                {
                    b.HasOne("CameraStore.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("cateID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CameraStore.Models.Supplier", "Supplier")
                        .WithMany("Products")
                        .HasForeignKey("supID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("CameraStore.Models.Cart", b =>
                {
                    b.Navigation("CartDetails");
                });

            modelBuilder.Entity("CameraStore.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CameraStore.Models.Customer", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Feedbacks");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("CameraStore.Models.Order", b =>
                {
                    b.Navigation("orderdetails");
                });

            modelBuilder.Entity("CameraStore.Models.Product", b =>
                {
                    b.Navigation("CartDetails");

                    b.Navigation("Feedbacks");

                    b.Navigation("orderdetails");
                });

            modelBuilder.Entity("CameraStore.Models.Role", b =>
                {
                    b.Navigation("Customers");
                });

            modelBuilder.Entity("CameraStore.Models.Supplier", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
