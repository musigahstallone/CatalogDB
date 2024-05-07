using System.ComponentModel.DataAnnotations;
namespace CatalogDB.Models;

public class CatalogItem
{
    // [BsonRepresentation(BsonType.ObjectId)]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? PictureFileName { get; set; }

    public string? CatalogType { get; set; }

    public string? CatalogBrand { get; set; }
}
