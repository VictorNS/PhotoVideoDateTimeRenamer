using PhotoVideoDateTimeRenamer;

Console.OutputEncoding = System.Text.Encoding.Unicode;

try
{
	string inputPath = args == null || args.Length == 0 ? Environment.CurrentDirectory : args[0];

	if (Directory.Exists(inputPath))
	{
		Console.WriteLine($"Working directory: {inputPath}");

		foreach (var filePath in Directory.GetFiles(inputPath, "*.*"))
		{
			var dTresult = MetadataDateTimeReader.Process(filePath);

			if (dTresult.IsSuccess)
			{
				//var outFileName = Path.Combine(inputPath, result.LocalDateTime.ToString("yyyy-MM-dd HH-mm-ss"), Path.GetExtension(fileName));
				//var outFileName = result.LocalDateTime.ToString("yyyy-MM-dd HH-mm-ss") + Path.GetExtension(filePath);
				var fnResult = FileNameParser.Process(filePath, dTresult.LocalDateTime);

				if (fnResult.IsSuccess)
					Console.WriteLine($"{Path.GetFileName(filePath)} => {fnResult.FileName}");
				else
					Console.WriteLine($"{Path.GetFileName(filePath)} => keep the same");
			}
			else
			{
				Console.WriteLine($"{Path.GetFileName(filePath)} => ???");
			}
		}
	}
	else
	{
		Console.WriteLine(@"Folder {0} does not exist. Please, define correct argument.", inputPath);
	}
}
catch (Exception ex)
{
	Console.WriteLine(ex.ToString());
	Console.ReadKey();
}
