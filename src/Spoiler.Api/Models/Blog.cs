using System.ComponentModel.DataAnnotations;

namespace Spoiler.Api.Models
{
    public class Blog
    {
        [Required(ErrorMessage ="title is required")]
        public string Title { get; set; }


        [Required(ErrorMessage = "description is required")]
        public string Description { get; set; }
    }
}
