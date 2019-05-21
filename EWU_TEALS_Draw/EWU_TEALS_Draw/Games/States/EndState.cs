using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.States
{
    class EndState : State
    {
        private Game game;

        public EndState(Game game)
        {
            this.game = game;
        }

        public void DetectColor()
        {
            throw new InvalidOperationException("Game is over");
        }

        public void End()
        {
            game.SetState(game.End);
        }

        public void PlayerPrompt()
        {
            throw new InvalidOperationException("Game is over");
        }

        public void Restart()
        {
            throw new InvalidOperationException("Game is over");
        }

        public void ShowResults()
        {
            throw new InvalidOperationException("Game is over");
        }

        public void StartGame()
        {
            throw new InvalidOperationException("Game is over");
        }
    }
}
