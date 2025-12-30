using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsQueueTypeConfiguration : IEntityTypeConfiguration<QMS_QUEUE_TYPE>
{
    public void Configure(EntityTypeBuilder<QMS_QUEUE_TYPE> builder)
    {
        builder.ToTable("QMS_QUEUE_TYPE");

        // Primary Key
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.NAME)
            .HasMaxLength(150);

        builder.Property(e => e.REMARKS)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.QUEUEGROUP_ID)
            .IsRequired();

        builder.Property(e => e.BENHVIEN_ID)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(e => e.NGAYTAO)
            .IsRequired();

        builder.Property(e => e.NGUOITAO_ID);

        builder.Property(e => e.NGAYCAPNHAT);

        builder.Property(e => e.NGUOICAPNHAT_ID);

        builder.Property(e => e.QUEUE_TYPE_CODE)
            .HasMaxLength(10)
            .IsFixedLength()
            .IsUnicode(false);
        // Relationships
        builder.HasOne(e => e.QueueGroup)
            .WithMany(qg => qg.QueueTypes)
            .HasForeignKey(e => e.QUEUEGROUP_ID)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.NAME);
        builder.HasIndex(e => e.QUEUEGROUP_ID);
        builder.HasIndex(e => e.QUEUE_TYPE_CODE);
    }
}