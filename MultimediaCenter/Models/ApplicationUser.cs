using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Favourites> Favourites { get; set; }
        public List<Order> Orders { get; set; }
    }
}
