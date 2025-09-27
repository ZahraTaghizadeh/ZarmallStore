using Newtonsoft.Json;

namespace ZarmallStore.Web.Extensions
{
    public class ProvinceFinder
    {
        private static List<ProvinceData> _provinceData;

        static ProvinceFinder()
        {
            // Load JSON data from file
            var json = File.ReadAllText("JsonFiles/Cities.json");
            _provinceData = JsonConvert.DeserializeObject<List<ProvinceData>>(json);
        }

        public static string GetProvince(string city)
        {
            return _provinceData.FirstOrDefault(p => p.cities.Contains(city))?.province;
        }
    }

    public class ProvinceData
    {
        public string province { get; set; }
        public List<string> cities { get; set; }
    }
}
