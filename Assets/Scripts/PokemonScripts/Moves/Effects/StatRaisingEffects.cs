namespace PokemonScripts.Moves.Effects
{
    public class RaiseTargetsAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.Attack, 1);
        }
    }
    
    public class RaiseTargetsDefence : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.Defence, 1);
        }
    }

    public class RaiseTargetsSpAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.SpAttack, 1);
        }
    }

    public class RaiseTargetsSpDefence : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.SpDefence, 1);
        }
    }
    
    public class RaiseTargetsSpeed : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) Stat.Speed, 1);
        }
    }
    
    public class RaiseUsersAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.Attack, 1);
        }
    }
    
    public class RaiseUsersDefence : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.Defence, 1);
        }
    }

    public class RaiseUsersSpAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.SpAttack, 1);
        }
    }
    
    public class RaiseUsersSpDefence : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.SpDefence, 1);
        }
    }
    
    public class RaiseUsersSpeed : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, user, (int) Stat.Speed, 1);
        }
    }
    
}