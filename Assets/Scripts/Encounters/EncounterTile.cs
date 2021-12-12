using UnityEngine;

namespace Encounters
{
    public class EncounterTile : MonoBehaviour
    {
        private Animator animator;

        public void Init()
        {
            animator = GetComponent<Animator>();
        }
        
        public void Animate()
        {
            animator.Play("Rustle");
        }
    }
}