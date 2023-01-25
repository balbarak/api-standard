using Spoiler.Api.Controllers;

namespace Spoiler.Api.Services
{
    public interface IBlogService
    {
        bool CanBeDeleted(string id);
        BlogDto Create(CreateBlogRequest create);
        Blog? GetById(string id);
        Blog? Update(Blog? blog);
    }

    public class BlogService : IBlogService
    {
        /// <summary>
        /// Whenever edited, update <see cref="BlogController.Delete(string)"/> 
        /// </summary>
        public const string DEFAULT_BLOG_ID = "05cf7a03-3747-4e1a-890e-af67e35b039f";

        public static List<Blog> BLOGS = new() {
            CreateDefault()
        };

        public BlogDto Create(CreateBlogRequest create)
        {
            var blog = new Blog(create);
            BLOGS.Add(blog);
            return new BlogDto(blog);
        }

        public Blog? GetById(string id)
        {
            return BLOGS.FirstOrDefault(a => a.Id == id);
        }

        public Blog? Update(Blog? blog)
        {
            var foundBlog = BLOGS.FirstOrDefault(a => a.Id == blog?.Id);

            if (foundBlog is null)
                return null;

            foundBlog.Title = blog?.Title;
            foundBlog.Description = blog?.Description;

            return foundBlog;
        }

        public bool CanBeDeleted(string id)
        {
            return id != DEFAULT_BLOG_ID;
        }

        private static Blog CreateDefault()
        {
            return new Blog()
            {
                Id = DEFAULT_BLOG_ID,
                Title = "Default blog",
                Description = "This is a default blog that cannot be deleted."
            };
        }
    }
}
