namespace Roleplay
{
    public class CreatureSheet
    {
        public int[] maxhp;
        public int[] hp;
        public bool[] isActive;

        public string[] names;
        public int[] IDs;
        public int[][] tIDs;
        public string[][] tuses;

        public CreatureSheet(int[] hp_, int[] maxHp_, string[] names_, bool[] isActive_, int[][] tIDs_, string[][] tuses_, int[] IDs_)
        {
            IDs = IDs_;
            tuses = tuses_;
            tIDs = tIDs_;
            maxhp = maxHp_;
            hp = hp_;
            names = names_;
            isActive = isActive_;
        }
    }
}
