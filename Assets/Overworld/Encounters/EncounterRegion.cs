using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters.Monsters;
using GameSystem.Utilities;
using Overworld.Players;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Overworld.Encounters
{
    public class EncounterRegion : MonoBehaviour
    {
        public delegate IEnumerator OnEncounterCallback(Pokemon wildPokemon);
        public event OnEncounterCallback OnEncountered;

        [SerializeField] private PlayerController playerController;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private List<SpriteToEncounterTilePair> spriteToEncounterTileLookup;
        
        [SerializeField] private float encounterRate;
        [SerializeField] private List<Encounter> encounters;

        private Dictionary<Vector2Int, EncounterTile> _tiles;
        private Vector2Int _lastPlayerCell;

        private void Start()
        {
            _tiles = new Dictionary<Vector2Int, EncounterTile>();
            _lastPlayerCell = PlayerPosition;
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
                var localPos = cellPos + tilemap.tileAnchor;
                var encounterTile = Instantiate(prefab, transform);
                encounterTile.transform.localPosition = localPos;
                
                // encounterTile.hideFlags = HideFlags.HideInHierarchy;
                var cell = cellPos.ToVector2Int();
                _tiles.Add(cell, encounterTile);
                _tiles.Last().Value.Init();
            }
        }

        private void Update()
        {
            var playerCell = PlayerPosition;

            var isOnEncounterTile = tilemap.GetTile(playerCell.ToVector3Int());
            if (!isOnEncounterTile || playerCell == _lastPlayerCell) return;

            _lastPlayerCell = playerCell;
            _tiles[playerCell].Animate();

            var isEncounter = Random.Range(0, 100) < encounterRate;
            if (!isEncounter) return;

            var encounterThreshold = Random.Range(0, 100);
            var encounter = encounters.FirstOrDefault(e => encounterThreshold < e.EncounterThreshold);
            if (encounter is null) return;

            var wildPokemon = new Pokemon();
            wildPokemon.Initialization(encounter.Pokemon, encounter.Level);

            StartCoroutine(OnEncountered?.Invoke(wildPokemon));
        }

        private Vector2Int PlayerPosition => new(
            Mathf.RoundToInt(playerController.transform.localPosition.x), 
            Mathf.RoundToInt(playerController.transform.localPosition.y)
            );
        
        [Serializable]
        private class SpriteToEncounterTilePair
        {
            [SerializeField] private Sprite indexSprite;
            [SerializeField] private EncounterTile encounterTile;

            public Sprite IndexSprite => indexSprite;
            public EncounterTile EncounterTile => encounterTile;
        }
    }
}

