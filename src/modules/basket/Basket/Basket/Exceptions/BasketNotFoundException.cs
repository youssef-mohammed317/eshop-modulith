namespace Basket.Basket.Exceptions;

public class BasketNotFoundException(string username) : Shared.Contracts.Exceptions.NotFoundException("Basket", username);