﻿// <auto-generated />
using System;
using InventoryApp.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InventoryApp.Migrations
{
    [DbContext(typeof(InventoryAppDbContext))]
    [Migration("20241203074949_seeding_2")]
    partial class seeding_2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("InventoryApp.Models.Entity.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ForgotCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("MailVerificationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "a@a.com",
                            IsDeleted = false,
                            IsVerified = true,
                            Name = "A",
                            Password = "111",
                            Salt = "111"
                        },
                        new
                        {
                            Id = 2,
                            Email = "s@s.com",
                            IsDeleted = false,
                            IsVerified = true,
                            Name = "S",
                            Password = "111",
                            Salt = "111"
                        });
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Inventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DeliveredDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GivenByEmployeeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTaken")
                        .HasColumnType("bit");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ReceivedByEmployeeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("GivenByEmployeeId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ReceivedByEmployeeId");

                    b.ToTable("Inventories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DeliveredDate = new DateTime(2024, 12, 3, 7, 49, 49, 5, DateTimeKind.Utc).AddTicks(1695),
                            GivenByEmployeeId = 1,
                            IsDeleted = false,
                            IsTaken = false,
                            ProductId = 1,
                            ReceivedByEmployeeId = 2
                        },
                        new
                        {
                            Id = 2,
                            DeliveredDate = new DateTime(2024, 12, 3, 7, 49, 49, 5, DateTimeKind.Utc).AddTicks(2252),
                            GivenByEmployeeId = 2,
                            IsDeleted = false,
                            IsTaken = false,
                            ProductId = 2,
                            ReceivedByEmployeeId = 1,
                            ReturnDate = new DateTime(2024, 12, 3, 10, 49, 49, 5, DateTimeKind.Local).AddTicks(2258)
                        });
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FirmName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InvoiceNo")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Invoices");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirmName = "Tekno",
                            InvoiceNo = 4321,
                            IsDeleted = false,
                            Price = "6000",
                            PurchaseDate = new DateTime(2024, 12, 3, 10, 49, 49, 10, DateTimeKind.Local).AddTicks(4383)
                        },
                        new
                        {
                            Id = 2,
                            FirmName = "GardenShop",
                            InvoiceNo = 1234,
                            IsDeleted = false,
                            Price = "5000",
                            PurchaseDate = new DateTime(2024, 12, 3, 10, 49, 49, 10, DateTimeKind.Local).AddTicks(4667)
                        });
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Exception")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageTemplate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Properties")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceId");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            InvoiceId = 1,
                            IsDeleted = false,
                            Name = "Klavye",
                            ProductTypeId = 1
                        },
                        new
                        {
                            Id = 2,
                            InvoiceId = 2,
                            IsDeleted = false,
                            Name = "Kürek",
                            ProductTypeId = 2
                        });
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.ProductType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProductTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsDeleted = false,
                            Name = "Elektronik"
                        },
                        new
                        {
                            Id = 2,
                            IsDeleted = false,
                            Name = "Bahçe"
                        });
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Inventory", b =>
                {
                    b.HasOne("InventoryApp.Models.Entity.Employee", "GivenByEmployee")
                        .WithMany("InventoriesGiven")
                        .HasForeignKey("GivenByEmployeeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("InventoryApp.Models.Entity.Product", "Product")
                        .WithMany("Inventories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("InventoryApp.Models.Entity.Employee", "ReceivedByEmployee")
                        .WithMany("InventoriesReceived")
                        .HasForeignKey("ReceivedByEmployeeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("GivenByEmployee");

                    b.Navigation("Product");

                    b.Navigation("ReceivedByEmployee");
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Product", b =>
                {
                    b.HasOne("InventoryApp.Models.Entity.Invoice", "Invoice")
                        .WithMany("Products")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("InventoryApp.Models.Entity.ProductType", "ProductType")
                        .WithMany("Products")
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Employee", b =>
                {
                    b.Navigation("InventoriesGiven");

                    b.Navigation("InventoriesReceived");
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Invoice", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.Product", b =>
                {
                    b.Navigation("Inventories");
                });

            modelBuilder.Entity("InventoryApp.Models.Entity.ProductType", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
