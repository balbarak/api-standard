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
        ///     POST /Create (Return 201)
        ///     {
        ///        "title": "Title goes here",
        ///        "description": "Description goes here",
        ///     }
        /// 
        ///     POST /Create (Return 412)
        ///     {
        ///        "title": "Title goes here",
        ///     }
        ///     
        ///     POST /Create (Return 400)
        ///     {
        ///         "title": "no",
        ///         "description":"ok"
        ///     }
        /// 
        ///     POST /Create (Return 500)
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
                return GetModelStateErrors();
            }

            if (model.Title == "no")
            {
                return GetError("title already exist.",300);
            }

            if (model.Title == "500")
            {
                return GetError("Something went wrong.",500,100);
            }

            return Created(Request.Path, model);
        }
        
        protected ObjectResult GetModelStateErrors()
        {
            var error = new ValidationProblemDetails(ModelState)
            {
                Instance = HttpContext.Request.Path,
                Type = "https://httpstatuses.com/412",
                Title = "One or more validation errors occurred.",
                Status = 412
            };

            var model = error.ToModel();

            return new ObjectResult(model)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = 412
            };

            //return result;
        }

        protected ObjectResult GetError(string message = null, int statusCode = 400,int applicationErrorCode = 100)
        {
            var problem = new ValidationProblemDetails()
            {
                Title = message ?? "Unkown error.",
                Instance = HttpContext.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}",
            };

            var model = problem.ToModel(applicationErrorCode);

            return new ObjectResult(model)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = statusCode,
            };
        }
    }
}
