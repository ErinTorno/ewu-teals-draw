using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.States
{
    class DetectState : State
    {
        private Game game;

        public DetectState (Game game) 
        {
            this.game = game;
        }

        public void DetectColor()
        {
            throw new InvalidOperationException("Already detecting colors");
        }

        public void End()
        {
            game.Exit();
            game.SetState(game.End);
        }

        public void PlayerPrompt()
        {
            game.PromptAddPlayer();
            game.SetState(game.Ready);
        }

        public void Restart()
        {
            game.DisplayIntro();
            game.SetState(game.Intro);
        }

        public void ShowResults()
        {
            throw new InvalidOperationException("There are no results to show yet");
        }

        public void StartGame()
        {
            throw new InvalidOperationException("Needs to check if the user wants to add more players first");
        }
    }
}
