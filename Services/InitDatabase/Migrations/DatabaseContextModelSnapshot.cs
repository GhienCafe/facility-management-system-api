﻿// <auto-generated />
using System;
using MainData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InitDatabase.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MainData.Entities.Asset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AssetCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AssetName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsMovable")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastMaintenanceTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ManufacturingYear")
                        .HasColumnType("datetime2");

                    b.Property<double>("Quantity")
                        .HasColumnType("float");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid>("TypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AssetCode")
                        .IsUnique()
                        .HasFilter("[AssetCode] IS NOT NULL");

                    b.HasIndex("TypeId");

                    b.ToTable("Assets", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.AssetType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TypeCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Unit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TypeCode")
                        .IsUnique();

                    b.ToTable("AssetTypes", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Building", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BuildingCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BuildingName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CampusId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CampusId");

                    b.ToTable("Buildings", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Campus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CampusCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CampusName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Telephone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Campuses", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Floor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BuildingId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FloorMap")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FloorNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.ToTable("Floors", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Maintenance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssignedTo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CompletionDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("RequestedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Maintenances", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.MaintenanceDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDone")
                        .HasColumnType("bit");

                    b.Property<Guid>("MaintenanceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("MaintenanceId");

                    b.ToTable("MaintenanceDetails", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.MaintenanceScheduleConfig", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssetTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Period")
                        .HasColumnType("int");

                    b.Property<DateTime>("SpecificDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TimeUnit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssetTypeId");

                    b.ToTable("MaintenanceScheduleConfigs", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.MediaFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Extensions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FileType")
                        .HasColumnType("int");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RawUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MediaFiles", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsSendAll")
                        .HasColumnType("bit");

                    b.Property<string>("ShortContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Replacement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssignedTo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CompletionDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RequestedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Replacements", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.ReplacementDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDone")
                        .HasColumnType("bit");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ReplacementAssetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ReplacementId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("ReplacementId");

                    b.ToTable("ReplacementDetails", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Room", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Area")
                        .HasColumnType("float");

                    b.Property<int?>("Capacity")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FloorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PathRoom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoomCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("RoomType")
                        .HasColumnType("int");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("FloorId");

                    b.HasIndex("RoomCode")
                        .IsUnique();

                    b.HasIndex("StatusId");

                    b.ToTable("Rooms", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.RoomAsset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("FromDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ToDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomAssets", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.RoomStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RoomStatus", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Token", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("AccessExpiredAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("RefreshExpiredAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tokens", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DepartmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Dob")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FirstLoginAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Gender")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PersonalIdentifyNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserCode")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("MainData.Entities.Asset", b =>
                {
                    b.HasOne("MainData.Entities.AssetType", "Type")
                        .WithMany("Assets")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type");
                });

            modelBuilder.Entity("MainData.Entities.Building", b =>
                {
                    b.HasOne("MainData.Entities.Campus", "Campus")
                        .WithMany("Buildings")
                        .HasForeignKey("CampusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campus");
                });

            modelBuilder.Entity("MainData.Entities.Floor", b =>
                {
                    b.HasOne("MainData.Entities.Building", "Buildings")
                        .WithMany("Floors")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Buildings");
                });

            modelBuilder.Entity("MainData.Entities.Maintenance", b =>
                {
                    b.HasOne("MainData.Entities.User", "User")
                        .WithMany("Maintenances")
                        .HasForeignKey("CreatorId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MainData.Entities.MaintenanceDetail", b =>
                {
                    b.HasOne("MainData.Entities.Asset", "Asset")
                        .WithMany("MaintenanceDetails")
                        .HasForeignKey("AssetId");

                    b.HasOne("MainData.Entities.Maintenance", "Maintenance")
                        .WithMany("MaintenanceDetails")
                        .HasForeignKey("MaintenanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");

                    b.Navigation("Maintenance");
                });

            modelBuilder.Entity("MainData.Entities.MaintenanceScheduleConfig", b =>
                {
                    b.HasOne("MainData.Entities.AssetType", "AssetType")
                        .WithMany("MaintenanceScheduleConfigs")
                        .HasForeignKey("AssetTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssetType");
                });

            modelBuilder.Entity("MainData.Entities.Notification", b =>
                {
                    b.HasOne("MainData.Entities.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MainData.Entities.Replacement", b =>
                {
                    b.HasOne("MainData.Entities.User", "User")
                        .WithMany("Replacements")
                        .HasForeignKey("CreatorId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MainData.Entities.ReplacementDetail", b =>
                {
                    b.HasOne("MainData.Entities.Asset", "Asset")
                        .WithMany("ReplacementDetails")
                        .HasForeignKey("AssetId");

                    b.HasOne("MainData.Entities.Replacement", "Replacement")
                        .WithMany("ReplacementDetails")
                        .HasForeignKey("ReplacementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");

                    b.Navigation("Replacement");
                });

            modelBuilder.Entity("MainData.Entities.Room", b =>
                {
                    b.HasOne("MainData.Entities.Floor", "Floors")
                        .WithMany("Rooms")
                        .HasForeignKey("FloorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MainData.Entities.RoomStatus", "Status")
                        .WithMany("Rooms")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Floors");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("MainData.Entities.RoomAsset", b =>
                {
                    b.HasOne("MainData.Entities.Asset", "Asset")
                        .WithMany("RoomAssets")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MainData.Entities.Room", "Room")
                        .WithMany("RoomAssets")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("MainData.Entities.Token", b =>
                {
                    b.HasOne("MainData.Entities.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MainData.Entities.Asset", b =>
                {
                    b.Navigation("MaintenanceDetails");

                    b.Navigation("ReplacementDetails");

                    b.Navigation("RoomAssets");
                });

            modelBuilder.Entity("MainData.Entities.AssetType", b =>
                {
                    b.Navigation("Assets");

                    b.Navigation("MaintenanceScheduleConfigs");
                });

            modelBuilder.Entity("MainData.Entities.Building", b =>
                {
                    b.Navigation("Floors");
                });

            modelBuilder.Entity("MainData.Entities.Campus", b =>
                {
                    b.Navigation("Buildings");
                });

            modelBuilder.Entity("MainData.Entities.Floor", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("MainData.Entities.Maintenance", b =>
                {
                    b.Navigation("MaintenanceDetails");
                });

            modelBuilder.Entity("MainData.Entities.Replacement", b =>
                {
                    b.Navigation("ReplacementDetails");
                });

            modelBuilder.Entity("MainData.Entities.Room", b =>
                {
                    b.Navigation("RoomAssets");
                });

            modelBuilder.Entity("MainData.Entities.RoomStatus", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("MainData.Entities.User", b =>
                {
                    b.Navigation("Maintenances");

                    b.Navigation("Notifications");

                    b.Navigation("Replacements");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
