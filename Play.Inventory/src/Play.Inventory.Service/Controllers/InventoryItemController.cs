using Microsoft.AspNetCore.Mvc;
using Play.Common.Contracts;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.DtoModels;
using Play.Inventory.Service.Extensions;
using Play.Inventory.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryItemController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryRepository;
        private readonly CatalogClient _catalogClient;
        public InventoryItemController(IRepository<InventoryItem> inventoryRepository, CatalogClient catalogClient)
        {
            _inventoryRepository = inventoryRepository;
            _catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await _catalogClient.GetCatalogItemsAsync();
            var inventoryItems = await _inventoryRepository.FilterAllAsync(item => item.UserId == userId);

            var inventoryItemDtos = inventoryItems.Select(item =>
            {
                var catalogItem = catalogItems.Single(catItem => catItem.Id == item.CatalogItemId);
                return item.ToDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrandItemsDto grantItemsDto)
        {
            var inventoryItem = await _inventoryRepository.FilterOneAsync(item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem is null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiresDate = DateTimeOffset.UtcNow
                };

                await _inventoryRepository.CreateAsync(inventoryItem);
            } else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await _inventoryRepository.UpdateAsync(inventoryItem);
            }

            return Ok();

        }
    }
}
