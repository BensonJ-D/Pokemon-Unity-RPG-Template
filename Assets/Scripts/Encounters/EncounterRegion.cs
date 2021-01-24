using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Encounters;
using UnityEngine;
using UnityEngine.Tilemaps;
using VFX;
using Random = UnityEngine.Random;

public class EncounterRegion : MonoBehaviour
{
    private Tilemap _underlay;
    private Tilemap _overlay;

    private Vector3Int _lastPlayerCell;
    private Grid _grid;
    
    private Vector3 _position;
    private Vector3Int _playerCell;
    
    private Dictionary<Vector3Int, Coroutine> _animating;

    [SerializeField] private BattleSystem _battleSystem;
    [SerializeField] private List<Tile> animatedTile;
    [SerializeField] private int framesPerSecond;
    [SerializeField] private List<Encounter> encounters;
    [SerializeField] private float encounterRate;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Transition transition;
    
    // Start is called before the first frame update
    private void Start()
    {
        _grid = GetComponent<Grid>();
        _underlay = transform.Find("Underlay").GetComponent<Tilemap>();
        _overlay = transform.Find("Overlay").GetComponent<Tilemap>();

        _animating = new Dictionary<Vector3Int, Coroutine>();
        
        _position = player.transform.position;
        _playerCell = _grid.WorldToCell(_position);

        var encounterThreshold = 100;
        foreach(var encounter in encounters)
        {
            encounterThreshold -= encounter.EncounterChance;
            encounter.EncounterThreshold = encounterThreshold;
        }
    }

    // Update is called once per frame
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
            StartCoroutine(GetEncounter());
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

    private IEnumerator GetEncounter()
    {
        var isEncounter = Random.Range(0, 100) < encounterRate;
        if (!isEncounter) yield break;
        
        var encounterThreshold = Random.Range(0, 100);
        var encounter = encounters.FirstOrDefault(e => encounterThreshold < e.EncounterThreshold);

        if (encounter is null) { yield break; }

        Game.State = GameState.Battle;

        yield return transition.StartTransition();
        
        _battleSystem.gameObject.SetActive(true);
        
        var level = Random.Range(encounter.MinimumLevel, encounter.MaximumLevel);
        Pokemon.Pokemon pokemon = new Pokemon.Pokemon(encounter.Pokemon, level);
        
        StartCoroutine(_battleSystem.SetupBattle(pokemon));
        yield return transition.EndTransition();
    }
}
