using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.Services.ThunderstoreModels
{
    public class ThunderStoreMod
    {
        public string name;
        public string full_name;
        public string owner;
        public string package_url;
        public string donation_link;
        public string date_created;
        public string date_updated;
        public string uuid4;
        public int rating_score;
        public bool is_deprecated;
        public List<string> categories = new List<string>();
        public List<ThunderStoreVersion> versions = new List<ThunderStoreVersion>();

        // Public Functions

        public bool PassesFilterChecks() {
            if (is_deprecated) return false;
            if (Thunderstore.allowedMods.Contains(name)) return true;
            if (Thunderstore.disallowedMods.Contains(name)) return false;
            if (DateTime.Parse(date_updated) < new DateTime(2024, 11, 7)) return false;

            return true;
        }
    }
}
