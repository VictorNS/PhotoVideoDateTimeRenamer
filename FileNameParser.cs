namespace PhotoVideoDateTimeRenamer;

public static class FileNameParser
{
	public static Result Process(string filePath, DateTime localDateTime)
	{
		var fileName = Path.GetFileName(filePath);
		string newFileName;

		if (fileName.StartsWith("PXL_" + localDateTime.Year.ToString().Substring(0, 3)))
		{
			newFileName = localDateTime.ToString("yyyy-MM-dd HH-mm-ss") + fileName[19..];
		}
		else
		{
			newFileName = localDateTime.ToString("yyyy-MM-dd HH-mm-ss") + Path.GetExtension(filePath);
		}

		if (filePath == newFileName)
			return Result.Fail(newFileName);

		return Result.Ok(newFileName);
	}

	public record Result(string FileName, bool IsSuccess)
	{
		public static Result Ok(string fileName) => new(fileName, true);
		public static Result Fail(string fileName) => new(fileName, false);
	}
}
