namespace Minesweeper
{
    class Cell
    {
        public DisplayState State { get; set; }
        public bool HasBomb { get; set; }
        public int Count { get; set; }
        public override string ToString()
        {
            switch (State)
            {
                case DisplayState.Hidden: return "?";
                case DisplayState.Flagged: return "!";
                case DisplayState.Shown:
                    if (HasBomb)
                        return "*";
                    return Count > 0 ? Count.ToString() : " ";
            }
            return base.ToString();
        }
    }
    enum DisplayState
    {
        Hidden,
        Flagged,
        Shown
    }
}