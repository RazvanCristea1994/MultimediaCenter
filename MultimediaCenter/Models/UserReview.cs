using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.Models
{
    public class UserReview
    {
        public int Id { get; set; }
        public String Content { get; set; }
        public int Stars { get; set; }
        public DateTime DateTime { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
