using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsQueueGroupConfiguration : IEntityTypeConfiguration<QMS_QUEUE_GROUP>
{
    public void Configure(EntityTypeBuilder<QMS_QUEUE_GROUP> builder)
    {
        builder.ToTable("QMS_QUEUE_GROUP");

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

        builder.Property(e => e.NGAYTAO);

        builder.Property(e => e.NGUOITAO_ID);

        builder.Property(e => e.NGAYCAPNHAT);

        builder.Property(e => e.NGUOICAPNHAT_ID);

        builder.Property(e => e.CODE)
            .HasMaxLength(10)
            .IsUnicode(false);

        // Indexes
        builder.HasIndex(e => e.CODE);
        builder.HasIndex(e => e.NAME);
    }
}