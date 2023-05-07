using System;
using DefaultNamespace.Input;
using RhythmEngine;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimelineCommandsUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TimelineCommandManager _commandManager;
        [SerializeField] private TimelineUI _timeline;
        [SerializeField] private EditorEngine _engine;
        [Header("Buttons")]
        [SerializeField] private Button _undoButton;
        [SerializeField] private Button _redoButton;

        private void Start()
        {
            _undoButton.onClick.AddListener(InvokeUndo);
            _redoButton.onClick.AddListener(InvokeRedo);
        }

        private void InvokeUndo()
        {
            _commandManager.UndoCommand(_timeline, _engine);
        }

        private void InvokeRedo()
        {
            _commandManager.RedoCommand(_timeline, _engine);
        }
    }
}