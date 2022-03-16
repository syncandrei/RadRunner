using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Console
{
    public abstract class ConsoleCommand
    {
        public abstract string Action { get; protected set; }
        public abstract string What { get; protected set; }
        public abstract string Amount { get; protected set; }

        public void AddCommandToConsole()
        {
            string addMessage = " command has been added to the console.";

            DeveloperDebugConsole.AddCommandsToConsole(What, this);
            DeveloperDebugConsole.AddStaticMessageToConsole(Action + addMessage);
        }

        public abstract void RunCommand();
    }

    public class DeveloperDebugConsole : MonoBehaviour
    {
        public bool ConsoleActive = false;
        public static DeveloperDebugConsole Instance { get; private set; }
        public static Dictionary<string, ConsoleCommand> Commands { get; private set; }

        [Header("UI Components")]
        public Canvas consoleCanvas;
        public ScrollRect scrollRect;
        public Text consoleText;
        public Text inputText;
        public InputField consoleInput;

        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
            Commands = new Dictionary<string, ConsoleCommand>();
            CreateCommands();
        }

        private void Start()
        {
            consoleCanvas.gameObject.SetActive(false);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                consoleCanvas.gameObject.SetActive(!consoleCanvas.gameObject.activeInHierarchy);                
            }

            if (consoleCanvas.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if(inputText.text != "")
                    {
                        AddMessageToConsole(inputText.text);
                        ParseInput(inputText.text);
                    }
                }
            }

            if (consoleCanvas.gameObject.activeInHierarchy)
            {
                Time.timeScale = 0;
                ConsoleActive = true;
            }
            else
            {
                Time.timeScale = 1;
                ConsoleActive = false;
            }
        }

        private void CreateCommands()
        {
            QuitCommands commandQuit = QuitCommands.CreateCommand();
            ScoreCommand scoreCommand = ScoreCommand.CreateCommand();
        }

        public static void AddCommandsToConsole(string name, ConsoleCommand command)
        {
            if (!Commands.ContainsKey(name))
            {
                Commands.Add(name, command);
            }
        }

        private void AddMessageToConsole(string message)
        {
            consoleText.text += message + "\n";
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public static void AddStaticMessageToConsole(string message)
        {
            DeveloperDebugConsole.Instance.consoleText.text += message + "\n";
            DeveloperDebugConsole.Instance.scrollRect.verticalNormalizedPosition = 0f;
        }

        private void ParseInput(string input)
        {
            string[] _input = input.Split(null);

            if(_input.Length == 0 || _input == null)
            {
                AddMessageToConsole("Command not recognized.");
                return;
            }

            if (!Commands.ContainsKey(_input[0]))
            {
                AddMessageToConsole("Command not recognized.");
            }
            else
            {
                Commands[_input[0]].RunCommand();
            }
        }
    }
}