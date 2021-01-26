using PokemonScripts;
using UnityEngine;

namespace Encounters
{
    [System.Serializable]
    public class Encounter
    {
        [SerializeField] private PokemonBase encounterPokemonBase;
        [SerializeField] private int encounterThreshold;
        [SerializeField] private int minimumLevel;
        [SerializeField] private int maximumLevel;

        public PokemonBase Pokemon => encounterPokemonBase;
        public int MinimumLevel => minimumLevel;
        public int MaximumLevel => maximumLevel;
        public int EncounterThreshold => encounterThreshold;
    }
}