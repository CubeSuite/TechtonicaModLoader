using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.Services.ThunderstoreModels
{
    public class ThunderStoreVersion
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
