using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    public PokemonBase Base { get; set; }

    public int Level { get; set; }

    public int HP { get; set; }
    public Move[] Moves { get; set; }

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHP;
        Debug.Log("Base HP: " + Base.MaxHP);
        Debug.Log("Max HP: " + MaxHP);

        Moves = new Move[4];
        var moveSlot = 0;
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level > Level) break;

            Move newMove = new Move(move.Base);
            Moves[moveSlot] = newMove;

            moveSlot = moveSlot >= 3 ? 0 : moveSlot + 1;
        }
    }

    public int MaxHP => Mathf.FloorToInt((2 * Base.MaxHP * Level) / 100f) + 10 + Level;
    public int Attack => Mathf.FloorToInt((2 * Base.Attack * Level) / 100f) + 5;
    public int Defence => Mathf.FloorToInt((2 * Base.Defence * Level) / 100f) + 5;
    public int SpAttack => Mathf.FloorToInt((2 * Base.SpAttack * Level) / 100f) + 5;
    public int SpDefence => Mathf.FloorToInt((2 * Base.SpDefence * Level) / 100f) + 5;
    public int Speed => Mathf.FloorToInt((2 * Base.Speed * Level) / 100f) + 5;
}