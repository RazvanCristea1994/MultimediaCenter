using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.ViewModel.Favourites
{
    public class UpdateFavouritesForUserViewModel
    {
        public int Id { get; set; }
        public List<int> MovieIds { get; set; }
    }
}
