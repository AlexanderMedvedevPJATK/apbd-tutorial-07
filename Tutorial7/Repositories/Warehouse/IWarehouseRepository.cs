using Tutorial7.DTOs;

namespace Tutorial7.Repositories.Warehouse;

public interface IWarehouseRepository
{
    Task<bool> CheckWarehouseExistenceAsync(int idWarehouse);
    Task<bool> CheckIfOrderWasAlreadyFulfilledAsync(int idOrder);

    Task<int?> AddProductToWarehouseAsync(AddProductToWarehouseDto addProductToWarehouseDto,
        int idOrder,
        float productPrice);
}