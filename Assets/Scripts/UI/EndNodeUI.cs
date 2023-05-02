using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class EndNodeUI : EventNodeBase, ISelectorInteractor
    {
        public override void Select(SelectInfo info, Vector2 pos, bool special = false)
        {
            
        }

        public override void Click(SelectInfo info, Vector2 pos)
        {
            base.Click(info, pos);
        }

        public override void Move(SelectInfo info, Vector2 pos)
        {
            if (Parent.ParentUI.Toolbar.ActiveOption != ToolbarOption.Select) return;
            Parent.RequestExtension(info, pos);
        }

        public override void Place(SelectInfo info, Vector2 pos)
        {
            base.Place(info, pos);
        }

        public override void RightClicked(SelectInfo info, Vector2 pos)
        {
            Parent.RightClicked(info, pos);
        }
        
    }
}