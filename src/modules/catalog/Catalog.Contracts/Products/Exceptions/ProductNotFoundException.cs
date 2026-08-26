using Shared.Contracts.Exceptions;

namespace Catalog.Contracts.Products.Exceptions;

public class ProductNotFoundException(Guid id) : NotFoundException("Product", id);