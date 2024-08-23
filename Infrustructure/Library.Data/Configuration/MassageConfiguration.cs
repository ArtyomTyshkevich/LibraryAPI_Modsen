using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MassageConfiguration : IEntityTypeConfiguration<Massage>
{
    public void Configure(EntityTypeBuilder<Massage> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Desription)
            .IsRequired()
            .HasMaxLength(500); 

        builder.Property(m => m.DepartureTime)
            .IsRequired();
    }
}