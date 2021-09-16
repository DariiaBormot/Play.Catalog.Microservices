using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extentions;
using Play.Catalog.Service.Models;
using Play.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";

        private readonly IRepository<Item> _itemsRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public ItemsController(IRepository<Item> itemRepository, IPublishEndpoint publishEndpoint)
        {
            _itemsRepository = itemRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Authorize(Policies.Read)]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync()
        {
            var items = (await _itemsRepository.GetAllAsync())
                .Select(item => item.AsDto());

            return Ok(items);
        }

        [HttpGet("{id}")]
        [Authorize(Policies.Read)]
        public async Task<ActionResult<ItemDto>> GetItemByIdAsync(Guid id)
        {
            var item = await _itemsRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return item.AsDto();
        }

        [HttpPost]
        [Authorize(Policies.Write)]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _itemsRepository.CreateAsync(item);

            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            if (item == null)
                return NotFound();

            return CreatedAtAction(nameof(GetItemByIdAsync), new { item.Id }, item);
        }

        [HttpPut("{id}")]
        [Authorize(Policies.Write)]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _itemsRepository.GetByIdAsync(id);

            if (existingItem == null)
                return NotFound();

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await _itemsRepository.UpdateAsync(existingItem);

            await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policies.Write)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var item = await _itemsRepository.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            await _itemsRepository.RemoveAsync(item.Id);

            await _publishEndpoint.Publish(new CatalogItemDeleted(id));

            return Ok();
        }

    }
}
