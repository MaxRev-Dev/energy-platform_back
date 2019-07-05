using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;

namespace Energy_Platform
{
    public class CsvParser
    {
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
        public IEnumerable<Record> FromFile(string file)
        {
            using (var stream = File.OpenRead(file))

            using (var reader = new StreamReader(stream))
            {
                var re = new CsvHelper.CsvReader(reader,
                    new Configuration
                    {
                        Delimiter = ","
                    });
                re.Configuration.HasHeaderRecord = false;

                var records = re.GetRecords<dynamic>();
                int type = 0;

                var types = ApiHelpers.GetStaticTypes();

                using (var enu = records.GetEnumerator())
                    while (enu.MoveNext())
                    {
                        var c = enu.GetCurrent();

                        if (c.Values.AreAllEmpty())
                            continue;

                        //entity branch name
                        var name = c.First().Key;

                        var t = types.ElementAt(type++).Value["short"];

                        enu.MoveNext();
                        enu.MoveNext();
                        enu.MoveNext();

                        c = enu.GetCurrent();

                        var month = c.Values.Skip(1).Where(x => (string)x != "").ToArray();

                        var monthWrap = month.SplitByMonth().ToArray();

                        enu.MoveNext();
                        while (true)
                        {
                            c = enu.GetCurrent();

                            var valrecords = c.Skip(1).Take(month.Length).ToArray();

                            for (int i = 0; i < valrecords.Length; i++)
                            {
                                yield return new Record
                                {
                                    Time = DateTime.Now, // must use mx var
                                    NameType = t,
                                    DeviceId = type,
                                    Value = (double)Convert.ChangeType(valrecords[i].Value, TypeCode.Double)
                                };

                            }

                            //enu.MoveNext();
                            //c = enu.GetCurrent();
                            //if (c.Values.AreAllEmpty())

                            //return first for presentation only
                            while (!c.Values.AreAllEmpty())
                            {
                                enu.MoveNext();
                                c = enu.GetCurrent();
                            }

                            break;
                        }
                    }
            }
        }
    }
}

