using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Models;

namespace Play.Catalog.Service.Extentions
{
    public static class MappingItemExtention
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}
