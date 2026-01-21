using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MarioArcade.Objects;

namespace MarioArcade.Managers
{
    public class GameLogicManager
    {
        private Player _player;
        private List<GameObject> _gameObjects;
        private Action<int> _onScoreChanged;
        private bool _isPaused;

        public GameLogicManager(Player player, List<GameObject> gameObjects, Action<int> onScoreChanged)
        {
            _player = player;
            _gameObjects = gameObjects;
            _onScoreChanged = onScoreChanged;
        }

        public void SetPausedState(bool paused)
        {
            _isPaused = paused;
        }

        public void ProcessGameLogic()
        {
            if (_isPaused) return;

            _onScoreChanged?.Invoke(_player.Score);
        }
    }
}