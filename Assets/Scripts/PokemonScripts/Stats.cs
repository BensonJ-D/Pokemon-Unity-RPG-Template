namespace PokemonScripts
{
    public class Stats
    {
        public int MaxHp;
        public int Attack;
        public int Defence;
        public int SpAttack;
        public int SpDefence;
        public int Speed;

        public static Stats operator -(Stats b, Stats a)
        {
            return new Stats
            {
                MaxHp = b.MaxHp - a.MaxHp,
                Attack = b.Attack - a.Attack,
                Defence = b.Defence - a.Defence,
                SpAttack = b.SpAttack - a.SpAttack,
                SpDefence = b.SpDefence - a.SpDefence,
                Speed = b.Speed - a.Speed
            };
        } 
    }
}