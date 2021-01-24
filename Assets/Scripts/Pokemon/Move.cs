namespace Pokemon
{
    public class Move
    {
        public MoveBase Base { get; set; }
        public int Pp { get; set; }

        public Move(MoveBase pBase)
        {
            Base = pBase;
            Pp = pBase.Pp;
        }

    }
}