﻿// <auto-generated />
using System;
using CodeExecutor.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CodeExecutor.DB.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CodeExecutor.DB.Models.CodeExecution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<DateTimeOffset?>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("InitiatorId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsError")
                        .HasColumnType("boolean");

                    b.Property<long>("LanguageId")
                        .HasColumnType("bigint");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("RequestedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SecretKey")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<DateTimeOffset?>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.ToTable("CodeExecutions");
                });

            modelBuilder.Entity("CodeExecutor.DB.Models.CodeExecutionResult", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ExecutionResults");
                });

            modelBuilder.Entity("CodeExecutor.DB.Models.Language", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Version")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.HasKey("Id");

                    b.ToTable("Languages");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Name = "CSharp",
                            Version = "12"
                        },
                        new
                        {
                            Id = 2L,
                            Name = "Python",
                            Version = "10"
                        });
                });

            modelBuilder.Entity("CodeExecutor.DB.Models.SourceCode", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("CodeText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SourceCodes");
                });

            modelBuilder.Entity("CodeExecutor.DB.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("DeactivationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<bool>("IsDeactivated")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSuperUser")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("bytea");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.HasIndex("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CodeExecutor.DB.Models.CodeExecution", b =>
                {
                    b.HasOne("CodeExecutor.DB.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Language");
                });

            modelBuilder.Entity("CodeExecutor.DB.Models.CodeExecutionResult", b =>
                {
                    b.HasOne("CodeExecutor.DB.Models.CodeExecution", "CodeExecution")
                        .WithOne("Result")
                        .HasForeignKey("CodeExecutor.DB.Models.CodeExecutionResult", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("CodeExecution");
                });

            modelBuilder.Entity("CodeExecutor.DB.Models.SourceCode", b =>
                {
                    b.HasOne("CodeExecutor.DB.Models.CodeExecution", "CodeExecution")
                        .WithOne("SourceCode")
                        .HasForeignKey("CodeExecutor.DB.Models.SourceCode", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("CodeExecution");
                });

            modelBuilder.Entity("CodeExecutor.DB.Models.CodeExecution", b =>
                {
                    b.Navigation("Result");

                    b.Navigation("SourceCode");
                });
#pragma warning restore 612, 618
        }
    }
}