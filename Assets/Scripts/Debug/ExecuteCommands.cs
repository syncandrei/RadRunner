using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    public class QuitCommands : ConsoleCommand
    {
        public override string Action { get; protected set; }
        public override string What { get; protected set; }
        public override string Amount { get; protected set; }

        public QuitCommands()
        {
            Action = "Quit";
            What = "quit";
            Amount = "Quits the application";

            AddCommandToConsole();
        }

        public override void RunCommand()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }

        public static QuitCommands CreateCommand()
        {
            return new QuitCommands();
        }
    }

    public class ScoreCommand : ConsoleCommand
    {
        int devScore;

        public override string Action { get; protected set; }
        public override string What { get; protected set; }
        public override string Amount { get; protected set; }

        public ScoreCommand()
        {
            Action = "Score";
            What = "player give score " + devScore;
            Amount = "This gives cohones to player during the track";

            AddCommandToConsole();
        }

        public override void RunCommand()
        {
            GameManager.Instance.AddScore(devScore);
        }

        public static ScoreCommand CreateCommand()
        {
            return new ScoreCommand();
        }
    }
}
