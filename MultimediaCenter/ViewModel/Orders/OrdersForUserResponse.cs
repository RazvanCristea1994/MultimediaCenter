using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.ViewModel.Orders
{
    public class OrdersForUserResponse
    {
        public int Id { get; set; }
        public ApplicationUserViewModel User { get; set; }
        public List<MovieViewModel> Movies { get; set; }
        public DateTime OrderDateTime { get; set; }
    }
}
