using System.ComponentModel.DataAnnotations;

namespace Spoiler.Api.Models
{
    public class Blog
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "title is required")]
        public string? Title { get; set; }


        [Required(ErrorMessage = "description is required")]
        public string? Description { get; set; }

        public Blog()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Blog(CreateBlogRequest create)
            : this()
        {
            Title = create.Title;
            Description = create.Description;
        }

        public static BlogDto Create(CreateBlogRequest create, List<Blog> blogs)
        {
            blogs ??= new List<Blog>();

            var blog = new Blog(create);

            blogs.Add(blog);

            return new BlogDto(blog);
        }
    }

    public class CreateBlogRequest
    {
        [Required(ErrorMessage = "title is required")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "description is required")]
        public string? Description { get; set; }
    }

    public class BlogDto
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public BlogDto(Blog blog)
        {
            Id = blog.Id;
            Title = blog.Title;
            Description = blog.Description;
        }
    }
}
