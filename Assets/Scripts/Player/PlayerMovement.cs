using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Grid grid;

    private bool _moving;
    private Animator _animator;
    private Tilemap _obstructions;
    private Vector3Int _cell;
    private Vector3Int _targetCell;
    public Transform grass;

    private enum Direction
    {
        South = 1,
        West = 2,
        North = 3,
        East = 4
    }

    private int _facing = (int) Direction.South;
    private static readonly int AnimatorHeading = Animator.StringToHash("Heading");
    private static readonly int AnimatorMoving = Animator.StringToHash("Moving");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _cell = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(_targetCell);
        _targetCell = _cell;
        _obstructions = grid.transform.Find("Obstructions").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    private void Update()
    {
        _animator.SetBool(AnimatorMoving, _moving);
        
        var position = transform.position;
        var curCell = grid.WorldToCell(position);

        if (_moving)
        {
            var curCellPos = grid.GetCellCenterWorld(curCell);
            if (grass.position == curCellPos)
            {
                grass.GetChild(0).gameObject.SetActive(true);
            }
        }
        Vector3Int translationVector = Vector3Int.zero;
        translationVector.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        translationVector.y = translationVector.x != 0.0f ? 0 : Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        if (translationVector == Vector3Int.zero) return;

        var heading = (translationVector.x + 3) * Math.Abs(translationVector.x) +
                      (translationVector.y + 2) * Math.Abs(translationVector.y);

        if (heading == _facing)
        {
            _targetCell = curCell + translationVector;
        }

        if (_moving) return;
        
        if (_facing != heading)
        {
            _facing = heading;
            _animator.SetFloat(AnimatorHeading, heading);
            return;
        }

        var isObstructed = _obstructions.GetTile(_targetCell);
        if (isObstructed) return;
        
        var targetPos = grid.GetCellCenterWorld(_targetCell);
        StartCoroutine(Move(targetPos));
    }

    public void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(grid.CellToWorld(_targetCell), 0.1f);
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        _moving = true;
        _animator.SetBool(AnimatorMoving, _moving);

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        _moving = false;
    }
}