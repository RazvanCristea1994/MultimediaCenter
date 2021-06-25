using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.ViewModel
{
    public class FavouritesForUserResponse 
    {
        public int Id { get; set; }
        public ApplicationUserViewModel User { get; set; }
        public List<MovieViewModel> Movies { get; set; }
    }
}
