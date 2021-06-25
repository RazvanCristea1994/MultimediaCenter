using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.ViewModel.Orders
{
    public class UpdateOrderForUserViewModel
    {
        public int Id { get; set; }
        public List<int> OrderIds { get; set; }
    }
}
