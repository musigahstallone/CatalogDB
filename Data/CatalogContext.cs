using CatalogDB.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace CatalogDB.Data;

public class CatalogContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<CatalogItem> Products { get; init; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<CatalogItem>(b =>
        {
            b.ToCollection("Products");
            b.Property(c => c.Id).HasElementName("_id");
            b.Property(c => c.CatalogBrand).HasElementName("Brand");
            b.Property(c => c.CatalogType).HasElementName("Type");
        }
        );

    }
}

