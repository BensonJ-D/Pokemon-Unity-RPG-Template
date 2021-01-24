using Pokemon;
using UnityEngine;

namespace Encounters
{
    [CreateAssetMenu(fileName = "Encounter", menuName = "Encounter/New encounter", order = 0)]
    public class Encounter : ScriptableObject
    {
        [SerializeField] private PokemonBase pokemon;
        [SerializeField] private int encounterChance;
        [SerializeField] private int minimumLevel;
        [SerializeField] private int maximumLevel;

        public PokemonBase Pokemon => pokemon;
        public int EncounterChance
        {
            get => encounterChance;
            set => encounterChance = value;
        }

        public int MinimumLevel => minimumLevel;
        public int MaximumLevel => maximumLevel;
        
        public int EncounterThreshold { get; set; }
    }
}