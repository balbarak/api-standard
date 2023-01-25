using Spoiler.Api.Controllers;
using System.ComponentModel.DataAnnotations;

namespace Spoiler.Api.Models
{
    public class Blog
    {
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


        public static BlogSearchResult Search(BlogSearch search)
        {
            Func<Blog, bool> predicate = a => true;
            if (!string.IsNullOrEmpty(search.Keyword))
            {
                predicate = a => a.Title!.Contains(search.Keyword, StringComparison.InvariantCultureIgnoreCase)
                || a.Description!.Contains(search.Keyword, StringComparison.InvariantCultureIgnoreCase);
            }

            var query = BLOGS.Where(predicate);
            var count = query.Count();
            var items = query
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(blog => new BlogDto(blog))
                .ToList().AsReadOnly();

            return new BlogSearchResult(search, items, count);
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
