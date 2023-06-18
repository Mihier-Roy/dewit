﻿// <auto-generated />
using System;
using Dewit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Dewit.Infrastructure.Migrations
{
    [DbContext(typeof(DewitDbContext))]
    [Migration("20230618074322_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("Dewit.Core.Entities.ConfigItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ConfigItems");
                });

            modelBuilder.Entity("Dewit.Core.Entities.JournalItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CalendarDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Mood")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("JournalLogs");
                });

            modelBuilder.Entity("Dewit.Core.Entities.TaskItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AddedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CompletedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("TaskDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
