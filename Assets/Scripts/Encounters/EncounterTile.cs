using System;
using System.Collections.Generic;
using System.Linq;
using PokemonScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

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