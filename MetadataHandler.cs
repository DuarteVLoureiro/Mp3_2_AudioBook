using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using TagLib.Id3v2;
using System.Diagnostics;

namespace Mp3_2_AudioBook
{
    internal class MetadataHandler
    {
        public class AudioMetadata
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public uint ReleaseYear { get; set; }
            public IPicture Cover { get; set; }
            public List<Chapter> Chapters { get; set; }
            
            public AudioMetadata()
            {
                Chapters = new List<Chapter>();
            }
        }
        public class Chapter
        {
            public string Title { get; set; }
            public long StartTime { get; set; }
            public long EndTime { get; set; }
        }
        public static AudioMetadata ReadMetadata(string InputFilePath)
        {
            var metadata = new AudioMetadata();

            using (var file = TagLib.File.Create(InputFilePath))
            {
                metadata.Title = file.Tag.Title ?? "Unknow Title";
                metadata.Author = file.Tag.FirstPerformer ?? "Unknown Author";
                metadata.ReleaseYear = file.Tag.Year;

                if (file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
                {
                    metadata.Cover = file.Tag.Pictures[0];
                }

                var id3Tag = file.GetTag(TagTypes.Id3v2) as TagLib.Id3v2.Tag;
                if (id3Tag != null)
                {
                    var chapFrames = id3Tag.GetFrames<TagLib.Id3v2.ChapterFrame>();
                    foreach (var frame in chapFrames)
                    {
                        var chapter = new Chapter();
                        chapter.StartTime = (long)frame.StartMilliseconds;
                        chapter.EndTime = (long)frame.EndMilliseconds;
                        chapter.Title = GetChapterTitle(frame);
                        metadata.Chapters.Add(chapter);
                    }
                }
            }

                return metadata;
        }
        public static int ReadBitrate(string InputFilePath)
        {
            using (var file = TagLib.File.Create(InputFilePath))
            {
                return file.Properties.AudioBitrate;
            }
        }
        private static string GetChapterTitle(TagLib.Id3v2.ChapterFrame Frame)
        {
            var embedFrames = Frame.SubFrames;
            foreach (var frame in embedFrames)
            {
                if (frame is TagLib.Id3v2.TextInformationFrame textFrame && textFrame.FrameId.ToString() == "TIT2")
                {
                    if (textFrame.Text.Length > 1)
                    {
                        return textFrame.Text.Length > 0 ? string.Join(" / ", textFrame.Text) : "Untitled Chapter";
                    }
                    else
                    {
                        return textFrame.Text.Length > 0 ? textFrame.Text[0] : "Untitled Chapter";
                    }
                }
            }
            return "Untitle Chapter";
        }
        public static void WriteMetadata(string filePath, AudioMetadata metadata)
        {
            // Write chapters to MP4
            WriteChaptersToMp4File(filePath, metadata.Chapters);
            using (var file = TagLib.File.Create(filePath))
            {
                file.Tag.Title = metadata.Title;
                file.Tag.Album = metadata.Title;
                file.Tag.Performers = new[] { metadata.Author }; 
                file.Tag.AlbumArtists = new[] { metadata.Author };
                file.Tag.Year = metadata.ReleaseYear;
                file.Tag.Genres = new[] { "Audiobook" };

                if (metadata.Cover != null)
                {
                    file.Tag.Pictures = new IPicture[] { metadata.Cover };
                }
                file.Save();
            }
        }
        public static string GetFFmpegPath()
        {
            //get path to ffmpeg,exe in the app folder
            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            if (!System.IO.File.Exists(ffmpegPath))
            {
                throw new FileNotFoundException("FFmpeg not found");
            }
            return ffmpegPath;
        }
        public static void WriteChaptersToMp4File(string file, List<Chapter> chapters)
        {
            if (chapters == null || chapters.Count == 0) return;

            string tempOutput = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".m4b");

            try
            {
                var metadata = new System.Text.StringBuilder();
                metadata.AppendLine(";FFMETADATA1");
                foreach (var chapter in chapters)
                {
                    metadata.AppendLine("[CHAPTER]");
                    metadata.AppendLine("TIMEBASE=1/1000");
                    metadata.AppendLine("START=" + chapter.StartTime);
                    metadata.AppendLine("END=" + chapter.EndTime);
                    metadata.AppendLine("title=" + chapter.Title);
                }
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = GetFFmpegPath(),
                    Arguments = $"-i \"{file}\" -f ffmetadata -i pipe:0 " +
                                $"-map 0:a " +
                                $"-map 0:v? " +
                                $"-map_metadata 1 " +
                                $"-map_chapters 1 " +
                                $"-codec copy " +
                                $"-disposition:v attached_pic " +
                                $"\"{tempOutput}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (var process = System.Diagnostics.Process.Start(processInfo))
                {
                    process.StandardInput.Write(metadata.ToString());
                    process.StandardInput.Close();

                    string stderr = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                        throw new InvalidOperationException(
                            $"FFmpeg failed with exit code {process.ExitCode}. Error: {stderr}");

                    System.IO.File.Delete(file);
                    System.IO.File.Move(tempOutput, file);
                }
            }
            finally
            {
                if (System.IO.File.Exists(tempOutput))
                    System.IO.File.Delete(tempOutput);
            }
        }
    }
}

