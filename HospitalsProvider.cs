using System.Collections.Generic;
using System.IO;
using BAMCIS.GeoJSON;
using Newtonsoft.Json;

namespace Energy_Platform
{
    public class HospitalsProvider
    {
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