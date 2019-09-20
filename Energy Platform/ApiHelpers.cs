using System.Collections.Generic;

namespace Energy_Platform
{
    public class ApiHelpers
    {
        public static Dictionary<string, Dictionary<string, string>> GetStaticTypes()
        {
            var metersCubic = new Dictionary<string, string>
            {
                {"long", "м^3"},
                {"short", "Метри кубічні, м^3"}
            };

            var kilovatsPh = new Dictionary<string, string>
            {
                {"long", "Кв/год"},
                {"short", "Кіловат на годину, Кв/год"}
            };
            var gigaCal = new Dictionary<string, string>
            {
                {"long", "Гкал"},
                {"short", "Гігакалорій, Гкал"}
            };
            return new Dictionary<string, Dictionary<string, string>>
            {
                { "elec_kv", kilovatsPh },
                { "water_mc", metersCubic },
                { "gas_mc", metersCubic },
                { "heat_gc", gigaCal },
            };
        }
    }
}