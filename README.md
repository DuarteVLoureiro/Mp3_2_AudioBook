# MP3 to AudioBook Converter

A lightweight Windows desktop tool that converts MP3 files to M4A format for audiobooks.

## Features

- Convert Mp3 files into Audiobook format (m4b)

## Requirements

- Windows 7 or later
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## How to Use

1. Launch the app
2. Drag and drop an MP3 file onto the drop zone, or click **Select File**
3. Optionally change the bitrate via the menu
4. Click **Convert**
5. The output `.m4b` file is saved in the same folder as the original MP3

## Building from Source

1. Clone the repository
2. Open `Mp3_2_AudioBook.sln` in Visual Studio 2022 or later
3. Restore NuGet packages (automatic on build)
4. Build and run

## Dependencies

- [NAudio](https://github.com/naudio/NAudio) — audio reading, resampling, and MediaFoundation encoding
- TagLib
- ffmpeg
