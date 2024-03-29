﻿// <auto-generated />
using System;
using AuthFrontend.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthFrontend.Migrations
{
    [DbContext(typeof(AuthContext))]
    partial class AuthContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AuthFrontend.entities.AuthClaim", b =>
                {
                    b.Property<string>("AuthClaimName")
                        .HasColumnType("text");

                    b.Property<int>("AuthClaimRight")
                        .HasColumnType("integer");

                    b.HasKey("AuthClaimName");

                    b.ToTable("AuthClaims");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthUser", b =>
                {
                    b.Property<Guid>("AuthUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("NewAuthUserId")
                        .HasColumnType("uuid");

                    b.HasKey("AuthUserId");

                    b.HasIndex("NewAuthUserId");

                    b.ToTable("AuthUsers");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthUserClaim", b =>
                {
                    b.Property<Guid>("AuthUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("AuthClaimName")
                        .HasColumnType("text");

                    b.Property<string>("AuthClaimValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AuthUserId", "AuthClaimName");

                    b.HasIndex("AuthClaimName");

                    b.ToTable("AuthUserClaims");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthUserRefreshToken", b =>
                {
                    b.Property<Guid>("JTI")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthUserId")
                        .HasColumnType("uuid");

                    b.Property<long>("Expire")
                        .HasColumnType("bigint");

                    b.Property<string>("HashedToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("JTI");

                    b.HasIndex("AuthUserId");

                    b.ToTable("AuthUserRefreshTokens");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthUser", b =>
                {
                    b.HasOne("AuthFrontend.entities.AuthUser", "NewAuthUser")
                        .WithMany()
                        .HasForeignKey("NewAuthUserId");

                    b.Navigation("NewAuthUser");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthUserClaim", b =>
                {
                    b.HasOne("AuthFrontend.entities.AuthClaim", "AuthClaim")
                        .WithMany("AuthUserClaims")
                        .HasForeignKey("AuthClaimName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuthFrontend.entities.AuthUser", "AuthUser")
                        .WithMany("AuthUserClaims")
                        .HasForeignKey("AuthUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuthClaim");

                    b.Navigation("AuthUser");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthUserRefreshToken", b =>
                {
                    b.HasOne("AuthFrontend.entities.AuthUser", "AuthUser")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("AuthUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuthUser");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthClaim", b =>
                {
                    b.Navigation("AuthUserClaims");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthUser", b =>
                {
                    b.Navigation("AuthUserClaims");

                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
