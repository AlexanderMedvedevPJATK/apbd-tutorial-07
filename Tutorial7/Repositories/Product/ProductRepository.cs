using System.Globalization;
using Microsoft.Data.SqlClient;

namespace Tutorial7.Repositories.Product;

public class ProductRepository : IProductRepository
{
    private readonly IConfiguration _configuration;

    public ProductRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> CheckProductExistenceAsync(int idProduct)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await connection.OpenAsync();
        
        await using var command = new SqlCommand("SELECT COUNT(*) FROM Product WHERE IdProduct = @id", connection);
        command.Parameters.AddWithValue("@id", idProduct);

        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }
    
    public async Task<float?> GetProductPriceAsync(int idProduct)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await connection.OpenAsync();
        
        await using var command = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @id", connection);
        command.Parameters.AddWithValue("@id", idProduct);

        var result = await command.ExecuteScalarAsync();
        if (result is null) return null;
        
        var resultString = result.ToString();
        resultString = resultString.Replace(",", ".");
        
        var resultValue = float.Parse(resultString, CultureInfo.InvariantCulture);

        return resultValue;
    }
}