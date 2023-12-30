using System.Collections;
using System.Utilities.Input;
using MyBox;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld.Players
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private int speed;
        [SerializeField] private Grid grid;
        [SerializeField] private Characters.Players.Player player;
        [SerializeField] private Animator animator;

        private bool _moving;
        private Vector3Int _targetCell;
        public Tilemap Obstructions { get; set; }
        public Characters.Players.Player Player => player;

        private enum Direction { Unknown = 0, South = 1, West = 2, North = 3, East = 4 }

        private Direction _direction = Direction.South;
        private static readonly int AnimatorDirection = Animator.StringToHash("Direction");
        private static readonly int AnimatorMoving = Animator.StringToHash("Moving");

        private void Start()
        {
            animator = GetComponent<Animator>();
            Obstructions = grid.transform.Find("Obstructions").GetComponent<Tilemap>();
            
            var localPosition = transform.localPosition;
            var xTile = Mathf.RoundToInt(localPosition.x);
            var yTile = Mathf.RoundToInt(localPosition.y);
            _targetCell = new Vector3Int(xTile, yTile);
            
            localPosition = _targetCell;
            transform.localPosition = localPosition;
        }

        public IEnumerator HandleMovement()
        {
            animator.SetBool(AnimatorMoving, _moving);

            var movementVector = InputController.GetMoveInput();
            if (movementVector == Vector3Int.zero) yield break;

            var moveDirection = GetDirection(movementVector);

            if (moveDirection == _direction) {
                var curCell = transform.localPosition.ToVector3Int();
                _targetCell = curCell + movementVector;
            } else {
                _direction = moveDirection;
                animator.SetFloat(AnimatorDirection, (float)_direction);
                yield break;
            }

            var isObstructed = Obstructions.GetTile(_targetCell);
            if (isObstructed) yield break;
        
            var targetPos = _targetCell;
            yield return Move(targetPos);
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
            animator.SetBool(AnimatorMoving, _moving);

            while ((targetPos - transform.localPosition).sqrMagnitude > Mathf.Epsilon)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, speed * Time.deltaTime);
                yield return null;
            }

            transform.localPosition = targetPos;
            _moving = false;
        
            animator.SetBool(AnimatorMoving, _moving);
        }
    }
}

