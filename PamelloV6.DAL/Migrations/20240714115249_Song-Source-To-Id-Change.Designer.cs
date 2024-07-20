﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PamelloV6.DAL;

#nullable disable

namespace PamelloV6.DAL.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240714115249_Song-Source-To-Id-Change")]
    partial class SongSourceToIdChange
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("PamelloV6.DAL.Entity.EpisodeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Skip")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SongId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Start")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SongId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("PamelloV6.DAL.Entity.PlaylistEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsProtected")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("PamelloV6.DAL.Entity.SongEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CoverUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDownloaded")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("YoutubeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("PamelloV6.DAL.Entity.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("DiscordId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAdministrator")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Token")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PlaylistEntitySongEntity", b =>
                {
                    b.Property<int>("PlaylistsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SongsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlaylistsId", "SongsId");

                    b.HasIndex("SongsId");

                    b.ToTable("PlaylistEntitySongEntity");
                });

            modelBuilder.Entity("PamelloV6.DAL.Entity.EpisodeEntity", b =>
                {
                    b.HasOne("PamelloV6.DAL.Entity.SongEntity", "Song")
                        .WithMany("Episodes")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                });

            modelBuilder.Entity("PamelloV6.DAL.Entity.PlaylistEntity", b =>
                {
                    b.HasOne("PamelloV6.DAL.Entity.UserEntity", "Owner")
                        .WithMany("OwnedPlaylists")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PlaylistEntitySongEntity", b =>
                {
                    b.HasOne("PamelloV6.DAL.Entity.PlaylistEntity", null)
                        .WithMany()
                        .HasForeignKey("PlaylistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PamelloV6.DAL.Entity.SongEntity", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PamelloV6.DAL.Entity.SongEntity", b =>
                {
                    b.Navigation("Episodes");
                });

            modelBuilder.Entity("PamelloV6.DAL.Entity.UserEntity", b =>
                {
                    b.Navigation("OwnedPlaylists");
                });
#pragma warning restore 612, 618
        }
    }
}