using Spoiler.Api.Controllers;
using System.ComponentModel.DataAnnotations;

namespace Spoiler.Api.Models
{
    public class Blog
    {
        /// <summary>
        /// Whenever edited, update <see cref="BlogController.Delete(string)"/> 
        /// </summary>
        public static readonly string DEFAULT_BLOG_ID = "05cf7a03-3747-4e1a-890e-af67e35b039f";

        public static readonly List<Blog> BLOGS = new() {
            CreateDefault()
        };

        [Required(ErrorMessage = "id is required")]
        public string? Id { get; set; }

        [Required(ErrorMessage = "title is required")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "description is required")]
        public string? Description { get; set; }

        public Blog() { }

        public Blog(CreateBlogRequest create)
        {
            Id = Guid.NewGuid().ToString();
            Title = create.Title;
            Description = create.Description;
        }

        public static BlogDto Create(CreateBlogRequest create)
        {
            var blog = new Blog(create);
            BLOGS.Add(blog);
            return new BlogDto(blog);
        }

        public static Blog CreateDefault()
        {
            return new Blog()
            {
                Id = DEFAULT_BLOG_ID,
                Title = "Default blog",
                Description = "This is a default blog that cannot be deleted."
            };
        }

        public static Blog? GetById(string id)
        {
            return BLOGS.FirstOrDefault(a => a.Id == id);
        }

        public static Blog? Update(Blog? blog)
        {
            var foundBlog = BLOGS.FirstOrDefault(a => a.Id == blog?.Id);

            if (foundBlog is null)
                return null;

            foundBlog.Title = blog?.Title;
            foundBlog.Description = blog?.Description;

            return foundBlog;
        }

        public static bool CanBeDeleted(string id)
        {
            return id != DEFAULT_BLOG_ID;
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
