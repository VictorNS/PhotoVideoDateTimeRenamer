namespace PhotoVideoDateTimeRenamer;

public static class FileNameParser
{
	public static Result Process(string filePath, DateTime localDateTime)
	{
		var fileName = Path.GetFileName(filePath);
		var newFileNameYMDHMS = localDateTime.ToString("yyyy-MM-dd HH-mm-ss");
		string newFileName;

		if (fileName.StartsWith("PXL_" + newFileNameYMDHMS.Substring(0, 3)))
			newFileName = newFileNameYMDHMS + fileName[23..];
		else if (fileName.StartsWith(newFileNameYMDHMS.Substring(0, 8)))
			newFileName = newFileNameYMDHMS + fileName[19..];
		else
			newFileName = newFileNameYMDHMS + Path.GetExtension(filePath);

		if (fileName == newFileName)
			return Result.Fail(newFileName);

		return Result.Ok(newFileName);
	}

	public record Result(string FileName, bool IsSuccess)
	{
		public static Result Ok(string fileName) => new(fileName, true);
		public static Result Fail(string fileName) => new(fileName, false);
	}
}
