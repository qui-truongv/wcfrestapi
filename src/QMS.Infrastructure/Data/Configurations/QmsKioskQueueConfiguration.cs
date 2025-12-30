using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsKioskQueueConfiguration : IEntityTypeConfiguration<QMS_KIOSK_QUEUE>
{
    public void Configure(EntityTypeBuilder<QMS_KIOSK_QUEUE> builder)
    {
        builder.ToTable("QMS_KIOSK_QUEUE");

        // Primary Key
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.KIOSK_ID)
            .IsRequired();

        builder.Property(e => e.QUEUE_ID);

        builder.Property(e => e.DISPLAYTEXT)
            .HasMaxLength(150);

        builder.Property(e => e.PRIORITY);

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

        // Relationships
        builder.HasOne(e => e.Kiosk)
            .WithMany(k => k.KioskQueues)
            .HasForeignKey(e => e.KIOSK_ID)
            .HasPrincipalKey(k => k.ID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Queue)
            .WithMany(q => q.KioskQueues)
            .HasForeignKey(e => e.QUEUE_ID)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.KIOSK_ID);
        builder.HasIndex(e => e.QUEUE_ID);
    }
}