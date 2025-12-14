using Final.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Final.ViewModels.Commands;
using System.Windows.Input;

namespace Final.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        public Action? RequestClose { get; set; }

        private string _token = string.Empty;
        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                OnPropertyChanged(nameof(Token));
            }
        }

        // (optionnel) langue, comme ton UI
        private string? _selectedLanguage;
        public string? SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ConfigurationViewModel(Action requestClose)
        {
            RequestClose = requestClose;

            SaveCommand = new RelayCommand(Save, null);
            CancelCommand = new RelayCommand(Cancel, null);

            // TODO plus tard: charger via config service / settings
            // Token = ...
        }

        private void Save(object? _)
        {
            // TODO plus tard: sauver token/langue via Configuration service
            RequestClose?.Invoke();
        }

        private void Cancel(object? _)
        {
            RequestClose?.Invoke();
        }
    }
}
