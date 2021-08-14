using System;
using System.IO;
using System.Reflection;

namespace RankedPlaylistGenerator.Utils
{
    public class ResourceUtil
    {
        public static Lazy<string> GetCoverImageLazy(string name)
        {
            return new Lazy<string>(() => GetCoverImage(name));
        }
        
        public static string GetCoverImage(string name)
        {
            try
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
                {
                    var b = new byte[stream.Length];
                    stream.Read(b, 0, (int) stream.Length);
                    return Convert.ToBase64String(b);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return "";
        }
    }
}