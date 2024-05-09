using Tutorial7.DTOs;

namespace Tutorial7.Repositories;

public interface IWarehouseRepository
{
    Task<bool> CheckWarehouseExistenceAsync(int idWarehouse);
    Task<int?> AddProductToWarehouseAsync(AddProductToWarehouseDto addProductToWarehouseDto,
                                          int idOrder,
                                          int productPrice);
    Task<bool> CheckIfOrderWasAlreadyFulfilledAsync(int idOrder);
}