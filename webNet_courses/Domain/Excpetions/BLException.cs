namespace webNet_courses.Domain.Excpetions
{
	public class BLException : Exception
	{
		public string Status { get; set; }

		public BLException(string message = "You want something strange", string status = "error") : base(message) 
		{
			this.Status = status;
		}
	}
}
