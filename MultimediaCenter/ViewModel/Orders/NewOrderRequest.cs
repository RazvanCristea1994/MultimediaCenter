using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.ViewModel.Orders
{
    public class NewOrderRequest
    {
        public List<int> OrderedMoviesIds { get; set; }
        public DateTime? OrderedDateTime { get; set; }
    }
}
