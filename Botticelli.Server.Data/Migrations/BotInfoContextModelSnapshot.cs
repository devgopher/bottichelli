﻿// <auto-generated />
using System;
using Botticelli.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Botticelli.Server.Data.Migrations
{
    [DbContext(typeof(ServerDataContext))]
    partial class BotInfoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("Botticelli.Server.Data.Entities.Bot.BotAdditionalInfo", b =>
                {
                    b.Property<string>("BotId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BotInfoBotId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemValue")
                        .HasColumnType("TEXT");

                    b.HasKey("BotId");

                    b.HasIndex("BotInfoBotId");

                    b.ToTable("BotAdditionalInfo");
                });

            modelBuilder.Entity("Botticelli.Server.Data.Entities.Bot.BotInfo", b =>
                {
                    b.Property<string>("BotId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BotKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("BotName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastKeepAlive")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("BotId");

                    b.ToTable("BotInfo");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<string>", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ApplicationRoles");

                    b.HasData(
                        new
                        {
                            Id = "969600de-70d9-43e9-bc01-56415c57863b",
                            ConcurrencyStamp = "10/14/2024 11:29:05",
                            Name = "admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "b57f3fda-a6d3-4f93-8faa-ce99a06a476d",
                            ConcurrencyStamp = "10/14/2024 11:29:05",
                            Name = "bot_manager",
                            NormalizedName = "BOT_MANAGER"
                        },
                        new
                        {
                            Id = "22c98256-7794-4d42-bdc8-4725445de3c9",
                            ConcurrencyStamp = "10/14/2024 11:29:05",
                            Name = "viewer",
                            NormalizedName = "VIEWER"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser<string>", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ApplicationUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("ApplicationUserRoles");
                });

            modelBuilder.Entity("Botticelli.Server.Data.Entities.Bot.BotAdditionalInfo", b =>
                {
                    b.HasOne("Botticelli.Server.Data.Entities.Bot.BotInfo", null)
                        .WithMany("AdditionalInfo")
                        .HasForeignKey("BotInfoBotId");
                });

            modelBuilder.Entity("Botticelli.Server.Data.Entities.Bot.BotInfo", b =>
                {
                    b.Navigation("AdditionalInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
