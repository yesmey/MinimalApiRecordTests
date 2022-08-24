using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApiRecordTests.Data.Model;

namespace MinimalApiRecordTests.Data.Configuration;

public class UserTypeConfiguration : IEntityTypeConfiguration<UserData>
{
    public void Configure(EntityTypeBuilder<UserData> builder)
    {
        builder.HasKey(p => p.Id);
        builder
            .Property(b => b.FirstName)
            .IsUnicode(false)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(b => b.LastName)
            .IsUnicode(false)
            .HasMaxLength(100)
            .IsRequired();
    }
}
