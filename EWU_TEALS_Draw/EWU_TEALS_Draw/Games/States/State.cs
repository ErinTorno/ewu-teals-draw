using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.States
{
    interface State
    {
        void DetectColor();
        void PlayerPrompt();
        void StartGame();
        void ShowResults();
        void Restart();
        void End();
    }
}
