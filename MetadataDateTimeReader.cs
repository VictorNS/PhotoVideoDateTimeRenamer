using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;
using System.Globalization;
using System.Text;

namespace PhotoVideoDateTimeRenamer;

public static class MetadataDateTimeReader
{
	public static Result Process(string filePath)
	{
		var directories = ImageMetadataReader.ReadMetadata(filePath);

		var exifSubIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

		if (exifSubIfdDirectory is not null)
		{
			if (exifSubIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTimeOriginal))
			{
				var timeZoneOriginalObject = exifSubIfdDirectory.GetString(ExifDirectoryBase.TagTimeZoneOriginal);
				//Console.WriteLine($"\tDate/Time Original: {dateTimeOriginal} Time Zone Original: {timeZoneOriginalObject}");
				return Result.Ok(NormilizeDateTime(dateTimeOriginal, timeZoneOriginalObject));
			}
			else
			{
				return Result.Fail();
			}
		}

		var quickTimeMovieHeaderDirectory = directories.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();

		if (quickTimeMovieHeaderDirectory is not null)
		{
			if (quickTimeMovieHeaderDirectory.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out var dateCreated))
			{
				//Console.WriteLine($"\tDateTime Created: {dateCreated}");
				return Result.Ok(NormilizeDateTime(dateCreated));
			}
			else
			{
				return Result.Fail();
			}
		}

		return Result.Fail();
	}

	static DateTime NormilizeDateTime(DateTime dateTime, string? timeZoneString = "")
	{
		TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

		if (!string.IsNullOrEmpty(timeZoneString))
		{
			var timeZoneSpan = DateTimeOffset.ParseExact(timeZoneString, "zzz", CultureInfo.InvariantCulture).Offset;
			var customTimeZone = TimeZoneInfo.CreateCustomTimeZone(timeZoneString, timeZoneSpan, null, null);

			//if (dateTime.Kind == DateTimeKind.Local)
			//	Console.WriteLine("\tThe DateTime is in local time.");
			//else if (dateTime.Kind == DateTimeKind.Utc)
			//	Console.WriteLine("\tThe DateTime is in UTC.");

			var localDateTime = TimeZoneInfo.ConvertTime(dateTime, customTimeZone, localTimeZone);
			return localDateTime;
		}
		else if (dateTime.Kind == DateTimeKind.Local)
		{
			//Console.WriteLine("\tThe DateTime is in local time.");
			return dateTime;
		}
		else
		{
			//if (dateTime.Kind == DateTimeKind.Utc)
			//	Console.WriteLine("\tThe DateTime is in UTC.");

			var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, localTimeZone);
			return localDateTime;
		}
	}

	public record Result(DateTime LocalDateTime, bool IsSuccess)
	{
		public static Result Ok(DateTime localDateTime) => new(localDateTime, true);
		public static Result Fail() => new(DateTime.UtcNow, false);
	}
}
