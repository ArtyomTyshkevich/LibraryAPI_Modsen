using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasIndex(b => b.ISBN)
             .IsUnique();

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(b => b.StartRentDateTime)
            .IsRequired(false);

        builder.Property(b => b.EndRentDateTime)
            .IsRequired(false);

        builder.Property(b => b.ImageFileName)
            .HasMaxLength(255);
    }
}