using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Spoiler.Api.Helpers
{
    /// <summary>
    /// To handle unhandled exceptions, or any thrown Exception of type Exception.
    /// </summary>
    public class GeneralExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is Exception)
            {
                var exceptionType = context.Exception.GetType();

                if (exceptionType == typeof(BusinessException))
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = context.Exception.Message,
                        Instance = context.HttpContext.Request.Path
                    };

                    context.Result = new ObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" },
                        StatusCode = StatusCodes.Status400BadRequest,
                    };
                }
                else if (exceptionType == typeof(Exception))
                {

                    context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = MessagesText.GENERAL_EXCEPTION,
                        Instance = context.HttpContext.Request.Path,
                        Detail = context.Exception.Message
                    };

                    context.Result = new ObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" },
                        StatusCode = StatusCodes.Status500InternalServerError,
                    };
                }
            }

            base.OnException(context);
        }
    }

    [Serializable]
    public class BusinessException : Exception
    {
        public string ReferenceErrorCode { get; set; }
        public string ReferenceDocumentLink { get; set; }
        public List<string> Errors { get; protected set; }

        public string SqlExceptionMessage { get; protected set; }

        public BusinessException() { }

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, string referenceErrorCode) : base(message) { ReferenceErrorCode = referenceErrorCode; }

        public BusinessException(string message, string referenceErrorCode, string referenceDocumentLink) : this(message, referenceErrorCode) { ReferenceDocumentLink = referenceDocumentLink; }

        public BusinessException(string message, Exception inner) : base(message, inner) { }

        public BusinessException(List<string> erros) => Errors = erros;

        protected BusinessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
