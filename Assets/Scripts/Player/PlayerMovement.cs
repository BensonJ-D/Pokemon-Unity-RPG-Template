using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed;

    private Vector2 _targetPos;
    private bool _moving;
    private Animator _animator;

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
        _targetPos = transform.position;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 translationVector;
        translationVector.x = Input.GetAxisRaw("Horizontal");
        translationVector.y = translationVector.x != 0.0f ? 0.0f : Input.GetAxisRaw("Vertical");

        if (translationVector == Vector2.zero) return;

        var heading = (int) ((translationVector.x + 3) * Math.Abs(translationVector.x) +
                             (translationVector.y + 2) * Math.Abs(translationVector.y));

        if (heading == _facing)
        {
            var position = transform.position;
            var curCell = new Vector2Int((int) position.x, (int) position.y);

            _targetPos = new Vector2(curCell.x + 0.5f, curCell.y + 0.5f) + translationVector;
        }
        
        if (_moving) return;
        Debug.Log("Not moving WITH input");
        if (_facing != heading)
        {
            _facing = heading;
            _animator.SetFloat(AnimatorHeading, heading);
            return;
        }

        StartCoroutine(Move());
    }

    public void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_targetPos, 0.1f);
    }

    private IEnumerator Move()
    {
        _moving = true;
        _animator.SetBool(AnimatorMoving, true);

        while ((_targetPos - (Vector2) transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = _targetPos;

        _moving = false;

        _animator.SetBool(AnimatorMoving, false);
    }
}