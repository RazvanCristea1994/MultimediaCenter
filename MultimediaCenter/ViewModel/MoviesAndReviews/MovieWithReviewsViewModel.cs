using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.ViewModel
{
    public class MovieWithReviewsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int YearOfRelease { get; set; }
        public string Director { get; set; }
        public DateTime AddedDate { get; set; }
        public int Rating { get; set; }
        public bool Watched { get; set; }
        public MovieGenre Genre { get; set; }

        public enum MovieGenre
        {
            Action, Comedy, Horror, Thriller
        }

        public IEnumerable<ReviewViewModel> UserReviews { get; set; }
        public IEnumerable<FavouritesForUserResponse> Favourites { get; set; }
    }
}
