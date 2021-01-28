using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PokemonScripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Encounters
{
    public class EncounterRegion : MonoBehaviour
    {
        public event Action<Pokemon> OnEncountered;
        
        [SerializeField] private PlayerController player;
        [SerializeField] private float encounterRate;
        [SerializeField] private List<Encounter> encounters;
        [SerializeField] private List<SpriteToEncounterTilePair> spriteToEncounterTileLookup;

        private Dictionary<Vector3Int, EncounterTile> _tiles;
        private Vector3Int _lastPlayerCell;
        private Tilemap _tilemap;
        private void Start()
        {
            _tilemap = transform.Find("Tiles").GetComponent<Tilemap>();
            _tiles = new Dictionary<Vector3Int, EncounterTile>();
            _lastPlayerCell = _tilemap.WorldToCell(player.transform.position);
            encounters.Sort((a, b) => a.EncounterThreshold - b.EncounterThreshold);
        
            var cellBounds = _tilemap.cellBounds;
            foreach (var cellPos in cellBounds.allPositionsWithin)
            {
                var tile = _tilemap.GetTile(cellPos);
                if (tile is null) continue;
                
                var sprite = _tilemap.GetSprite(cellPos);
                var prefab = spriteToEncounterTileLookup
                    .First(pair => pair.IndexSprite == sprite)
                    .EncounterTile;

                if (prefab is null) continue;
                var localPos = _tilemap.CellToWorld(cellPos) + _tilemap.tileAnchor;
                var encounterTile = Instantiate(prefab, localPos, Quaternion.identity);
                encounterTile.transform.parent = transform;
                _tiles.Add(cellPos, encounterTile.GetComponent<EncounterTile>());
                _tiles.Last().Value.Init();
            }
        }

        private void Update()
        {
            var playerCell = _tilemap.WorldToCell(player.transform.position);
        
            var isOnEncounterTile = _tilemap.GetTile(playerCell);
            if (!isOnEncounterTile || playerCell == _lastPlayerCell) return;
            
            _lastPlayerCell = playerCell;
            _tiles[playerCell].Animate();
            
            var isEncounter = Random.Range(0, 100) < encounterRate;
            if (!isEncounter) return;
            
            var encounterThreshold = Random.Range(0, 100);
            var encounter = encounters.FirstOrDefault(e => encounterThreshold < e.EncounterThreshold);
            if (encounter is null) return;
            
            var level = Random.Range(encounter.MinimumLevel, encounter.MaximumLevel);
            var wildPokemon = new Pokemon();
            wildPokemon.Initialization(encounter.Pokemon, level);
            
            OnEncountered?.Invoke(wildPokemon);
        }
    }

    [Serializable]
    public class SpriteToEncounterTilePair
    {
        [SerializeField] private Sprite indexSprite;
        [SerializeField] private GameObject encounterTile;

        public Sprite IndexSprite => indexSprite;
        public GameObject EncounterTile => encounterTile;
    }
}
