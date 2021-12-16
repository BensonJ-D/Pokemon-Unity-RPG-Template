using UnityEngine;

namespace Encounters
{
    public class EncounterTile : MonoBehaviour
    {
        private Animator _animator;

        public void Init()
        {
            _animator = GetComponent<Animator>();
        }
        
        public void Animate()
        {
            _animator.Play("Rustle");
        }
    }
}