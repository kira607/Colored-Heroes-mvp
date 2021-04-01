
[System.Serializable]
public class FlippedChips
{
    public Chip left;
    public Chip right;

    public FlippedChips(Chip l, Chip r)
    {
        left = l;
        right = r;
    }

    public Chip GetOtherChip(Chip chip)
    {
        if (chip == left)
            return right;
        if (chip == right)
            return left;
        return null;
    }
}