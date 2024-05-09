using Microsoft.Data.SqlClient;

namespace Tutorial7.Repositories.Order;

public class OrderRepository : IOrderRepository
{
    private readonly IConfiguration _configuration;

    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int?> CheckIfOrderForProductExistsAsync(int idProduct, int specifiedAmount)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await connection.OpenAsync();

        await using var command =
            new SqlCommand("SELECT IdOrder FROM [Order] WHERE IdProduct = @id AND Amount = @amount", connection);
        command.Parameters.AddWithValue("@id", idProduct);
        command.Parameters.AddWithValue("@amount", specifiedAmount);

        var result = await command.ExecuteScalarAsync();

        return result as int?;
    }

    public async Task<bool> CheckIfOrderCreatedAtIsBeforeRequestCreatedAtAsync(int idOrder, DateTime requestDate)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await connection.OpenAsync();

        await using var command =
            new SqlCommand("SELECT CreatedAt FROM [Order] WHERE IdOrder = @id", connection);
        command.Parameters.AddWithValue("@id", idOrder);

        var result = await command.ExecuteScalarAsync();
        var orderDate = result as DateTime?;
        return orderDate < requestDate;
    }

    public async Task<bool> FulfillOrderAsync(int idOrder)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await connection.OpenAsync();
        
        await using var command = new SqlCommand(
            "UPDATE [Order] SET FulfilledAt = @date WHERE IdOrder = @id",
            connection);
        command.Parameters.AddWithValue("@date", DateTime.Now);
        command.Parameters.AddWithValue("@id", idOrder);

        var result = await command.ExecuteNonQueryAsync();

        return result > 0;
    }
}