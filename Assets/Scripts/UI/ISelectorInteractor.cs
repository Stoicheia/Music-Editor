using UnityEngine;

namespace UI
{
    public interface ISelectorInteractor
    {
        public void Select(SelectInfo info, Vector2 pos, bool special = false);
        public void Click(SelectInfo info, Vector2 pos);
        public void Move(SelectInfo info, Vector2 pos);
        public void Place(SelectInfo info, Vector2 pos);
        public void RightClicked(SelectInfo info, Vector2 pos);
    }
}