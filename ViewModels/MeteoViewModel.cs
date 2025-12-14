using Final.DataService.Repositories.Interfaces;
using Final.Models;
using Final.ViewModels.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Final.ViewModels
{
    public class MeteoViewModel : BaseViewModel
    {
        private readonly IRegionRepository _regionRepository;

        public ObservableCollection<Region> Regions { get; set; }

        private Region? _regionSelectionnee;
        public Region? RegionSelectionnee
        {
            get => _regionSelectionnee;
            set
            {
                _regionSelectionnee = value;
                OnPropertyChanged();

                // Remplir les champs quand on sélectionne
                Nom = _regionSelectionnee?.Nom ?? "";
                Latitude = _regionSelectionnee?.Latitude.ToString(CultureInfo.InvariantCulture) ?? "";
                Longitude = _regionSelectionnee?.Longitude.ToString(CultureInfo.InvariantCulture) ?? "";

                ModeAjout = _regionSelectionnee == null;
            }
        }

        private bool _modeAjout = true;
        public bool ModeAjout
        {
            get => _modeAjout;
            set { _modeAjout = value; OnPropertyChanged(); }
        }

        private string _nom = "";
        public string Nom
        {
            get => _nom;
            set { _nom = value; OnPropertyChanged(); }
        }

        private string _latitude = "";
        public string Latitude
        {
            get => _latitude;
            set { _latitude = value; OnPropertyChanged(); }
        }

        private string _longitude = "";
        public string Longitude
        {
            get => _longitude;
            set { _longitude = value; OnPropertyChanged(); }
        }

        public RelayCommand CommandeNouvelleRegion { get; }
        public AsyncCommand CommandeAjouter { get; }
        public AsyncCommand CommandeSupprimer { get; }

        public MeteoViewModel(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;

            Regions = new ObservableCollection<Region>(_regionRepository.GetAll());
            RegionSelectionnee = Regions.FirstOrDefault();

            CommandeNouvelleRegion = new RelayCommand(_ => NouvelleRegion(), null);
            CommandeAjouter = new AsyncCommand(AjouterAsync, _ => CanAjouter());
            CommandeSupprimer = new AsyncCommand(SupprimerAsync, _ => RegionSelectionnee != null);
        }

        private void NouvelleRegion()
        {
            RegionSelectionnee = null;
            Nom = "";
            Latitude = "";
            Longitude = "";
            ModeAjout = true;
        }

        private bool CanAjouter() => true; // bouton + toujours dispo


        private async Task AjouterAsync(object? _)
        {
            // Parse double (accepte virgule ou point)
            if (!TryParseDouble(Latitude, out var lat) || !TryParseDouble(Longitude, out var lon))
            {
                MessageBox.Show("Latitude/Longitude invalides (ex: 46.57, -72.73).",
                    "Valeurs invalides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
            {
                MessageBox.Show("Latitude doit être entre -90 et 90, Longitude entre -180 et 180.",
                    "Valeurs hors limite", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }



            var nomClean = (Nom ?? "").Trim();

            if (Regions.Any(r => string.Equals(r.Nom, nomClean, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show(
                    Properties.traduction.Msg_Region_Existe,
                    Properties.traduction.Msg_Titre_Info,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var region = new Region
            {
                Nom = nomClean,
                Latitude = lat,
                Longitude = lon
            };

            try
            {
                await _regionRepository.AddAsync(region);

                // Ajout en mémoire (UI)
                Regions.Add(region);

                // Optionnel : garder l’ordre alphabétique
                ReorderRegions();

                Nom = "";
                Latitude = "";
                Longitude = "";

                MessageBox.Show("Région ajoutée!", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;

                if (msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("constraint", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Une région avec ce nom existe déjà.",
                        "Nom déjà utilisé", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(msg, "Erreur BD", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SupprimerAsync(object? _)
        {
            if (RegionSelectionnee == null)
                return;

            // Optionnel: empêcher la suppression du seed
            if (RegionSelectionnee.Id == 1)
            {
                MessageBox.Show("Impossible de supprimer la région par défaut.",
                    "Action refusée", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var question = string.Format(
                Properties.traduction.Msg_Confirm_SupprimerRegion,
                RegionSelectionnee!.Nom
            );

            if (MessageBox.Show(question,Properties.traduction.Msg_Titre_Info,
                MessageBoxButton.YesNo,MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                var toDelete = RegionSelectionnee;
                await _regionRepository.DeleteAsync(toDelete);

                Regions.Remove(toDelete);
                RegionSelectionnee = Regions.FirstOrDefault();
                MessageBox.Show("Région supprimée.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool TryParseDouble(string s, out double value)
        {
            s = (s ?? "").Trim();
            // accepte virgule ou point
            s = s.Replace(',', '.');
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private void ReorderRegions()
        {
            var ordered = Regions.OrderBy(r => r.Nom).ToList();
            Regions.Clear();
            foreach (var r in ordered) Regions.Add(r);
        }
    }
}
