using UnityEngine;

namespace Tulip.Core
{
    public class GameState : ScriptableObject
    {
        public static GameState Empty { get; private set; }
        public static LoadingGameState Loading { get; protected set; }
        public static MainMenuGameState MainMenu { get; protected set; }
        public static PlayingGameState Playing { get; protected set; }
        public static PausedGameState Paused { get; protected set; }

        public virtual bool RequestApplicationQuit() => true;

        protected virtual void OnEnable() => Empty = this;
    }
}
