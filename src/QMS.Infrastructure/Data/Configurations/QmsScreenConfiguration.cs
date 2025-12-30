using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsScreenConfiguration : IEntityTypeConfiguration<QMS_SCREEN>
{
    public void Configure(EntityTypeBuilder<QMS_SCREEN> builder)
    {
        builder.ToTable("QMS_SCREEN");

        // Primary Key
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.CODE)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.NAME)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.SECTION_ID);

        builder.Property(e => e.STATE);

        builder.Property(e => e.DISPLAYROWS);

        builder.Property(e => e.URL)
            .HasMaxLength(255);

        builder.Property(e => e.REMARKS)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.BENHVIEN_ID)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(e => e.NGAYTAO)
            .IsRequired();

        builder.Property(e => e.NGUOITAO_ID);

        builder.Property(e => e.NGAYCAPNHAT);

        builder.Property(e => e.NGUOICAPNHAT_ID);

        builder.Property(e => e.NUMSCREEN)
            .IsRequired()
            .HasDefaultValue(1);

        // Relationships
        builder.HasOne(e => e.Section)
            .WithMany(s => s.Screens)
            .HasForeignKey(e => e.SECTION_ID)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.CODE);
        builder.HasIndex(e => e.NAME);
        builder.HasIndex(e => e.SECTION_ID);
    }
}