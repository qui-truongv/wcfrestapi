using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsQueueConfiguration : IEntityTypeConfiguration<QMS_QUEUE>
{
    public void Configure(EntityTypeBuilder<QMS_QUEUE> builder)
    {
        builder.ToTable("QMS_QUEUE");

        // No explicit Primary Key defined in schema - using ID
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.NAME)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.STATE);

        builder.Property(e => e.SCREEN_ID);

        builder.Property(e => e.DEPARTMENT_ID);

        builder.Property(e => e.MAX);

        builder.Property(e => e.QUEUETYPE_ID);

        builder.Property(e => e.SECTION_ID);

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

        builder.Property(e => e.TENBACSI)
            .HasMaxLength(254);

        builder.Property(e => e.TENDIEUDUONG)
            .HasMaxLength(254);

        builder.Property(e => e.IS_MANUAL)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.IDX);

        builder.Property(e => e.CODESCREEN)
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(e => e.Screen)
            .WithMany(s => s.Queues)
            .HasForeignKey(e => e.SCREEN_ID)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Section)
            .WithMany(s => s.Queues)
            .HasForeignKey(e => e.SECTION_ID)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.QueueType)
            .WithMany(qt => qt.Queues)
            .HasForeignKey(e => e.QUEUETYPE_ID)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.NAME);
        builder.HasIndex(e => e.SCREEN_ID);
        builder.HasIndex(e => e.SECTION_ID);
        builder.HasIndex(e => e.QUEUETYPE_ID);
    }
}