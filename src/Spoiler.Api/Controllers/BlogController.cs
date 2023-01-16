using Microsoft.AspNetCore.Mvc;

namespace Spoiler.Api.Controllers
{

    [ApiController]
    [Route("/api/v1/blog")]
    [Produces("application/json")]
    public class BlogController : ControllerBase
    {
        /// <summary>
        /// Create new blog
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/blog/create (Return 201)
        ///     {
        ///        "title": "Title goes here",
        ///        "description": "Description goes here",
        ///     }
        /// 
        ///     POST /api/v1/blog/create (Return 412)
        ///     {
        ///        "title": "Title goes here",
        ///     }
        ///     
        ///     POST /api/v1/blog/create (Return 400)
        ///     {
        ///         "title": "no",
        ///         "description":"ok"
        ///     }
        /// 
        ///     POST /api/v1/blog/create (Return 500)
        ///     {
        ///        "title": "500",
        ///        "description": "Description goes here",
        ///     }
        ///  
        ///
        /// </remarks>
        /// <response code="201">OK Returns the newly created item</response>
        /// <response code="412">One or more validation errors occurred.</response>
        /// <response code="400">Business logic error.</response>
        /// <response code="500">General server error.</response>
        [Route("create")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Create([FromBody]Blog model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(statusCode:412);
            }

            if (model.Title == "no")
            {
                return Problem("Blog is already exist !",statusCode:400);
            }

            if (model.Title == "500")
            {
                return Problem("Something went wrong !",statusCode:500);
            }

            return Created(Request.Path, model);
        }


        /// <summary>
        /// Get blog by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/v1/blog/any-id-goes-here (Return 200)
        /// 
        ///     GET /api/v1/blog/404 (Return 404)
        ///     
        ///     GET /api/v1/blog/500 (Return 500)
        ///     
        /// </remarks>
        /// <response code="200">Returns blog item</response>
        /// <response code="404">Item not found.</response>
        /// <response code="500">General server error.</response>
        [Route("{id}")]
        [HttpGet]
        
        public IActionResult Get(string id)
        {
            if (id == "404")
            {
                return NotFound("Blog not found");
            }

            if (id == "500")
            {
                return Problem("Something went wrong !",statusCode:500);
            }

            var model = new Blog()
            {
                Title = "Here you go !",
                Description = "You must do it in the right way! no exception"
            };

            return Ok(model);
        }
    }
}
