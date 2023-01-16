using Microsoft.AspNetCore.Mvc;

namespace Spoiler.Api
{
    public static class ValidationProblemDetailsExtensions
    {
        public static ErrorModel ToModel(this ValidationProblemDetails enttiy,int applicationErrorCode = 100)
        {
            return new ErrorModel()
            {
                Errors = enttiy.Errors,
                Instance = enttiy.Instance,
                Title = enttiy.Title,
                Type = enttiy.Type,
                ErrorCode = applicationErrorCode
            };
        }
    }
}
