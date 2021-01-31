using PokemonScripts;
using UnityEngine;

namespace Encounters
{
    [System.Serializable]
    public class Encounter
    {
        [SerializeField] private PokemonBase encounterPokemonBase;
        [SerializeField] private int encounterThreshold;
        [SerializeField] private int level;

        public PokemonBase Pokemon => encounterPokemonBase;
        public int Level => level;
        public int EncounterThreshold => encounterThreshold;
    }
}