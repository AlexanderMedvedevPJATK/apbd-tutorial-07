using Microsoft.AspNetCore.Mvc;
using Tutorial7.DTOs;
using Tutorial7.Repositories.Order;
using Tutorial7.Repositories.Product;
using Tutorial7.Repositories.Warehouse;

namespace Tutorial7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IOrderRepository _orderRepository;
        
        public WarehouseController(IProductRepository productRepository,
                                   IWarehouseRepository warehouseRepository,
                                   IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public IActionResult AddProductToWarehouse(int idProduct, int idWarehouse, int amount)
        {
            var dto = new AddProductToWarehouseDto
            {
                IdProduct = idProduct,
                IdWarehouse = idWarehouse,
                Amount = amount
            };
            
            var validationResult = ValidateProductAndWarehouse(dto);
            if (validationResult != null)
            {
                return validationResult;
            }
            
            var idOrder = _orderRepository.CheckIfOrderForProductExistsAsync(idProduct, amount).Result;
            if (idOrder == null)
            {
                return BadRequest("Order does not exist");
            }
            
            var orderValidation = ValidateOrder((int) idOrder);
            if (orderValidation != null)
            {
                return orderValidation;
            }

            if (!_orderRepository.FulfillOrderAsync((int) idOrder).Result)
            {
                return BadRequest("Unable to fulfill order");
            }
            
            var productPrice = _productRepository.GetProductPriceAsync(idProduct).Result;
            if (productPrice == null)
            {
                return BadRequest("Product price not found");
            }
            
            var id = _warehouseRepository.AddProductToWarehouseAsync(dto,
                                                                        (int) idOrder,
                                                                        (float) productPrice).Result;

            Console.WriteLine($"Added product to warehouse with ID: {id}");

            return Ok("Added");
        }
        
        private IActionResult? ValidateProductAndWarehouse(AddProductToWarehouseDto dto)
        {
            var productValidation = ValidateRequestedProduct(dto.IdProduct);
            if (productValidation != null)
            {
                return productValidation;
            }

            var warehouseValidation = ValidateRequestedWarehouse(dto.IdWarehouse);
            if (warehouseValidation != null)
            {
                return warehouseValidation;
            }
            
            if (dto.Amount <= 0)
            {
                return BadRequest("Invalid amount");
            }

            return null;
        }

        private IActionResult? ValidateRequestedProduct(int idProduct)
        {
            if (idProduct <= 0)
            {
                return BadRequest("Invalid product ID");
            }

            if (!_productRepository.CheckProductExistenceAsync(idProduct).Result)
            {
                return BadRequest("Product does not exist");
            }

            return null;
        }

        private IActionResult? ValidateRequestedWarehouse(int idWarehouse)
        {
            if (idWarehouse <= 0)
            {
                return BadRequest("Invalid warehouse ID");
            }

            if (!_warehouseRepository.CheckWarehouseExistenceAsync(idWarehouse).Result)
            {
                return BadRequest("Warehouse does not exist");
            }

            return null;
        }
        
        private IActionResult? ValidateOrder(int idOrder)
        {
            if (!_orderRepository.CheckIfOrderCreatedAtIsBeforeRequestCreatedAtAsync(idOrder, DateTime.Now).Result)
            {
                return BadRequest("Order was created after request");
            }
            
            if(_warehouseRepository.CheckIfOrderWasAlreadyFulfilledAsync(idOrder).Result)
            {
                return BadRequest("Order was already fulfilled");
            }

            return null;
        }
    }
}
