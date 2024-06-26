﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UsersDbComponent.entities;

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

                    b.HasKey("AuthUserId");

                    b.ToTable("AuthUsers");
                });

            modelBuilder.Entity("AuthFrontend.entities.AuthUserClaim", b =>
                {
                    b.Property<Guid>("AuthUserClaimId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AuthClaimName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AuthClaimValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("AuthUserId")
                        .HasColumnType("uuid");

                    b.HasKey("AuthUserClaimId");

                    b.HasIndex("AuthClaimName");

                    b.HasIndex("AuthUserId");

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

            modelBuilder.Entity("UsersDbComponent.entities.AuthGroup", b =>
                {
                    b.Property<string>("AuthGroupName")
                        .HasColumnType("text");

                    b.HasKey("AuthGroupName");

                    b.ToTable("AuthGroups");
                });

            modelBuilder.Entity("UsersDbComponent.entities.AuthGroupPermission", b =>
                {
                    b.Property<string>("AuthGroupName")
                        .HasColumnType("text");

                    b.Property<string>("AuthPermissionName")
                        .HasColumnType("text");

                    b.HasKey("AuthGroupName", "AuthPermissionName");

                    b.HasIndex("AuthPermissionName");

                    b.ToTable("AuthGroupPermissions");
                });

            modelBuilder.Entity("UsersDbComponent.entities.AuthPermission", b =>
                {
                    b.Property<string>("AuthPermissionName")
                        .HasColumnType("text");

                    b.HasKey("AuthPermissionName");

                    b.ToTable("AuthPermissions");
                });

            modelBuilder.Entity("UsersDbComponent.entities.AuthUserGroup", b =>
                {
                    b.Property<Guid>("AuthUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("AuthGroupName")
                        .HasColumnType("text");

                    b.HasKey("AuthUserId", "AuthGroupName");

                    b.HasIndex("AuthGroupName");

                    b.ToTable("AuthUserGroups");
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

            modelBuilder.Entity("UsersDbComponent.entities.AuthGroupPermission", b =>
                {
                    b.HasOne("UsersDbComponent.entities.AuthGroup", "AuthGroup")
                        .WithMany("GroupPermissions")
                        .HasForeignKey("AuthGroupName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UsersDbComponent.entities.AuthPermission", "AuthPermission")
                        .WithMany("GroupPermissions")
                        .HasForeignKey("AuthPermissionName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuthGroup");

                    b.Navigation("AuthPermission");
                });

            modelBuilder.Entity("UsersDbComponent.entities.AuthUserGroup", b =>
                {
                    b.HasOne("UsersDbComponent.entities.AuthGroup", "AuthGroup")
                        .WithMany("UserGroups")
                        .HasForeignKey("AuthGroupName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuthFrontend.entities.AuthUser", "AuthUser")
                        .WithMany("UserGroups")
                        .HasForeignKey("AuthUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuthGroup");

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

                    b.Navigation("UserGroups");
                });

            modelBuilder.Entity("UsersDbComponent.entities.AuthGroup", b =>
                {
                    b.Navigation("GroupPermissions");

                    b.Navigation("UserGroups");
                });

            modelBuilder.Entity("UsersDbComponent.entities.AuthPermission", b =>
                {
                    b.Navigation("GroupPermissions");
                });
#pragma warning restore 612, 618
        }
    }
}
