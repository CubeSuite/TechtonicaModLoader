using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MyClasses.ThunderStoreResponses
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
        public List<ThunderStoreVerion> versions = new List<ThunderStoreVerion>();
    }

    public class ThunderStoreVerion
    {
        public string name;
        public string full_name;
        public string description;
        public string icon;
        public string version_number;
        public List<string> dependencies = new List<string>();
        public string download_url;
        public int downloads;
        public string date_created;
        public string website_url;
        public bool is_active;
        public int file_size;
    }
}
