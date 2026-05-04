using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AuthApp.Services
{
    public class KatoEntry
    {
        public string Code { get; set; } = "";
        public string Ab { get; set; } = "";
        public string Cd { get; set; } = "";
        public string Ef { get; set; } = "";
        public string Hij { get; set; } = "";
        public string Level { get; set; } = "";
        public string KazName { get; set; } = "";
        public string RusName { get; set; } = "";
    }

    public class KatoService
    {
        private readonly List<KatoEntry> _entries;

        
        private const string VKO_AB = "63";

        public KatoService(string csvPath)
        {
            _entries = LoadCsv(csvPath);
        }

        private List<KatoEntry> LoadCsv(string path)
        {
            var result = new List<KatoEntry>();
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var p = line.Split(';');
                if (p.Length < 8) continue;
                result.Add(new KatoEntry
                {
                    Code = p[0],
                    Ab = p[1],
                    Cd = p[2],
                    Ef = p[3],
                    Hij = p[4],
                    Level = p[5],
                    KazName = p[6],
                    RusName = p[7]
                });
            }
            return result;
        }

      
        public List<KatoEntry> GetDistricts()
        {
            return _entries
                .Where(e => e.Ab == VKO_AB && e.Cd != "00"
                         && e.Ef == "00" && e.Hij == "000")
                .OrderBy(e => e.RusName)
                .ToList();
        }

        
        public List<KatoEntry> GetRuralDistricts(string districtCd)
        {
            return _entries
                .Where(e => e.Ab == VKO_AB && e.Cd == districtCd
                         && e.Ef != "00" && e.Hij == "000")
                .OrderBy(e => e.RusName)
                .ToList();
        }

     
        public List<KatoEntry> GetSettlements(string districtCd, string ruralCd)
        {
            return _entries
                .Where(e => e.Ab == VKO_AB && e.Cd == districtCd
                         && e.Ef == ruralCd && e.Hij != "000")
                .OrderBy(e => e.RusName)
                .ToList();
        }

      
        public List<KatoEntry> GetCitySettlements(string districtCd)
        {
            return _entries
                .Where(e => e.Ab == VKO_AB && e.Cd == districtCd
                         && e.Ef == "00" && e.Hij != "000")
                .OrderBy(e => e.RusName)
                .ToList();
        }
    }
}