namespace Tutorial7.Repositories.Product;

public interface IProductRepository
{ 
    Task<bool> CheckProductExistenceAsync(int idProduct);
    Task<float?> GetProductPriceAsync(int idProduct);
}