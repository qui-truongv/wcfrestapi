using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsSectionConfiguration : IEntityTypeConfiguration<QMS_SECTION>
{
    public void Configure(EntityTypeBuilder<QMS_SECTION> builder)
    {
        builder.ToTable("QMS_SECTION");

        // Primary Key
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.NAME)
            .HasMaxLength(150);

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

        // Indexes
        builder.HasIndex(e => e.NAME);
    }
}