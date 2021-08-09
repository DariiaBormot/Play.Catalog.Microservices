using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Identity.Service.Models
{
    public record UserDto(Guid Id, string UserName, string Email, decimal Gold, DateTimeOffset CreatedDate);
    public record UpdateUserDto([Required][EmailAddress]string Email, [Range(0, 1000000)]decimal Gold);
}
