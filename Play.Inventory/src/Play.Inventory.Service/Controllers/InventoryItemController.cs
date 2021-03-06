using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common.Contracts;
using Play.Inventory.Service.DtoModels;
using Play.Inventory.Service.Extensions;
using Play.Inventory.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryItemController : ControllerBase
    {
        private const string AdminRole = "Admin";

        private readonly IRepository<InventoryItem> _inventoryRepository;
        private readonly IRepository<CatalogItem> _catalogRepository;

        public InventoryItemController(IRepository<InventoryItem> inventoryRepository, IRepository<CatalogItem> catalogRepository)
        {
            _inventoryRepository = inventoryRepository;
            _catalogRepository = catalogRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                return BadRequest();
            }

            var currentUserId = User.FindFirstValue("sub");
            if (Guid.Parse(currentUserId) != userId)
            {
                if (!User.IsInRole(AdminRole))
                {
                    return Unauthorized();
                }
            }

            var inventoryItems = await _inventoryRepository.FilterAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItems.Select(item => item.CatalogItemId);
            var catalogItems = await _catalogRepository.FilterAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItems.Select(item =>
            {
                var catalogItem = catalogItems.Single(catItem => catItem.Id == item.CatalogItemId);
                return item.ToDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [Authorize(Roles = AdminRole)]
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
