using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.States
{
    class IntroState : State
    {
        private Game game;

        public IntroState (Game game)
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
            throw new InvalidOperationException("Must be in Detect state to detect colors");
        }

        public void Restart()
        {
            game.DisplayIntro();
            game.SetState(game.Intro);
        }

        public void ShowResults()
        {
            throw new InvalidOperationException("No game has started to have results");
        }

        public void StartGame()
        {
            game.SetState(game.Running);
        }
    }
}
