namespace Tutorial7.Repositories.Order;

public interface IOrderRepository
{
    Task<int?> CheckIfOrderForProductExistsAsync(int idProduct, int specifiedAmount);
    Task<bool> CheckIfOrderCreatedAtIsBeforeRequestCreatedAtAsync(int idOrder, DateTime requestDate);
    Task<bool> FulfillOrderAsync(int idOrder);
}