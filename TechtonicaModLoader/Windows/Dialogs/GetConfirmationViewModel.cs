using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.Windows.Dialogs
{
    public partial class GetConfirmationViewModel : ObservableObject
    {
        // Properties

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _description;

        // Constructors

        public GetConfirmationViewModel(string title, string description) {
            _title = title;
            _description = description;
        }
    }
}
