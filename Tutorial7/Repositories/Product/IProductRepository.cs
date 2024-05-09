namespace Tutorial7.Repositories;

public interface IProductRepository
{ 
    Task<bool> CheckProductExistenceAsync(int idProduct);
    Task<int?> GetProductPriceAsync(int idProduct);
}