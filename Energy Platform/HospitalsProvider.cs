using System.IO;
using BAMCIS.GeoJSON;
using Newtonsoft.Json;

namespace Energy_Platform
{
    public class HospitalsProvider
    {
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
        public FeatureCollection GetFromFile(string path)
        {
            using (var t = File.OpenRead(path))
            using (var f = new StreamReader(t))
            {
               return (FeatureCollection)new JsonSerializer().Deserialize(f, typeof(FeatureCollection));
            }
        }
    }
}