using System.Globalization;
using ConsoleApp;
using PhotoVideoDateTimeRenamer;

using var logger = new ConsoleFileLogger();

try
{
	var inputPath = Environment.CurrentDirectory;
	var toTimeZone = TimeZoneInfo.Local;

	if (args != null)
	{
		if (args.Length > 0)
		{
			inputPath = args[0];
		}

		if (args.Length > 1)
		{
			var timeZoneString = args[1];

			if (!timeZoneString.StartsWith('+') && !timeZoneString.StartsWith('-'))
				timeZoneString = "+" + timeZoneString;
			if (!timeZoneString.Contains(':'))
				timeZoneString += ":00";

			var timeZoneSpan = DateTimeOffset.ParseExact(timeZoneString, "zzz", CultureInfo.InvariantCulture).Offset;
			toTimeZone = TimeZoneInfo.CreateCustomTimeZone(timeZoneString, timeZoneSpan, null, null);
		}
	}

	if (Directory.Exists(inputPath))
	{
		var files = Directory.GetFiles(inputPath, "*.*");
		logger.WriteLine($"Working directory: {inputPath} with {files.Length} files");

		foreach (var filePath in files)
		{
			try
			{
				var dtResult = MetadataDateTimeReader.Process(filePath, toTimeZone);

				if (dtResult.IsSuccess)
				{
					var fnResult = FileNameParser.Process(filePath, dtResult.LocalDateTime);

					if (fnResult.IsSuccess)
					{
						logger.WriteLine($"{Path.GetFileName(filePath)} => {fnResult.FileName}");
						File.Move(filePath, Path.Combine(inputPath, fnResult.FileName));
					}
				}
			}
			catch (Exception ex)
			{
				logger.WriteLine($"\t{Path.GetFileName(filePath)} :: {ex.Message}");
			}
		}
	}
	else
	{
		logger.WriteLine("Folder {inputPath} does not exist. Please, define correct argument.");
	}
}
catch (Exception ex)
{
	logger.WriteLine();
	logger.WriteLine();
	logger.WriteLine(ex.ToString());
	return 1;
}
finally
{
	logger.MentionHerself();
}

return 0;
