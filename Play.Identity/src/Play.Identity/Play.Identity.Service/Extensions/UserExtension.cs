using Play.Identity.Service.Entities;
using Play.Identity.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Identity.Service.Extensions
{
    public static class UserExtension
    {
        public static UserDto AsDto(this ApplicationUser applicationUser)
        {
            return new UserDto(applicationUser.Id, 
                applicationUser.UserName, 
                applicationUser.Email, 
                applicationUser.Gold, 
                applicationUser.CreatedOn);
        }
    }
}
