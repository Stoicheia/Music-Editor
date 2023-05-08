using RhythmEngine;
using UI;
using UnityEngine;

namespace DefaultNamespace.Input
{
    /// <summary>
    /// Unimplemented.
    /// </summary>
    public class ClearAllCommand : TimelineCommand
    {
        private LevelData _cachedLevelData;

        public ClearAllCommand(EditorEngine engine)
        {
            _cachedLevelData = engine.LevelData;
        }
        public override void Apply(TimelineUI ui, EditorEngine engine)
        {
            foreach (var node in ui.EventToNode.Values)
            {
                ui.EventNodePool.Enqueue(node);
            }
            
            ui.EventToNode.Clear();
            engine.ClearLevelData();
        }

        public override void Undo(TimelineUI ui, EditorEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}