using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.States
{
    class ResultState : State
    {
        private Game game;

        public ResultState (Game game)
        {
            this.game = game;
        }

        public void DetectColor()
        {
            throw new InvalidOperationException("Game has finished");
        }

        public void End()
        {
            game.Exit();
            game.SetState(game.End);
        }

        public void PlayerPrompt()
        {
            throw new InvalidOperationException("Game has finished");
        }

        public void Restart()
        {
            game.DisplayIntro();
            game.SetState(game.Intro);
        }

        public void ShowResults()
        {
            throw new InvalidOperationException("Already showing results");
        }

        public void StartGame()
        {
            throw new InvalidOperationException("Game has finished");
        }
    }
}
