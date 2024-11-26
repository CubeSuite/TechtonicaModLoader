using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using TechtonicaModLoader.Stores;
using CommunityToolkit.Mvvm.Input;

namespace TechtonicaModLoader.MVVM.Settings.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        // Members

        private Setting<bool>? _boolSetting = null;
        private Setting<string>? _stringSetting = null;
        private EnumSetting<ModListSortOption>? _modListSortSetting = null;
        private EnumSetting<ModListSource>? _modListSourceSetting = null;

        private UserSettings _userSettings;

        // Properties

        // Normal settings

        [ObservableProperty] private Type _type;
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _description;
        [ObservableProperty] private object? _value;

        // Button settings
        [ObservableProperty] private string _buttonText = "";
        [ObservableProperty] private Action<UserSettings>? _onClick = null;

        // Enum settings
        [ObservableProperty] private Array? _enumOptions;

        // Constructors

        public SettingViewModel(Setting<bool> boolSetting, UserSettings userSettings) {
            _userSettings = userSettings;
            _name = boolSetting.Name;
            _description = boolSetting.Description;
            _value = boolSetting.Value;
            _type = typeof(bool);

            _boolSetting = boolSetting;
        }

        public SettingViewModel(Setting<string> stringSetting, UserSettings userSettings) {
            _userSettings = userSettings;
            _name = stringSetting.Name;
            _description = stringSetting.Description;
            _value = stringSetting.Value;
            _type = typeof(string);

            _stringSetting = stringSetting;
        }

        public SettingViewModel(ButtonSetting buttonSetting, UserSettings userSettings) {
            _userSettings = userSettings;
            _name = buttonSetting.Name;
            _description = buttonSetting.Description;
            _buttonText = buttonSetting.ButtonText;
            _onClick = buttonSetting.OnClick;
            _type = typeof(Action);
            _value = null;
        }

        public SettingViewModel(EnumSetting<ModListSortOption> modListSortSetting, UserSettings userSettings) {
            _userSettings = userSettings;
            _name = modListSortSetting.Name;
            _description = modListSortSetting.Description;
            _modListSortSetting = modListSortSetting;
            _value = modListSortSetting.Value;
            _enumOptions = modListSortSetting.Options;
            _type = typeof(ModListSortOption);
        }

        public SettingViewModel(EnumSetting<ModListSource> modListSourceSetting, UserSettings userSettings) {
            _userSettings = userSettings;
            _name = modListSourceSetting.Name;
            _description = modListSourceSetting.Description;
            _modListSourceSetting = modListSourceSetting;
            _value = modListSourceSetting.Value;
            _enumOptions = modListSourceSetting.Options;
            _type = typeof(ModListSource);
        }

        // Events

        partial void OnValueChanged(object? value) {
            if (value == null) return;

            if (Type == typeof(bool) && _boolSetting != null) _boolSetting.Value = (bool)value;
            else if (Type == typeof(string) && _stringSetting != null) _stringSetting.Value = (string)value;
            else if (Type == typeof(ModListSortOption) && _modListSortSetting != null) _modListSortSetting.Value = (ModListSortOption)value;
            else if (Type == typeof(ModListSource) && _modListSourceSetting != null) _modListSourceSetting.Value = (ModListSource)value;
        }

        // Commands

        [RelayCommand]
        private void ExecuteButtonAction() {
            OnClick?.Invoke(_userSettings);
        }
    }

    public class SettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? BoolTemplate { get; set; }
        public DataTemplate? StringTemplate { get; set; }
        public DataTemplate? ButtonTemplate { get; set; }
        public DataTemplate? EnumTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container) {
            if (item is SettingViewModel viewModel) {
                if (viewModel.Type == typeof(bool)) return BoolTemplate;
                if (viewModel.Type == typeof(string)) return StringTemplate;
                if (viewModel.Type == typeof(Action)) return ButtonTemplate;
                if (viewModel.Type == typeof(ModListSortOption)) return EnumTemplate;
                if (viewModel.Type == typeof(ModListSource)) return EnumTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
