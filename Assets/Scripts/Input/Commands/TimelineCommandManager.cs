using System;
using System.Collections.Generic;
using RhythmEngine;
using UI;
using UnityEngine;

namespace DefaultNamespace.Input
{
    public class TimelineCommandManager : MonoBehaviour
    {
        private Stack<TimelineCommand> _commands;
        private Stack<TimelineCommand> _undoStack;

        private void Awake()
        {
            _commands = new Stack<TimelineCommand>();
            _undoStack = new Stack<TimelineCommand>();
        }

        public void ApplyCommand(TimelineCommand c, TimelineUI ui, EditorEngine engine, bool redo = false)
        {
            if(redo) c.Redo(ui, engine);
            else c.Apply(ui, engine);
            _commands.Push(c);
            if(!redo) _undoStack.Clear();
        }

        public TimelineCommand GetLastCommand()
        {
            return _commands.Peek();
        }

        public void UndoCommand(TimelineUI ui, EditorEngine engine)
        {
            if (_commands.Count == 0)
            {
                Debug.LogWarning("No command to undo.");
                return;
            }
            var lastCommand = _commands.Pop();
            lastCommand.Undo(ui, engine);
            _undoStack.Push(lastCommand);
        }

        public void RedoCommand(TimelineUI ui, EditorEngine engine)
        {
            if (_undoStack.Count == 0)
            {
                Debug.LogWarning("No command to redo.");
                return;
            }
            var lastUndoCommand = _undoStack.Pop();
            ApplyCommand(lastUndoCommand, ui, engine, true);
        }

        public void Clear()
        {
            _commands.Clear();
        }
    }
}