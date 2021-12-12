using System.Collections;
using PokemonScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private int speed;
        [SerializeField] private Grid grid;
        [SerializeField] private PokemonParty party;
        [SerializeField] private Inventory.Inventory inventory;

        public PokemonParty Party => party;
        public Inventory.Inventory Inventory => inventory;

        private bool moving;
        private bool stopMovement;
        private Animator animator;
        private Tilemap obstructions;
        private Vector3Int targetCell;
    
        private enum Direction { Unknown = 0, South = 1, West = 2, North = 3, East = 4 }

        private Direction direction = Direction.South;
        private static readonly int AnimatorDirection = Animator.StringToHash("Direction");
        private static readonly int AnimatorMoving = Animator.StringToHash("Moving");

        private void Start()
        {
            animator = GetComponent<Animator>();
        
            targetCell = grid.WorldToCell(transform.position);
            transform.position = grid.GetCellCenterWorld(targetCell);
            obstructions = grid.transform.Find("Obstructions").GetComponent<Tilemap>();
        }

        public IEnumerator HandleMovement()
        {
            animator.SetBool(AnimatorMoving, moving);
        
            var movementVector = GetInputVector();
            if (movementVector == Vector3Int.zero) yield break;

            var moveDirection = GetDirection(movementVector);

            if (moveDirection == direction)
            {
                var position = transform.position;
                var curCell = grid.WorldToCell(position);
            
                targetCell = curCell + movementVector;
            } else
            {
                direction = moveDirection;
                animator.SetFloat(AnimatorDirection, (float)direction);
                yield break;
            }

            var isObstructed = obstructions.GetTile(targetCell);
            if (isObstructed) yield break;
        
            var targetPos = grid.GetCellCenterWorld(targetCell);
            yield return Move(targetPos);
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
            moving = true;
            animator.SetBool(AnimatorMoving, moving);

            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            moving = false;
        
            if(GameController.GameState != GameState.Moving) { animator.SetBool(AnimatorMoving, moving); }
        }
    }
}