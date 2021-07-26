using Microsoft.AspNetCore.Mvc;
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
        private readonly IRepository<Item> _ItemsRepository;

        public ItemsController(IRepository<Item> itemRepository)
        {
            _ItemsRepository = itemRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await _ItemsRepository.GetAllAsync())
                .Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemByIdAsync(Guid id)
        {
            var item = await _ItemsRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _ItemsRepository.CreateAsync(item);

            if (item == null)
                return NotFound();

            return CreatedAtAction(nameof(GetItemByIdAsync), new { item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _ItemsRepository.GetByIdAsync(id);

            if (existingItem == null)
                return NotFound();

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await _ItemsRepository.UpdateAsync(existingItem);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var item = await _ItemsRepository.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            await _ItemsRepository.RemoveAsync(item.Id);

            return Ok();
        }

    }
}
