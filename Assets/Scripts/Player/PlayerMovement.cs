using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Grid grid;

    private bool _moving;
    private bool _stopMovement;
    private Animator _animator;
    private Tilemap _obstructions;
    private Vector3Int _targetCell;
    
    private enum Direction { Unknown = 0, South = 1, West = 2, North = 3, East = 4 }

    private Direction _direction = Direction.South;
    private static readonly int AnimatorDirection = Animator.StringToHash("Direction");
    private static readonly int AnimatorMoving = Animator.StringToHash("Moving");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        _targetCell = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(_targetCell);
        _obstructions = grid.transform.Find("Obstructions").GetComponent<Tilemap>();
    }

    private void Update()
    {
        _animator.SetBool(AnimatorMoving, _moving);

        if (Game.State != GameState.Moving) return;
        if (_moving) return;
        
        var movementVector = GetInputVector();
        if (movementVector == Vector3Int.zero) return;

        var moveDirection = GetDirection(movementVector);

        if (moveDirection == _direction)
        {
            var position = transform.position;
            var curCell = grid.WorldToCell(position);
            
            _targetCell = curCell + movementVector;
        } else
        {
            _direction = moveDirection;
            _animator.SetFloat(AnimatorDirection, (float)_direction);
            return;
        }

        var isObstructed = _obstructions.GetTile(_targetCell);
        if (isObstructed) return;
        
        var targetPos = grid.GetCellCenterWorld(_targetCell);
        StartCoroutine(Move(targetPos));
    }

    private static Vector3Int GetInputVector()
    {
        Vector3Int translationVector = Vector3Int.zero;
        translationVector.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        translationVector.y = translationVector.x != 0.0f ? 0 : Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        
        return translationVector;
    }

    private static Direction GetDirection(Vector3Int movementVector)
    {
        var direction = Direction.Unknown;
        
        if (movementVector.x == 1) direction = Direction.East;
        else if (movementVector.y == 1) direction = Direction.North;
        else if (movementVector.x == -1) direction = Direction.West;
        else if (movementVector.y == -1) direction = Direction.South;

        return direction;
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