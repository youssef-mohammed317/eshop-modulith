namespace Catalog.Contracts.Products.Dtos;

// 1. For returning data to the client (Querying)
public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string ImageFile,
    List<string> Category,
    decimal Price
);
