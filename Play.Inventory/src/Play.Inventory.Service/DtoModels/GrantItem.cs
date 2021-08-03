using System;

namespace Play.Inventory.Service.DtoModels
{
    public record GrandItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId,string Name, string Description, int Quantiry, DateTimeOffset AcquiredDate);
    public record CatalogItemDto(Guid Id, string Name, string Description);
}
