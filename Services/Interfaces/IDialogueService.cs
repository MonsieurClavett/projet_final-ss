using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Services.Interfaces
{
    public interface IDialogueService
    {
        void AfficherMessageErreur(string message, string titre);
        void AfficherMessageInfo(string message, string titre);
        void AfficherMessageAvertissement(string message, string titre);
        bool PoserQuestion(string question, string titre);
    }
}

