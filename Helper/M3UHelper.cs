namespace Read_m3u_file.Helper
{
    public class M3UHelper
    {
        public static List<(string Name, string Url)> ReadM3UFile(string filePath)
        {
            var channels = new List<(string Name, string Url)>();

            var lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#EXTINF:"))
                {
                    // Extract the channel name
                    var name = lines[i].Split(new[] { "tvg-name=\"", "\"" }, StringSplitOptions.None)
                                       .Skip(1)
                                       .FirstOrDefault()?.Trim();

                    // If tvg-name is not found, use the display name after the comma
                    if (string.IsNullOrEmpty(name))
                    {
                        name = lines[i].Split(',').LastOrDefault()?.Trim();
                    }

                    // Get the URL (next line)
                    if (i + 1 < lines.Length && !lines[i + 1].StartsWith("#"))
                    {
                        var url = lines[i + 1].Trim();
                        channels.Add((name, url));
                    }
                }
            }

            return channels;
        }
        public static void WriteM3UFile(string path, List<string> videoUrls)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("#EXTM3U");
                foreach (string videoUrl in videoUrls)
                {
                    writer.WriteLine(videoUrl);
                }
            }
        }
    }
}
