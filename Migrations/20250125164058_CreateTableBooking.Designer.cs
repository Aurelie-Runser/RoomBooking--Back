﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomBookingApi.Data;

#nullable disable

namespace ReservationDeSalle__Back.Migrations
{
    [DbContext(typeof(RoomApiContext))]
    [Migration("20250125164058_CreateTableBooking")]
    partial class CreateTableBooking
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("RoomBookingApi.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTo")
                        .HasColumnType("TEXT");

                    b.Property<int>("IdOrganizer")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdRoom")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Statut")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("RoomBookingApi.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Adress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AdressComplements")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Area")
                        .HasColumnType("TEXT");

                    b.Property<int>("Capacity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Groupe")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAccessible")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Picture")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Surface")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("RoomBookingApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Company")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Job")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
