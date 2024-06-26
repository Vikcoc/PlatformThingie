﻿// <auto-generated />
using System;
using InventoryDbComponent.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InventoryDbComponent.Migrations
{
    [DbContext(typeof(InventoryContext))]
    partial class InventoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryEntity", b =>
                {
                    b.Property<Guid>("InventoryEntityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("InventoryEntityId");

                    b.ToTable("InventoryEntities");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryEntityAttributeValue", b =>
                {
                    b.Property<Guid>("InventoryEntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("InventoryTemplateName")
                        .HasColumnType("text");

                    b.Property<long>("InventoryTemplateVersion")
                        .HasColumnType("bigint");

                    b.Property<string>("InventoryTemplateEntityAttributeName")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("InventoryEntityId", "InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName");

                    b.HasIndex("InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName");

                    b.ToTable("InventoryEntitiesAttributeValues");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryEntityHistory", b =>
                {
                    b.Property<Guid>("InventoryEntityId")
                        .HasColumnType("uuid");

                    b.Property<long>("Timestamp")
                        .HasColumnType("bigint");

                    b.Property<Guid>("AuthUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Log")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("InventoryEntityId", "Timestamp");

                    b.ToTable("InventoryEntityHistories");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplate", b =>
                {
                    b.Property<string>("InventoryTemplateName")
                        .HasColumnType("text");

                    b.Property<long>("InventoryTemplateVersion")
                        .HasColumnType("bigint");

                    b.HasKey("InventoryTemplateName", "InventoryTemplateVersion");

                    b.ToTable("InventoryTemplates");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateAttribute", b =>
                {
                    b.Property<string>("InventoryTemplateName")
                        .HasColumnType("text");

                    b.Property<long>("InventoryTemplateVersion")
                        .HasColumnType("bigint");

                    b.Property<string>("InventoryTemplateAttributeName")
                        .HasColumnType("text");

                    b.Property<string>("InventoryTemplateAttributeAction")
                        .HasColumnType("text");

                    b.Property<string>("InventoryTemplateAttributeValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateAttributeName");

                    b.ToTable("InventoryTemplateAttributes");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateAttributeRead", b =>
                {
                    b.Property<string>("InventoryTemplateName")
                        .HasColumnType("text");

                    b.Property<long>("InventoryTemplateVersion")
                        .HasColumnType("bigint");

                    b.Property<string>("InventoryTemplateAttributeName")
                        .HasColumnType("text");

                    b.Property<string>("Permission")
                        .HasColumnType("text");

                    b.HasKey("InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateAttributeName", "Permission");

                    b.ToTable("InventoryTemplateAttributeReads");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateEntityAttribute", b =>
                {
                    b.Property<string>("InventoryTemplateName")
                        .HasColumnType("text");

                    b.Property<long>("InventoryTemplateVersion")
                        .HasColumnType("bigint");

                    b.Property<string>("InventoryTemplateEntityAttributeName")
                        .HasColumnType("text");

                    b.Property<string>("InventoryTemplateEntityAttributeAction")
                        .HasColumnType("text");

                    b.HasKey("InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName");

                    b.ToTable("InventoryTemplateEntityAttributes");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateEntityAttributePermission", b =>
                {
                    b.Property<string>("InventoryTemplateName")
                        .HasColumnType("text");

                    b.Property<long>("InventoryTemplateVersion")
                        .HasColumnType("bigint");

                    b.Property<string>("InventoryTemplateEntityAttributeName")
                        .HasColumnType("text");

                    b.Property<string>("Permission")
                        .HasColumnType("text");

                    b.Property<bool>("Writeable")
                        .HasColumnType("boolean");

                    b.HasKey("InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName", "Permission");

                    b.ToTable("InventoryTemplateEntityAttributesPermissions");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryEntityAttributeValue", b =>
                {
                    b.HasOne("InventoryDbComponent.entities.InventoryEntity", "InventoryEntity")
                        .WithMany("InventoryEntityAttributeValues")
                        .HasForeignKey("InventoryEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InventoryDbComponent.entities.InventoryTemplateEntityAttribute", "InventoryTemplateEntityAttribute")
                        .WithMany("InventoryEntityAttributeValues")
                        .HasForeignKey("InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryEntity");

                    b.Navigation("InventoryTemplateEntityAttribute");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryEntityHistory", b =>
                {
                    b.HasOne("InventoryDbComponent.entities.InventoryEntity", "InventoryEntity")
                        .WithMany("InventoryEntityHistories")
                        .HasForeignKey("InventoryEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryEntity");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateAttribute", b =>
                {
                    b.HasOne("InventoryDbComponent.entities.InventoryTemplate", "InventoryTemplate")
                        .WithMany("InventoryTemplateAttributes")
                        .HasForeignKey("InventoryTemplateName", "InventoryTemplateVersion")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryTemplate");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateAttributeRead", b =>
                {
                    b.HasOne("InventoryDbComponent.entities.InventoryTemplateAttribute", "InventoryTemplateAttribute")
                        .WithMany("InventoryTemplateAttributeReads")
                        .HasForeignKey("InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateAttributeName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryTemplateAttribute");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateEntityAttribute", b =>
                {
                    b.HasOne("InventoryDbComponent.entities.InventoryTemplate", "InventoryTemplate")
                        .WithMany("InventoryTemplateEntityAttributes")
                        .HasForeignKey("InventoryTemplateName", "InventoryTemplateVersion")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryTemplate");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateEntityAttributePermission", b =>
                {
                    b.HasOne("InventoryDbComponent.entities.InventoryTemplateEntityAttribute", "InventoryTemplateEntityAttribute")
                        .WithMany("InventoryEntityAttributeValuePermissions")
                        .HasForeignKey("InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryTemplateEntityAttribute");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryEntity", b =>
                {
                    b.Navigation("InventoryEntityAttributeValues");

                    b.Navigation("InventoryEntityHistories");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplate", b =>
                {
                    b.Navigation("InventoryTemplateAttributes");

                    b.Navigation("InventoryTemplateEntityAttributes");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateAttribute", b =>
                {
                    b.Navigation("InventoryTemplateAttributeReads");
                });

            modelBuilder.Entity("InventoryDbComponent.entities.InventoryTemplateEntityAttribute", b =>
                {
                    b.Navigation("InventoryEntityAttributeValuePermissions");

                    b.Navigation("InventoryEntityAttributeValues");
                });
#pragma warning restore 612, 618
        }
    }
}
