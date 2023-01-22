using Microsoft.AspNetCore.Mvc;

namespace Spoiler.Api.Controllers
{

    [ApiController]
    [Route("/api/v1/blog")]
    [Produces("application/json")]
    public class BlogController : ControllerBase
    {
        private static readonly List<Blog> _blogs = new();
        private const string NOT_FOUND_TEXT = "Blog not found.";
        private const string GENERAL_EXCEPTION_TEXT = "Something went wrong!";
        private const string BLOG_ALREADY_EXISTS_TEXT = "The Blog already exists!";

        /// <summary>
        /// Create a new blog.
        /// </summary>
        /// <param name="newBlog"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/blog/create (Returns 201)
        ///     {
        ///        "title": "Title goes here.",
        ///        "description": "Description goes here.",
        ///     }
        /// 
        ///     POST /api/v1/blog/create (Returns 412)
        ///     {
        ///        "title": "Title goes here.",
        ///     }
        ///     
        ///     POST /api/v1/blog/create (Returns 400)
        ///     {
        ///         "title": "no",
        ///         "description":"ok"
        ///     }
        /// 
        ///     POST /api/v1/blog/create (Returns 500)
        ///     {
        ///        "title": "500",
        ///        "description": "Description goes here.",
        ///     }
        ///  
        ///
        /// </remarks>
        /// <response code="201">OK Returns the newly created item.</response>
        /// <response code="412">One or more validation errors occurred.</response>
        /// <response code="400">Business logic error.</response>
        /// <response code="500">Internal server error.</response>
        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(BlogDto), StatusCodes.Status201Created)]
        public IActionResult Create([FromBody] CreateBlogRequest newBlog)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(statusCode: 412);

            if (newBlog.Title == "no")
                return Problem(BLOG_ALREADY_EXISTS_TEXT, statusCode: StatusCodes.Status400BadRequest);

            if (Requested500Response(newBlog.Title!))
                return Problem(GENERAL_EXCEPTION_TEXT, statusCode: StatusCodes.Status500InternalServerError);

            var responseDto = Blog.Create(newBlog, _blogs);

            return Created(Request.Path, responseDto);
        }


        /// <summary>
        /// Get blog by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/v1/blog/any-id-goes-here (Returns 200, or 404)
        ///     
        ///     GET /api/v1/blog/500 (Returns 500)
        ///     
        /// </remarks>
        /// <response code="200">Returns blog item.</response>
        /// <response code="404">Item not found.</response>
        /// <response code="500">Internal server error.</response>
        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(BlogDto), StatusCodes.Status200OK)]
        public IActionResult Get(string id)
        {
            if (Requested500Response(id))
                return Problem(GENERAL_EXCEPTION_TEXT, statusCode: StatusCodes.Status500InternalServerError);

            var foundBlog = GetBlogById(id);

            if (foundBlog is null)
                return NotFound(NOT_FOUND_TEXT);

            return Ok(new BlogDto(foundBlog));
        }

        /// <summary>
        /// Delete a blog by id.
        /// </summary>
        /// <param name="id">the blog Id to delete.</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/v1/blog/any-id-goes-here (Returns 204, or 404)
        /// 
        ///     GET /api/v1/blog/500 (Returns 500)
        ///     
        /// </remarks>
        /// <response code="204">Returns (No Content), indicates item deletion.</response>
        /// <response code="404">Item not found.</response>
        /// <response code="500">Internal server error.</response>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(string id)
        {
            if (Requested500Response(id))
                return Problem(GENERAL_EXCEPTION_TEXT, statusCode: StatusCodes.Status500InternalServerError);

            if (!_blogs.Remove(GetBlogById(id)!))
                return NotFound(NOT_FOUND_TEXT);

            return NoContent();
        }

        private static Blog? GetBlogById(string id)
        {
            return _blogs.FirstOrDefault(a => a.Id == id);
        }

        private static bool Requested500Response(string indicator)
        {
            return indicator == StatusCodes.Status500InternalServerError.ToString();
        }
    }
}
