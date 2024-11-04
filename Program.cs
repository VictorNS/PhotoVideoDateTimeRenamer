using ConsoleApp;
using PhotoVideoDateTimeRenamer;

using var logger = new ConsoleFileLogger();

try
{
	string inputPath = args == null || args.Length == 0 ? Environment.CurrentDirectory : args[0];

	if (Directory.Exists(inputPath))
	{
		var files = Directory.GetFiles(inputPath, "*.*");
		logger.WriteLine($"Working directory: {inputPath} with {files.Length} files");

		foreach (var filePath in files)
		{
			try
			{
				var dtResult = MetadataDateTimeReader.Process(filePath);

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
