using System.Windows.Forms;
using MarioArcade.Objects;

namespace MarioArcade.Managers
{
    public class InputManager
    {
        private Player _player;
        private bool _isPaused;

        public InputManager(Player player)
        {
            _player = player;
        }

        public void SetPausedState(bool paused)
        {
            _isPaused = paused;
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (!_isPaused)
                _player?.OnKeyDown(e);
        }

        public void HandleKeyUp(KeyEventArgs e)
        {
            if (!_isPaused)
                _player?.OnKeyUp(e);
        }

        public void ResetPlayerKeys()
        {
            _player?.ResetKeys();
        }
    }
}