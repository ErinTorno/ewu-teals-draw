using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.States
{
    class ReadyState : State
    {
        private Game game;

        public ReadyState (Game game)
        {
            this.game = game;
        }

        public void DetectColor()
        {
            game.DetectColor();
            game.SetState(game.Detect);
        }

        public void End()
        {
            game.Exit();
            game.SetState(game.End);
        }

        public void PlayerPrompt()
        {
            throw new InvalidOperationException("Already prompting players to add a player or start the game");
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
            game.SetState(game.Running);
        }
    }
}
