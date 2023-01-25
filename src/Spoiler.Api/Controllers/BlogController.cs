using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spoiler.Api.Helpers;
using Spoiler.Api.Services;

namespace Spoiler.Api.Controllers
{

    [ApiController]
    [Route("/api/v1/blog")]
    [Produces("application/json")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

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
        ///     POST /api/v1/blog/create (Returns 400 - validation errors)
        ///     {
        ///        "title": "Title goes here.",
        ///     }
        ///     
        ///     POST /api/v1/blog/create (Returns 400 - business logic error)
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
        /// <response code="400">Business logic/validation errors.</response>
        /// <response code="500">Internal server error.</response>
        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(BlogDto), StatusCodes.Status201Created)]
        public IActionResult Create([FromBody] CreateBlogRequest newBlog)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(statusCode: StatusCodes.Status400BadRequest);

            if (newBlog.Title == "no")
                return Problem(MessagesText.BLOG_ALREADY_EXISTS, statusCode: StatusCodes.Status400BadRequest);

            if (Requested500Response(newBlog.Title!))
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            var responseDto = _blogService.Create(newBlog);

            return Created(Request.Path, responseDto);
        }

        /// <summary>
        /// Update an existing blog.
        /// </summary>
        /// <param name="blogToUpdate"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/blog/update (Returns 200)
        ///     {
        ///        "id": "id_of_an_existing_item"
        ///        "title": "Title goes here.",
        ///        "description": "Description goes here.",
        ///     }
        /// 
        ///     POST /api/v1/blog/update (Returns 400 - validation errors)
        ///     {
        ///        "title": "Title goes here.",
        ///     }
        /// 
        ///     POST /api/v1/blog/update (Returns 500)
        ///     {
        ///        "id": "500",
        ///     }
        ///  
        ///
        /// </remarks>
        /// <response code="200">OK Returns the updated item.</response>
        /// <response code="400">Business logic/validation errors.</response>
        /// <response code="404">Blog not found.</response>
        /// <response code="500">Internal server error.</response>
        [Route("update")]
        [HttpPost]
        [ProducesResponseType(typeof(BlogDto), StatusCodes.Status200OK)]
        public IActionResult Update([FromBody] Blog blogToUpdate)
        {
            if (Requested500Response(blogToUpdate?.Id!))
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            if (!ModelState.IsValid)
                return ValidationProblem(statusCode: StatusCodes.Status400BadRequest);

            var updatedBlog = _blogService.Update(blogToUpdate);

            if(updatedBlog is null)
                return NotFound(MessagesText.BLOG_NOT_FOUND);

            return Ok(new BlogDto(updatedBlog));
        }

        /// <summary>
        /// Search posts.
        /// </summary>
        /// <param name="search">Search criteria.</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/v1/blog (Returns 200, or 204)
        ///     
        ///     GET /api/v1/blog?keyword=500 (Returns 500)
        ///     
        /// </remarks>
        /// <response code="200">Returns blog items.</response>
        /// <response code="204">If there is no result.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(BlogSearchResult), StatusCodes.Status200OK)]
        public IActionResult Get([FromQuery] BlogSearch search)
        {
            if (Requested500Response(search.Keyword!))
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            var searchResult = Blog.Search(search);

            if (searchResult?.Items is null || !searchResult.Items.Any())
                return NoContent();

            return Ok(searchResult);
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
        /// <response code="404">Blog not found.</response>
        /// <response code="500">Internal server error.</response>
        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(BlogDto), StatusCodes.Status200OK)]
        public IActionResult Get(string id)
        {
            if (Requested500Response(id))
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            var foundBlog = _blogService.GetById(id);

            if (foundBlog is null)
                return NotFound(MessagesText.BLOG_NOT_FOUND);

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
        ///     DELETE /api/v1/blog/any-id-goes-here (Returns 204, or 404)
        /// 
        ///     DELETE /api/v1/blog/05cf7a03-3747-4e1a-890e-af67e35b039f (Returns 400 - business logic error)
        /// 
        ///     DELETE /api/v1/blog/500 (Returns 500)
        ///     
        /// </remarks>
        /// <response code="204">Returns (No Content), indicates item deletion.</response>
        /// <response code="400">Business logic/validation errors.</response>
        /// <response code="404">Blog not found.</response>
        /// <response code="500">Internal server error.</response>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(string id)
        {
            if (Requested500Response(id))
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            if(!_blogService.CanBeDeleted(id))
                return Problem(MessagesText.BLOG_CAN_NOT_BE_DELETED, statusCode: StatusCodes.Status400BadRequest);

            if (!BlogService.BLOGS.Remove(_blogService.GetById(id)!))
                return NotFound(MessagesText.BLOG_NOT_FOUND);

            return NoContent();
        }

        /// <summary>
        /// Get blog by id.
        /// </summary>
        /// <param name="id">the blog's id.</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/v1/blog/secured (Returns 200, or 404)
        ///     
        ///     GET /api/v1/blog/secured/500 (Returns 500)
        ///     
        /// </remarks>
        /// <response code="200">Returns the default blog.</response>
        /// <response code="400">Business logic/validation errors.</response>
        /// <response code="401">Not Authorized.</response>
        /// <response code="404">Item not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("secured"), Authorize]
        [ProducesResponseType(typeof(BlogDto), StatusCodes.Status200OK)]
        public IActionResult Secured(string id)
        {
            if (Requested500Response(id))
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            var foundBlog = _blogService.GetById(BlogService.DEFAULT_BLOG_ID);

            if (foundBlog is null)
                return NotFound(MessagesText.BLOG_NOT_FOUND);

            return Ok(new BlogDto(foundBlog));
        }

        /// <summary>
        /// Get blog by id.
        /// </summary>
        /// <param name="id">the blog's id.</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/v1/blog/secured (Returns 200, or 404)
        ///     
        ///     GET /api/v1/blog/secured/500 (Returns 500)
        ///     
        /// </remarks>
        /// <response code="200">Returns the default blog.</response>
        /// <response code="400">Business logic/validation errors.</response>
        /// <response code="401">Not Authorized.</response>
        /// <response code="403">Insufficient role.</response>
        /// <response code="404">Item not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("secured-role"), Authorize(Roles = "Developer")]
        [ProducesResponseType(typeof(BlogDto), StatusCodes.Status200OK)]
        public IActionResult SecuredRole(string id)
        {
            if (Requested500Response(id))
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            var foundBlog = _blogService.GetById(BlogService.DEFAULT_BLOG_ID);

            if (foundBlog is null)
                return NotFound(MessagesText.BLOG_NOT_FOUND);

            return Ok(new BlogDto(foundBlog));
        }

        private static bool Requested500Response(string indicator)
        {
            return indicator == StatusCodes.Status500InternalServerError.ToString();
        }
    }
}
