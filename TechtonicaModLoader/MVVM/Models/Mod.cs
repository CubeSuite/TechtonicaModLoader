using Accessibility;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Services.ThunderstoreModels;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Models
{
    public class Mod
    {
        // Members
        private readonly string? _id;
        private readonly string? _name;
        private readonly string? _fullName;
        private readonly string? _tagline;
        private readonly DateTime? _dateUploaded;
        private readonly DateTime? _lastUpdated;
        private readonly int? _downloads;
        private readonly int? _rating;
        private readonly ModVersion? _version;
        
        private readonly string? _link;
        private readonly string? _iconLink;
        private readonly string? _donationLink;

        private readonly bool? _isDownloaded;
        private readonly bool? _updateAvailable;
        private readonly bool? _hasConfigFile;

        private readonly List<Mod> dependencies = new List<Mod>();

        private IThunderStore thunderStore;
        private IProfileManager profileManager;

        // Properties

        public string ID => _id ?? "";
        public string Name => _name ?? "Unknown Mod";
        public string FullName => _fullName ?? "Unknown Mod";
        public string TagLine => _tagline ?? "Description of mod unavailable";
        public DateTime DateUploaded => _dateUploaded ?? new DateTime();
        public DateTime LastUpdated => _lastUpdated ?? new DateTime();
        public int Downloads => _downloads ?? 0;
        public int Rating => _rating ?? 0;
        public ModVersion Version => _version ?? new ModVersion(1, 0, 0);

        public string Link => _link ?? "https://thunderstore.io/c/techtonica/";
        public string IconLink => _iconLink ?? ""; // ToDo: default icon link
        public string DonationLink => _donationLink ?? "";
        
        public bool IsDownloaded => _isDownloaded ?? false;
        
        public bool IsEnabled => profileManager.ActiveProfile.IsModEnabled(this);
        public bool UpdateAvailable => _updateAvailable ?? false;
        public bool HasConfigFile => _hasConfigFile ?? false;

        private bool _isDownloading = false;
        public event Action? IsDownloadingChanged;
        public bool IsDownloading {
            get => _isDownloading;
            set {
                _isDownloading = value;
                IsDownloadingChanged?.Invoke();
            }
        }

        // Constructors

        public Mod(ThunderStoreMod thunderStoreMod, IThunderStore thunderStore, IProfileManager profileManager) {
            this.thunderStore = thunderStore;
            this.profileManager = profileManager;

            _id = thunderStoreMod.uuid4;
            _name = thunderStoreMod.name;
            _fullName = thunderStoreMod.versions[0].full_name;
            _tagline = thunderStoreMod.versions[0].description;
            _downloads = thunderStoreMod.GetNumDownloads();
            _rating = thunderStoreMod.rating_score;
            _version = ModVersion.Parse(thunderStoreMod.versions[0].version_number);

            _link = thunderStoreMod.package_url;
            _iconLink = thunderStoreMod.versions[0].icon;
            _donationLink = thunderStoreMod.donation_link;
            
            _isDownloaded = thunderStore.IsModDownloaded(_id, Version);

            foreach(string dependency in thunderStoreMod.versions[0].dependencies) {
                if (dependency.Contains("BepInEx")) continue;

                if(thunderStore.SearchForMod(dependency, out ThunderStoreMod? mod) && mod != null) {
                    dependencies.Add(new Mod(mod, thunderStore, profileManager));
                }
                else {
                    string error = $"Failed to find dependency '{dependency}'";
                    Log.Error(error);
                    DebugUtils.CrashIfDebug(error);
                }
            }

            if(DateTime.TryParse(thunderStoreMod.date_updated, out DateTime lastUpdated)) {
                _lastUpdated = lastUpdated;
            }
            else {
                Log.Warning($"Couldn't parse {this}'s date_updated member ({thunderStoreMod.date_updated ?? "null"})");
            }

            if (DateTime.TryParse(thunderStoreMod.date_created, out DateTime dateUploaded)) {
                _dateUploaded = dateUploaded;
            }
            else {
                Log.Warning($"Couldn't parse {this}'s date_uploaded member ({thunderStoreMod.date_created ?? "null"})");
            }
        }

        // Public Functions

        public void Download() {
            foreach(Mod dependency in dependencies) {
                if (thunderStore.IsModDownloaded(dependency.ID, dependency.Version)) continue;
                thunderStore.DownloadMod(dependency.FullName);
            }

            thunderStore.DownloadMod(FullName);
        }

        // Overrides

        public override string ToString() {
            return $"Mod '{Name}' - {ID}";
        }
    }
}
