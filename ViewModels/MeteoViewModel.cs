using Final.DataService.Repositories.Interfaces;
using Final.Models;
using Final.Models.Weatherbit;
using Final.Services;
using Final.Services.Interfaces;
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Final.ViewModels
{
    public class MeteoViewModel : BaseViewModel
    {
        private readonly IRegionRepository _regionRepository;

        private readonly IWeatherService _weatherService;

        private readonly IDialogueService _dialogue;

        public ObservableCollection<WeatherDay> Previsions { get; } = new();

        public ObservableCollection<Region> Regions { get; set; }

        private Region? _regionSelectionnee;
        public Region? RegionSelectionnee
        {
            get => _regionSelectionnee;
            set
            {
                _regionSelectionnee = value;
                OnPropertyChanged();



                ModeAjout = _regionSelectionnee == null;
                _ = ChargerPrevisionsAsync(null);


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

        public MeteoViewModel(IRegionRepository regionRepository, IWeatherService weatherService, IDialogueService dialogue)
        {
            _regionRepository = regionRepository;
            _weatherService = weatherService;
            _dialogue = dialogue;

            Regions = new ObservableCollection<Region>(_regionRepository.GetAll());
            RegionSelectionnee = null;

            CommandeNouvelleRegion = new RelayCommand(_ => NouvelleRegion(), null);
            CommandeAjouter = new AsyncCommand(AjouterAsync, _ => CanAjouter());
            CommandeSupprimer = new AsyncCommand(SupprimerAsync, _ => RegionSelectionnee != null);
            CommandeChargerPrevisions = new AsyncCommand(ChargerPrevisionsAsync, _ => RegionSelectionnee != null);

        }


        private void NouvelleRegion()
        {
            RegionSelectionnee = null;
            Nom = "";
            Latitude = "";
            Longitude = "";
            ModeAjout = true;
        }

        private bool CanAjouter() => true; 


        public async Task AjouterAsync(object? _)
        {
            



            var nomClean = (Nom ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nomClean))
            {
                _dialogue.AfficherMessageAvertissement(
                    Properties.traduction.Msg_Nom_Region_Invalide,
                    Properties.traduction.Msg_Titre_Valeurs_Invalides
                );
                return;
            }

            if (!TryParseDouble(Latitude, out var lat) || !TryParseDouble(Longitude, out var lon))
            {


                _dialogue.AfficherMessageAvertissement(
                    Properties.traduction.Msg_LatLon_Invalide,
                    Properties.traduction.Msg_Titre_Valeurs_Invalides
                );


                return;
            }

            if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
            {


                _dialogue.AfficherMessageAvertissement(
                    Properties.traduction.Msg_LatLon_Limites,
                    Properties.traduction.Msg_Titre_Valeurs_HorsLimite
                );

                return;
            }

            if (Regions.Any(r => string.Equals(r.Nom, nomClean, StringComparison.OrdinalIgnoreCase)))
            {


                _dialogue.AfficherMessageAvertissement(
                    Properties.traduction.Msg_Region_Existe,
                    Properties.traduction.Msg_Titre_Nom_DejaUtilise
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

                Regions.Add(region);

                ReorderRegions();

                Nom = "";
                Latitude = "";
                Longitude = "";



                _dialogue.AfficherMessageInfo(
                    Properties.traduction.Msg_Region_Ajoutee,
                    Properties.traduction.Msg_Titre_Info
                );


            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;

                if (msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("constraint", StringComparison.OrdinalIgnoreCase))
                {


                    _dialogue.AfficherMessageAvertissement(
                        Properties.traduction.Msg_Region_Existe,
                        Properties.traduction.Msg_Titre_Nom_DejaUtilise
                    );

                }
                else
                {


                    _dialogue.AfficherMessageErreur(
                        msg,
                        Properties.traduction.Msg_Titre_Erreur_BD
                    );

                }
            }
            catch (Exception ex)
            {
                _dialogue.AfficherMessageErreur(
                    ex.Message,
                    Properties.traduction.Msg_Titre_Erreur
                );
            }
        }

        public Task ChargerPrevisions() => ChargerPrevisionsAsync(null);

        public async Task SupprimerAsync(object? _)
        {
            if (RegionSelectionnee == null)
                return;

            if (RegionSelectionnee.Id == 1)
            {


                _dialogue.AfficherMessageAvertissement(
                    Properties.traduction.Msg_Region_Defaut_ImpossibleSupprimer,
                    Properties.traduction.Msg_Titre_Action_Refusee
                );

                return;
            }

            var question = string.Format(
                Properties.traduction.Msg_Confirm_SupprimerRegion,
                RegionSelectionnee!.Nom
            );

            if (_dialogue.PoserQuestion(
                question,
                Properties.traduction.Msg_Titre_Info
            ) == false)
            {
                return;
            }

            try
            {
                var toDelete = RegionSelectionnee;
                await _regionRepository.DeleteAsync(toDelete);

                Regions.Remove(toDelete);
                RegionSelectionnee = Regions.FirstOrDefault();

                _dialogue.AfficherMessageInfo(
                    Properties.traduction.Msg_Region_Supprimee,
                    Properties.traduction.Msg_Titre_Info
                );

            }
            catch (Exception ex)
            {
                _dialogue.AfficherMessageErreur(
                    ex.Message,
                    Properties.traduction.Msg_Titre_Erreur
                );
            }
        }

        private static bool TryParseDouble(string s, out double value)
        {
            s = (s ?? "").Trim();
            s = s.Replace(',', '.');
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private void ReorderRegions()
        {
            var ordered = Regions.OrderBy(r => r.Nom).ToList();
            Regions.Clear();
            foreach (var r in ordered) Regions.Add(r);
        }

        private async Task ChargerPrevisionsAsync(object? _)
        {
            if (RegionSelectionnee == null) return;

            var token = Properties.Settings.Default.tokenWeatherbit ?? "";
            var lang = Properties.Settings.Default.langue ?? "fr-CA";
            var apiLang = lang.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? "en" : "fr";

            if (string.IsNullOrWhiteSpace(token))
            {
                

                _dialogue.AfficherMessageAvertissement(
                    Properties.traduction.Msg_Token_Invalide,
                    Properties.traduction.Msg_Titre_Erreur
                );
                return;
            }

            try
            {
                var jours = await _weatherService.Get7DaysAsync(RegionSelectionnee, token, apiLang);

                Previsions.Clear();
                foreach (var j in jours)
                    Previsions.Add(j);

            }
            catch (Exception ex)
            {


                _dialogue.AfficherMessageErreur(
                    ex.Message,
                    Properties.traduction.Msg_Titre_Erreur
                );
            }
        }

        public AsyncCommand CommandeChargerPrevisions { get; }

        



    }
}
