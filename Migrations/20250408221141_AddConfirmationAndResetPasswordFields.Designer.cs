﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Shop.Infrastructure.Data;

#nullable disable

namespace Shop.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250408221141_AddConfirmationAndResetPasswordFields")]
    partial class AddConfirmationAndResetPasswordFields
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Shop.Core.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("AddedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<bool>("IsAvailable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("PublisherId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PublisherId");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("Shop.Core.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConfirmationToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ConfirmationTokenExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("IsActivated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ResetPasswordToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ResetPasswordTokenExpireAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("Customer");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Shop.Core.Entities.Product", b =>
                {
                    b.HasOne("Shop.Core.Entities.User", "Publisher")
                        .WithMany()
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Publisher");
                });
#pragma warning restore 612, 618
        }
    }
}
