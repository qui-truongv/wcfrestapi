using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsParameterConfiguration : IEntityTypeConfiguration<QMS_PARAMETER>
{
    public void Configure(EntityTypeBuilder<QMS_PARAMETER> builder)
    {
        builder.ToTable("QMS_PARAMETER");

        // Primary Key - CODE is the PK
        builder.HasKey(e => e.CODE);

        builder.Property(e => e.CODE)
            .IsRequired()
            .HasMaxLength(50);

        // Properties
        builder.Property(e => e.ID)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.NAME)
            .HasMaxLength(150);

        builder.Property(e => e.VALUE)
            .HasMaxLength(255);

        builder.Property(e => e.REMARKS)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.BENHVIEN_ID)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(e => e.NGAYTAO);

        builder.Property(e => e.NGUOITAO_ID);

        builder.Property(e => e.NGAYCAPNHAT);

        builder.Property(e => e.NGUOICAPNHAT_ID);

        // Indexes
        builder.HasIndex(e => e.ID);
        builder.HasIndex(e => e.NAME);
    }
}