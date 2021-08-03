using Play.Inventory.Service.DtoModels;
using Play.Inventory.Service.Models;

namespace Play.Inventory.Service.Extensions
{
    public static class InventoryMappingExtension
    {
        public static InventoryItemDto ToDto(this InventoryItem item, string name, string description)
        {
            return new InventoryItemDto(item.CatalogItemId, name, description, item.Quantity, item.AcquiresDate);
        }
    }
}
