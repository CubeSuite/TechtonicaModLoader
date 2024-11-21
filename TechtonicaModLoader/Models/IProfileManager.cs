using System.Collections.ObjectModel;
using System.ComponentModel;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.Models
{
    public interface IProfileManager : INotifyPropertyChanged, INotifyPropertyChanging
    {
        Profile ActiveProfile { get; set; }

        ObservableCollection<Profile> ProfilesList { get; }

        void CreateNewProfile(string name);

        void DeleteActiveProfile();

        void Load();
    }
}
