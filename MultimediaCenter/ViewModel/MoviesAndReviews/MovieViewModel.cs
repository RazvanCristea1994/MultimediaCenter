using MultimediaCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MultimediaCenter.Models.Movie;

namespace MultimediaCenter.ViewModel
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int YearOfRelease { get; set; }
        public string Director { get; set; }
        public DateTime AddedDate { get; set; }
        public int Rating { get; set; }
        public bool Watched { get; set; } = false;
        public MovieGenre Genre { get; set; }

        public enum MovieGenre
        {
            Action, Comedy, Horror, Thriller
        }
    }
}
