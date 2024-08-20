using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.HasOne(b => b.Author)
            .WithMany()
            .IsRequired();

        builder.Property(b => b.StartRentDateTime)
            .IsRequired();

        builder.Property(b => b.EndRentDateTime)
            .IsRequired();

        builder.Property(b => b.ImageFileName)
            .HasMaxLength(255);
    }
}