using System.ComponentModel.DataAnnotations;

namespace MultimediaCenter.Models
{
    public enum MovieGenre
    {
        [Display(Name = "action")]
        action,
        [Display(Name = "comedy")]
        comedy,
        [Display(Name = "horror")]
        horror,
        [Display(Name = "thriller")]
        thriller
    }
}



