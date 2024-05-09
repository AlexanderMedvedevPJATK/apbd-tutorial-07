using Microsoft.Data.SqlClient;
using Tutorial7.DTOs;

namespace Tutorial7.Repositories.Warehouse;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> CheckWarehouseExistenceAsync(int idWarehouse)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await connection.OpenAsync();

        await using var command = 
            new SqlCommand("SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @id", connection);
        
        command.Parameters.AddWithValue("@id", idWarehouse);

        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }

    public async Task<bool> CheckIfOrderWasAlreadyFulfilledAsync(int idOrder)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await connection.OpenAsync();
        
        await using var command = 
            new SqlCommand("SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @id", connection);
        command.Parameters.AddWithValue("@id", idOrder);
        
        var result = await command.ExecuteScalarAsync();
        
        return result as int? > 0;
    }
    
    public async Task<int?> AddProductToWarehouseAsync(AddProductToWarehouseDto addProductToWarehouseDto, 
                                                       int idOrder, 
                                                       float productPrice)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await connection.OpenAsync();

        await using var command =
            new SqlCommand(
                @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                          OUTPUT INSERTED.IdProductWarehouse
                          VALUES (@idWarehouse, @idProduct, @idOrder, @amount, @price, @createdAt)",
                connection);
        command.Parameters.AddWithValue("@idWarehouse", addProductToWarehouseDto.IdWarehouse);
        command.Parameters.AddWithValue("@idProduct", addProductToWarehouseDto.IdProduct);
        command.Parameters.AddWithValue("@idOrder", idOrder);
        command.Parameters.AddWithValue("@amount", addProductToWarehouseDto.Amount);
        command.Parameters.AddWithValue("@price", productPrice);
        command.Parameters.AddWithValue("@createdAt", DateTime.Now);
        
        var result = await command.ExecuteScalarAsync();

        return result as int?;
    }
}