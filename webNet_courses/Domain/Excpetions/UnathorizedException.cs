namespace webNet_courses.Domain.Excpetions
{
	public class UnathorizedException : Exception
	{
		public string Status { get; set; }

		public UnathorizedException(string message = "User not found", string status = "error" ) : base(message)
		{
			Status = status;
		}
	}
}
