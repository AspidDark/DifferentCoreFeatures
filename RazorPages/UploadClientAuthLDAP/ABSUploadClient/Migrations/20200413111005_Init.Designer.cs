﻿// <auto-generated />
using System;
using ABSUploadClient.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ABSUploadClient.Migrations
{
    [DbContext(typeof(PaymentOrdersContext))]
    [Migration("20200413111005_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ABSUploadClient.Entity.EntityModel.ModuleBrief", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Accout")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("ModuleValue")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("ModuleBriefs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Accout = "40702810010310008965",
                            ModuleValue = "VTBO"
                        },
                        new
                        {
                            Id = 2,
                            Accout = "40702810210310028965",
                            ModuleValue = "VTBR"
                        },
                        new
                        {
                            Id = 3,
                            Accout = "40702810469000031377",
                            ModuleValue = "SBRF"
                        },
                        new
                        {
                            Id = 4,
                            Accout = "40702810310310000427",
                            ModuleValue = "VTBFK"
                        },
                        new
                        {
                            Id = 5,
                            Accout = "40701810710310008965",
                            ModuleValue = "VTBR2"
                        });
                });

            modelBuilder.Entity("ABSUploadClient.Entity.EntityModel.PaymentOrderEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("AuthentificationData")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("CreditContractNumber")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("IncomeDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("PayerName")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("RequestRecivedOn")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("PaymentOrders");
                });
#pragma warning restore 612, 618
        }
    }
}
