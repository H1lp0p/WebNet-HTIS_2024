using Microsoft.AspNetCore.Diagnostics;
using webNet_courses.API.DTO;

namespace webNet_courses.API.Middlewear
{
	public class ExeptionsHandler : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			if (exception is not SystemException)
			{
				return false;
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
