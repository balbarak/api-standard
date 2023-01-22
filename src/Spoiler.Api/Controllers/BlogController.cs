using Microsoft.AspNetCore.Mvc;

namespace Spoiler.Api.Controllers
{

    [ApiController]
    [Route("/api/v1/blog")]
    [Produces("application/json")]
    public class BlogController : ControllerBase
    {
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

            var responseDto = Blog.Create(newBlog);

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
        /// <response code="404">Item not found.</response>
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

            var updatedBlog = Blog.Update(blogToUpdate);

            if(updatedBlog is null)
                return NotFound(MessagesText.BLOG_NOT_FOUND);

            return Ok(new BlogDto(updatedBlog));
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
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            var foundBlog = Blog.GetById(id);

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
        /// <response code="404">Item not found.</response>
        /// <response code="500">Internal server error.</response>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(string id)
        {
            if (Requested500Response(id))
                return Problem(MessagesText.GENERAL_EXCEPTION, statusCode: StatusCodes.Status500InternalServerError);

            if(!Blog.CanBeDeleted(id))
                return Problem(MessagesText.BLOG_CAN_NOT_BE_DELETED, statusCode: StatusCodes.Status400BadRequest);

            if (!Blog.BLOGS.Remove(Blog.GetById(id)!))
                return NotFound(MessagesText.BLOG_NOT_FOUND);

            return NoContent();
        }

        private static bool Requested500Response(string indicator)
        {
            return indicator == StatusCodes.Status500InternalServerError.ToString();
        }
    }
}
