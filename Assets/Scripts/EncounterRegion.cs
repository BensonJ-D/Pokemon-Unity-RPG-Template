using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EncounterRegion : MonoBehaviour
{
    private Tilemap _underlay;
    private Tilemap _overlay;

    private GameObject _player;
    private Vector3Int _lastPlayerCell;
    private Grid _grid;
    
    private Vector3 _position;
    private Vector3Int _playerCell;
    
    private Dictionary<Vector3Int, Coroutine> _animating;
    
    [SerializeField] private List<Tile> animatedTile;
    [SerializeField] private int framesPerSecond;

    // Start is called before the first frame update
    private void Start()
    {
        _grid = GetComponent<Grid>();
        _underlay = transform.Find("Underlay").GetComponent<Tilemap>();
        _overlay = transform.Find("Overlay").GetComponent<Tilemap>();

        _player = GameObject.FindWithTag("Player");
        _animating = new Dictionary<Vector3Int, Coroutine>();
        
        _position = _player.transform.position;
        _playerCell = _grid.WorldToCell(_position);
    }

    // Update is called once per frame
    private void Update()
    {
        _position = _player.transform.position;
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
}