using Microsoft.AspNetCore.Diagnostics;
using webNet_courses.API.DTO;
using webNet_courses.Domain.Excpetions;

namespace webNet_courses.API.Middlewear
{
	public class ExeptionsHandler : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			
			if (exception is FileNotFoundException)
			{
				httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
			}
			else if (exception is UnathorizedException)
			{
				httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
			}
			else if (exception is BLException)
			{
				httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
			}

			var response = new Response
			{
				Message = exception.Message,
				Status = "error",
			};

			await httpContext.Response.WriteAsJsonAsync(response);
			return true;
		}
	}
}
