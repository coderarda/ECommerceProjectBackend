﻿// <auto-generated />
using System;
using ECommerceProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ECommerceProject.Migrations
{
    [DbContext(typeof(ECommerceDbContext))]
    partial class ECommerceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("ECommerceProject.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("ProductDescription")
                        .HasColumnType("longtext");

                    b.Property<uint>("ProductPrice")
                        .HasColumnType("int unsigned");

                    b.Property<string>("ProductTitle")
                        .HasColumnType("longtext");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ECommerceProject.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime>("UserBirthDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserEmail")
                        .HasColumnType("longtext");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserPassword")
                        .HasColumnType("longtext");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}