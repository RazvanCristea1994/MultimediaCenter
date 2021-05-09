using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.Models
{
    public class Movie
    {
        public int Id { get; set; } 

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public MovieGenre Genre { get; set; }
        
        [MinLength(5)]
        public int Duration { get; set; }
        [MinLength(1990)]
        public int YearOfRelease { get; set; }
        [Required]
        public string Director { get; set; }
        public DateTime AddedDate { get; set; }
        [Range(1, 10)]
        public int Rating { get; set; }
        public bool Watched { get; set; }
    }
}
