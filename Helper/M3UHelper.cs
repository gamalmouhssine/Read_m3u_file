namespace Read_m3u_file.Helper
{
    public class M3UHelper
    {
        public static List<string> ReadM3UFile(string path)
        {
            List<string> result = new List<string>();
            var videoUrls = new List<string>();
            foreach (string line in File.ReadLines(path))
            {
                if (!line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                {
                    videoUrls.Add(line.Trim());
                }
            }
            return videoUrls;   
        }
    }
}
