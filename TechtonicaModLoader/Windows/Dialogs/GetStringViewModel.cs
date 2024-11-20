using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.Windows.Dialogs
{
    public partial class GetStringViewModel : ObservableObject
    {
        // Properties

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _input;

        public Action<bool?>? CloseDialogAction { get; set; } = null;

        // Constructors

        public GetStringViewModel(string title, string input) {
            _title = title;
            _input = input;
        }

        // Commands

        [RelayCommand]
        private void Confirm() {
            CloseDialogAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel() {
            CloseDialogAction?.Invoke(false);
        }
    }
}
