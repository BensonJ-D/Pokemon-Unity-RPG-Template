using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using PokemonScripts;
using UnityEngine;
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

        private Dictionary<Vector3Int, EncounterTile> tiles;
        private Vector3Int lastPlayerCell;
        private Tilemap tilemap;

        private void Start()
        {
            tilemap = transform.Find("Tiles").GetComponent<Tilemap>();
            tiles = new Dictionary<Vector3Int, EncounterTile>();
            lastPlayerCell = tilemap.WorldToCell(player.transform.position);
            encounters.Sort((a, b) => a.EncounterThreshold - b.EncounterThreshold);

            var cellBounds = tilemap.cellBounds;
            foreach (var cellPos in cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(cellPos);
                if (tile is null) continue;

                var sprite = tilemap.GetSprite(cellPos);
                var prefab = spriteToEncounterTileLookup
                    .First(pair => pair.IndexSprite == sprite)
                    .EncounterTile;

                if (prefab is null) continue;
                var localPos = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor;
                var encounterTile = Instantiate(prefab, localPos, Quaternion.identity);
                encounterTile.transform.parent = transform;
                encounterTile.hideFlags = HideFlags.HideInHierarchy;
                tiles.Add(cellPos, encounterTile.GetComponent<EncounterTile>());
                tiles.Last().Value.Init();
            }
        }

        private void Update()
        {
            var playerCell = tilemap.WorldToCell(player.transform.position);

            var isOnEncounterTile = tilemap.GetTile(playerCell);
            if (!isOnEncounterTile || playerCell == lastPlayerCell) return;

            lastPlayerCell = playerCell;
            tiles[playerCell].Animate();

            var isEncounter = Random.Range(0, 100) < encounterRate;
            if (!isEncounter) return;

            var encounterThreshold = Random.Range(0, 100);
            var encounter = encounters.FirstOrDefault(e => encounterThreshold < e.EncounterThreshold);
            if (encounter is null) return;

            var wildPokemon = new Pokemon();
            wildPokemon.Initialization(encounter.Pokemon, encounter.Level);

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