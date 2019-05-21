using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.States
{
    class RunningState : State
    {
        private Game game;

        public RunningState (Game game)
        {
            this.game = game;
        }

        public void DetectColor()
        {
            throw new InvalidOperationException("Game has already started");
        }

        public void End()
        {
            game.Exit();
            game.SetState(game.End);
        }

        public void PlayerPrompt()
        {
            throw new InvalidOperationException("Game has already started");
        }

        public void Restart()
        {
            game.DisplayIntro();
            game.SetState(game.Intro);
        }

        public void ShowResults()
        {
            game.DisplayResults();
            game.SetState(game.Result);
        }

        public void StartGame()
        {
            throw new InvalidOperationException("Game has already started");
        }
    }
}
