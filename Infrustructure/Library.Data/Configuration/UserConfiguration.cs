using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.MiddleName)
            .HasMaxLength(100);

        builder.HasMany(u => u.Books)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(200);

        builder.Property(u => u.RefreshTokenExpiryTime)
            .IsRequired();
    }
}