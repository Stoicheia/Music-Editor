using UnityEngine;

namespace UI
{
    public class ExtenderNodeUI : EventNodeBase, ISelectorInteractor
    {
        public override void Select(SelectInfo info, Vector2 pos, bool special = false)
        {
            Parent.Select(info, pos, special);
        }

        private Vector2 vecToParent = Vector2.zero;
        public override void Click(SelectInfo info, Vector2 pos)
        {
            base.Click(info, pos);
            if (Parent.ParentUI.Toolbar.ActiveOption != ToolbarOption.Select) return;
            
            Vector2 parentPos = Parent.ReferenceTransform.TransformPoint(Parent.rectTransform.anchoredPosition);
            vecToParent = parentPos - pos;
        }

        public override void Move(SelectInfo info, Vector2 pos)
        {
            if (Parent.ParentUI.Toolbar.ActiveOption != ToolbarOption.Select) return;
            
            Vector2 truePos = pos + vecToParent; 
            Parent.Move(info, truePos);
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