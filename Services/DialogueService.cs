using Final.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Final.Services
{
    public class DialogueService : IDialogueService
    {
        public void AfficherMessageErreur(string message, string titre) =>
            MessageBox.Show(message, titre, MessageBoxButton.OK, MessageBoxImage.Error);

        public void AfficherMessageInfo(string message, string titre) =>
            MessageBox.Show(message, titre, MessageBoxButton.OK, MessageBoxImage.Information);

        public void AfficherMessageAvertissement(string message, string titre) =>
            MessageBox.Show(message, titre, MessageBoxButton.OK, MessageBoxImage.Warning);

        public bool PoserQuestion(string question, string titre) =>
            MessageBox.Show(question, titre, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}