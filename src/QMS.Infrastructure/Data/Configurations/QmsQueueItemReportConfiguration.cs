using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsQueueItemReportConfiguration : IEntityTypeConfiguration<QMS_QUEUE_ITEM_REPORT>
{
    public void Configure(EntityTypeBuilder<QMS_QUEUE_ITEM_REPORT> builder)
    {
        builder.ToTable("QMS_QUEUE_ITEM_REPORT");

        // Primary Key
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID)
            .ValueGeneratedNever(); // Not IDENTITY in schema

        // Properties
        builder.Property(e => e.QUEUE_ID);

        builder.Property(e => e.SEQUENCE);

        builder.Property(e => e.PREFIX)
            .HasMaxLength(10);

        builder.Property(e => e.DISPLAYTEXT)
            .HasMaxLength(50);

        builder.Property(e => e.SUFFIX)
            .HasMaxLength(10);

        builder.Property(e => e.ORDER)
            .HasMaxLength(50);

        builder.Property(e => e.STATE);

        builder.Property(e => e.PRIORITY);

        builder.Property(e => e.PREVIOUS);

        builder.Property(e => e.PREVIOUSDEPT_ID);

        builder.Property(e => e.PATIENTCODE)
            .HasMaxLength(50);

        builder.Property(e => e.PATIENTNAME)
            .HasMaxLength(150);

        builder.Property(e => e.PATIENTYOB);

        builder.Property(e => e.CREATEDATE)
            .HasColumnType("date");

        builder.Property(e => e.CREATETIME);

        builder.Property(e => e.ESTIMATETIME);

        builder.Property(e => e.PROCESSTIME);

        builder.Property(e => e.FINISHTIME);

        builder.Property(e => e.CLIENT_ID);

        builder.Property(e => e.CLIENT_NAME)
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

        builder.Property(e => e.ISMATOATHUOC);

        builder.Property(e => e.SOTIEN)
            .HasColumnType("decimal(24, 6)");

        builder.Property(e => e.TENCUA)
            .HasMaxLength(150);

        // Indexes
        builder.HasIndex(e => e.QUEUE_ID);
        builder.HasIndex(e => e.CREATEDATE);
    }
}