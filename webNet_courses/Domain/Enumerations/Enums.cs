namespace webNet_courses.Domain.Enumerations
{
	public enum StudentMarks
	{
		Passsed,
		Failed,
		NotDefined
	}

	public enum StudentStatuses
	{
		InQueue,
		Accepted,
		Declined
	}

	public enum Semester
	{
		Autumn,
		Spring
	}

	public enum MarkType
	{
		Midterm,
		Final
	}

	public enum CourseSorting
	{
		CreatedAsc,
		CreatedDesc
	}

	public enum CourseStatuses
	{
		Created,
		OpenForAssignig,
		Started,
		Finished
	}
}
