using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using TagLib.Id3v2;

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
    }
}
