namespace MatchBoard
{
    public class Cell
    {
        private Chip _chip;
        public readonly Point index;
        public ChipColor color;

        public Cell(ChipColor color, Point index)
        {
            this.color = color;
            this.index = index;
        }

        public bool IsPlayable()
        {
            return !(color == ChipColor.Blank || color == ChipColor.Hole);
        }

        public void SetChip(Chip chip)
        {
            _chip = chip;
            color = (chip == null) ? ChipColor.Blank : chip.color;
            if (chip == null) return;
            chip.SetIndex(index);
        }

        public Chip GetChip()
        {
            return _chip;
        }

    }
}