namespace UI
{
    public struct SelectInfo
    {
        public ISelectorInteractor Selected;

        public SelectInfo(ISelectorInteractor s)
        {
            Selected = s;
        }
    }
}