using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PokemonScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Encounters
{
    public class EncounterRegion : MonoBehaviour
    {
        public event Action<Pokemon> OnEncountered;
        private Tilemap _underlay;
        private Tilemap _overlay;

        private Vector3Int _lastPlayerCell;
        private Grid _grid;
    
        private Vector3 _position;
        private Vector3Int _playerCell;
    
        private Dictionary<Vector3Int, Coroutine> _animating;

        [SerializeField] private PlayerController player;
        [SerializeField] private int framesPerSecond;
        [SerializeField] private List<Tile> animatedTile;
        [SerializeField] private float encounterRate;
        [SerializeField] private List<Encounter> encounters;
    
        // Start is called before the first frame update
        private void Start()
        {
            _grid = GetComponent<Grid>();
            _underlay = transform.Find("Underlay").GetComponent<Tilemap>();
            _overlay = transform.Find("Overlay").GetComponent<Tilemap>();

            _animating = new Dictionary<Vector3Int, Coroutine>();
        
            _position = player.transform.position;
            _playerCell = _grid.WorldToCell(_position);

            encounters.Sort((a, b) => a.EncounterThreshold - b.EncounterThreshold);
        }

        private void Update()
        {
            _position = player.transform.position;
            _playerCell = _grid.WorldToCell(_position);

            var isOnEncounterTile = _underlay.GetTile(_playerCell);
            if (isOnEncounterTile && _playerCell != _lastPlayerCell)
            {
                if (_animating.ContainsKey(_playerCell))
                {
                    var oldCoroutine = _animating[_playerCell];
                    StopCoroutine(oldCoroutine);
                }

                _animating[_playerCell] = StartCoroutine(AnimateTile(_playerCell));
                CheckEncounter();
            }

            _lastPlayerCell = _playerCell;
        }

        private IEnumerator AnimateTile(Vector3Int cell)
        {
            var animationFrame = 0;
            var destroy = false;
        
            while (!destroy)
            {
                if (animationFrame < animatedTile.Count)
                {
                    SetTile(cell, animationFrame);
                    animationFrame++;
                    yield return new WaitForSeconds(1f / framesPerSecond);
                }
                else
                {
                    if (cell == _playerCell)
                    {
                        yield return new WaitForSeconds(1f / framesPerSecond);
                        continue;
                    }
                    _overlay.SetTile(cell, null);
                    destroy = true;
                }
            }
        
            _animating.Remove(cell);
        }
        private void SetTile(Vector3Int cell, int frame)
        {
            _overlay.SetTile(cell, animatedTile[frame]);
        }

        private void CheckEncounter()
        {
            var isEncounter = Random.Range(0, 100) < encounterRate;
            if (!isEncounter) return;
            
            var encounterThreshold = Random.Range(0, 100);
            var encounter = encounters.FirstOrDefault(e => encounterThreshold < e.EncounterThreshold);
            var level = Random.Range(encounter.MinimumLevel, encounter.MaximumLevel);
            Pokemon wildPokemon = new Pokemon();
            wildPokemon.Initialization(encounter.Pokemon, level);
            
            OnEncountered?.Invoke(wildPokemon);
        }
    }
}
