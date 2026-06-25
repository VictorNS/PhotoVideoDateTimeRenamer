# PhotoVideoDateTimeRenamer

PhotoVideoDateTimeRenamer is a small .NET command-line tool that renames photo and video files from their embedded metadata timestamps.

It is useful for normalizing camera-roll exports, especially Google Pixel files such as `PXL_20251128_173728451.TS.mp4`,
into sortable filenames such as `2025-11-28 19-38-09-451.TS.mp4`.

## Features

- Reads photo timestamps from EXIF `DateTimeOriginal` metadata.
- Reads video creation timestamps from QuickTime movie header metadata.
- Converts timestamps to the local system timezone by default.
- Accepts an optional target timezone offset, such as `+02:00` or `-05`.
- Preserves Google Pixel filename suffixes such as `.TS`, `.LS`, and `.PORTRAIT` when possible.
- Writes the rename results to both the console and a timestamped log file.

## Requirements

- .NET 8 SDK to build from source.
- .NET 8 runtime to run framework-dependent builds.
- Windows x64 if you use the included `publish.cmd` script.

The project depends on the [`MetadataExtractor`](https://www.nuget.org/packages/MetadataExtractor) NuGet package for metadata reading.

## Build

To publish a single-file Windows x64 executable into the `publish` directory, run:

```cmd
publish.cmd
```

Because the publish is framework-dependent, the target machine still needs the .NET runtime installed.

## Usage

```cmd
PhotoVideoDateTimeRenamer.exe [directory] [timezone-offset]
```

Arguments:

- `directory` is optional. If omitted, the current working directory is used.
- `timezone-offset` is optional. If omitted, the local system timezone is used.

Examples:

```cmd
PhotoVideoDateTimeRenamer.exe
PhotoVideoDateTimeRenamer.exe C:\Temp\Photos
PhotoVideoDateTimeRenamer.exe C:\Temp\Photos +02:00
PhotoVideoDateTimeRenamer.exe C:\Temp\Photos -05
```

The tool processes files directly inside the selected directory. It does not scan subdirectories recursively.

## Timezone Handling

The second argument controls the target timezone used for generated filenames.

Accepted formats include:

- `+02:00`
- `-05:00`
- `+02`
- `-05`
- `02`, which is treated as `+02:00`

When EXIF metadata contains an original timezone tag, the timestamp is converted from that timezone into the target timezone. When video metadata is stored as UTC, it is converted from UTC into the target timezone. Local timestamps are kept as local timestamps when no original timezone is available.

## Filename Format

Generated filenames use this timestamp format:

```text
yyyy-MM-dd HH-mm-ss
```

For Google Pixel names beginning with `PXL_`, the millisecond part and any Pixel suffix are preserved from the original name:

```text
PXL_20251128_173728451.TS.mp4
2025-11-28 19-38-09-451.TS.mp4
```

For names already beginning with a compact date pattern such as `yyyyMMdd`, the generated timestamp is used and the trailing filename portion is preserved from the timestamp boundary.

For other supported files, the generated timestamp is combined with the original file extension:

```text
IMG_1234.jpg
2025-11-24 19-29-44.jpg
```

If the generated filename is the same as the current filename, the file is skipped.

## Output Example

```console
C:\GitHub\PhotoVideoDateTimeRenamer\publish>PhotoVideoDateTimeRenamer.exe C:\Temp\_
Working directory: C:\Temp\_ with 7 files
PXL_20251124_172944897.jpg => 2025-11-24 19-29-44-897.jpg
PXL_20251128_173728451.TS.mp4 => 2025-11-28 19-38-09-451.TS.mp4
PXL_20251129_162849549.PORTRAIT.jpg => 2025-11-29 18-28-49-549.PORTRAIT.jpg
PXL_20251202_070858582.jpg => 2025-12-02 09-08-58-582.jpg
PXL_20251205_190626821.LS.mp4 => 2025-12-05 21-06-26-821.LS.mp4
PXL_20251206_151913788.TS.mp4 => 2025-12-06 17-19-24-788.TS.mp4
PXL_20251209_160558851.jpg => 2025-12-09 18-05-58-851.jpg

-------------------------------------------------------------
You can find this log in C:\GitHub\PhotoVideoDateTimeRenamer\publish\20260625_215251.log

C:\GitHub\PhotoVideoDateTimeRenamer\publish>
```

## Logging

Every run creates a log file next to the executable using this name format:

```text
yyyyMMdd_HHmmss.log
```

The same rename messages are written to the console and the log file. Per-file errors are also logged, allowing the run to continue with the remaining files.

## Important Notes

- Files are renamed in place. Make a backup before running the tool on important folders.
- Only files in the selected directory are processed. Subdirectories are ignored.
- Files without supported metadata are skipped.
- Existing target filename collisions are not resolved automatically. If a generated name already exists, the rename operation can fail for that file.
- Unsupported, unreadable, or corrupt files may produce per-file log messages.
- If the target directory does not exist, the tool logs an error and exits without processing files.

## Project Structure

- `Program.cs` contains argument parsing and the main rename loop.
- `MetadataDateTimeReader.cs` reads EXIF and QuickTime timestamps.
- `FileNameParser.cs` creates the new filenames.
- `ConsoleFileLogger.cs` writes console output and log files.
- `publish.cmd` publishes the Windows x64 executable.

## License

This project is released into the public domain under the [Unlicense](LICENSE).
