using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MultimediaCenter.Models
{
    public class Movie
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

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum MovieGenre
        {
            Action, Comedy, Horror, Thriller
        }

        public List<UserReview> UserReviews { get; set; }
    }
}
