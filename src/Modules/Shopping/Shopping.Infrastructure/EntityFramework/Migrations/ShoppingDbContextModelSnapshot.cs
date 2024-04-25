﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Shopping.Infrastructure.EntityFramework;

#nullable disable

namespace Modules.Shopping.Infrastructure.EntityFramework.Migrations
{
    [DbContext(typeof(ShoppingDbContext))]
    partial class ShoppingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("shopping")
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BridgingIT.DevKit.Infrastructure.EntityFramework.Storage.StorageDocument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("PartitionKey")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("PropertiesJson")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Properties");

                    b.Property<string>("RowKey")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<DateTimeOffset?>("UpdatedDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("PartitionKey");

                    b.HasIndex("RowKey");

                    b.HasIndex("Type");

                    b.ToTable("__Storage_Documents", "shopping");
                });

            modelBuilder.Entity("Modules.Shopping.Domain.Cart", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Identity")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id");

                    b.HasIndex("Identity")
                        .IsUnique();

                    b.ToTable("Carts", "shopping");
                });

            modelBuilder.Entity("Modules.Shopping.Domain.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("Id");

                    b.ToTable("Tenants", "shopping");
                });

            modelBuilder.Entity("Modules.Shopping.Domain.Cart", b =>
                {
                    b.OwnsOne("BridgingIT.DevKit.Domain.Model.AuditState", "AuditState", b1 =>
                        {
                            b1.Property<Guid>("CartId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("CreatedBy")
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)");

                            b1.Property<DateTimeOffset>("CreatedDate")
                                .HasColumnType("datetimeoffset");

                            b1.Property<string>("CreatedDescription")
                                .HasMaxLength(1024)
                                .HasColumnType("nvarchar(1024)");

                            b1.Property<bool?>("Deactivated")
                                .HasColumnType("bit");

                            b1.Property<string>("DeactivatedBy")
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)");

                            b1.Property<DateTimeOffset?>("DeactivatedDate")
                                .HasColumnType("datetimeoffset");

                            b1.Property<string>("DeactivatedDescription")
                                .HasMaxLength(1024)
                                .HasColumnType("nvarchar(1024)");

                            b1.Property<string>("DeactivatedReasons")
                                .HasMaxLength(8192)
                                .HasColumnType("nvarchar(max)");

                            b1.Property<bool?>("Deleted")
                                .HasColumnType("bit");

                            b1.Property<string>("DeletedBy")
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)");

                            b1.Property<DateTimeOffset?>("DeletedDate")
                                .HasColumnType("datetimeoffset");

                            b1.Property<string>("DeletedDescription")
                                .HasMaxLength(1024)
                                .HasColumnType("nvarchar(1024)");

                            b1.Property<string>("DeletedReason")
                                .HasMaxLength(1024)
                                .HasColumnType("nvarchar(1024)");

                            b1.Property<string>("UpdatedBy")
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)");

                            b1.Property<DateTimeOffset?>("UpdatedDate")
                                .HasColumnType("datetimeoffset");

                            b1.Property<string>("UpdatedDescription")
                                .HasMaxLength(1024)
                                .HasColumnType("nvarchar(1024)");

                            b1.Property<string>("UpdatedReasons")
                                .HasMaxLength(8192)
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("CartId");

                            b1.ToTable("Carts", "shopping");

                            b1.WithOwner()
                                .HasForeignKey("CartId");
                        });

                    b.OwnsMany("Modules.Shopping.Domain.CartItem", "Items", b1 =>
                        {
                            b1.Property<Guid>("CartId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Quantity")
                                .HasColumnType("int");

                            b1.Property<decimal>("TotalPrice")
                                .HasColumnType("decimal(18, 2)");

                            b1.HasKey("CartId", "Id");

                            b1.ToTable("CartItems", "shopping");

                            b1.WithOwner()
                                .HasForeignKey("CartId");

                            b1.OwnsOne("Modules.Shopping.Domain.CartProduct", "Product", b2 =>
                                {
                                    b2.Property<Guid>("CartItemCartId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<Guid>("CartItemId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasMaxLength(512)
                                        .HasColumnType("nvarchar(512)")
                                        .HasColumnName("Name");

                                    b2.Property<string>("SKU")
                                        .IsRequired()
                                        .HasMaxLength(256)
                                        .HasColumnType("nvarchar(256)")
                                        .HasColumnName("SKU");

                                    b2.Property<decimal>("UnitPrice")
                                        .HasColumnType("decimal(18, 2)")
                                        .HasColumnName("UnitPrice");

                                    b2.HasKey("CartItemCartId", "CartItemId");

                                    b2.ToTable("CartItems", "shopping");

                                    b2.WithOwner()
                                        .HasForeignKey("CartItemCartId", "CartItemId");
                                });

                            b1.Navigation("Product");
                        });

                    b.Navigation("AuditState");

                    b.Navigation("Items");
                });

            modelBuilder.Entity("Modules.Shopping.Domain.Tenant", b =>
                {
                    b.OwnsMany("Modules.Shopping.Domain.CartId", "CartIds", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<Guid>("TenantId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Value")
                                .HasColumnType("uniqueidentifier")
                                .HasColumnName("CartId");

                            b1.HasKey("Id");

                            b1.HasIndex("TenantId");

                            b1.ToTable("TenantCarts", "shopping");

                            b1.WithOwner()
                                .HasForeignKey("TenantId");
                        });

                    b.Navigation("CartIds");
                });
#pragma warning restore 612, 618
        }
    }
}
