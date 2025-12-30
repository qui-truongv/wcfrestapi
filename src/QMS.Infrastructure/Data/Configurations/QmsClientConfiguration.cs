using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QMS.Core.Entities;

namespace QMS.Infrastructure.Data.Configurations;

public class QmsClientConfiguration : IEntityTypeConfiguration<QMS_CLIENT>
{
    public void Configure(EntityTypeBuilder<QMS_CLIENT> builder)
    {
        builder.ToTable("QMS_CLIENT");

        // Primary Key
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.NAME)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.COMPUTERNAME)
            .HasMaxLength(50);

        builder.Property(e => e.IPADDRESS)
            .HasMaxLength(50);

        builder.Property(e => e.STATE);

        builder.Property(e => e.PROCESSDURATION);

        builder.Property(e => e.QUEUE_ID);

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

        builder.Property(e => e.IPSERVER)
            .HasMaxLength(50);

        builder.Property(e => e.NAMEAUDIO)
            .HasMaxLength(250)
            .IsUnicode(false);

        // Relationships
        builder.HasOne(e => e.Queue)
            .WithMany(q => q.Clients)
            .HasForeignKey(e => e.QUEUE_ID)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.NAME);
        builder.HasIndex(e => e.IPADDRESS);
    }
}