namespace PokemonScripts.Moves.Effects
{
    public class LowerTargetsAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.Attack, -1);
        }
    }
    
    public class LowerTargetsDefence : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.Defence, -1);
        }
    }

    public class LowerTargetsSpAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.SpAttack, -1);
        }
    }

    public class LowerTargetsSpDefence : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.SpDefence, -1);
        }
    }
    
    public class LowerTargetsSpeed : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.Speed, -1);
        }
    }
    
    public class LowerUsersAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.Attack, -1);
        }
    }
    
    public class LowerUsersDefence : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.Defence, -1);
        }
    }

    public class LowerUsersSpAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.SpAttack, -1);
        }
    }
    
    public class LowerUsersSpDefence : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.SpDefence, -1);
        }
    }
    
    public class LowerUsersSpeed : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.Speed, -1);
        }
    }
    
}