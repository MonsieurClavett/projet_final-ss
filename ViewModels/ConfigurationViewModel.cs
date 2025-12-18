using Final.Services;
using Final.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Final.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        public Action? RequestClose { get; set; }


        public ObservableCollection<string> Langues { get; } =
            new() { "Français", "English" };

        private string _token = "";
        public string Token
        {
            get => _token;
            set { _token = value; OnPropertyChanged(); }
        }

        private string _langueSelectionnee = "Français";
        public string LangueSelectionnee
        {
            get => _langueSelectionnee;
            set { _langueSelectionnee = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ConfigurationViewModel(Action requestClose)
        {
            RequestClose = requestClose;

            Token = Properties.Settings.Default.tokenWeatherbit ?? "";
            var code = Properties.Settings.Default.langue ?? "fr-CA";
            LangueSelectionnee = code.StartsWith("en") ? "English" : "Français";

            SaveCommand = new RelayCommand(_ => Save(), null);
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(), null);
        }

        private void Save()
        {
            Properties.Settings.Default.tokenWeatherbit = Token?.Trim() ?? "";
            Properties.Settings.Default.langue = (LangueSelectionnee == "English") ? "en-US" : "fr-CA";
            Properties.Settings.Default.Save();


            RequestClose?.Invoke();

        }
    }
}
