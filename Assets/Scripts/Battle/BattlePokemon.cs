using System;
using System.Collections;
using Pokemon;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class BattlePokemon : MonoBehaviour
    {
        [SerializeField] private PokemonBase @base;
        [SerializeField] private int level;
        [SerializeField] private bool displayFront;
    
        public Pokemon.Pokemon Pokemon { get; set; }
        private Image _image;
        // private Vector3 _battlePosition;

        private void Awake()
        {
            _image = GetComponentInChildren<Image>();
            // _battlePosition = _image.transform.localPosition;
        }

        public void Setup()
        {
            Pokemon = new Pokemon.Pokemon(@base, level);
            _image.sprite = displayFront ? Pokemon.Base.FrontSprite : Pokemon.Base.BackSprite;
        }
        
        public void Setup(Pokemon.Pokemon pokemon)
        {
            Pokemon = pokemon;
            _image.sprite = displayFront ? Pokemon.Base.FrontSprite : Pokemon.Base.BackSprite;
            
        }

        // public IEnumerator PlayEnterAnimation(int speed, float xOffset, float direction)
        // {
        //     var animating = true;
        //     
        //     Vector3 newPosition = _image.transform.localPosition;
        //     newPosition.x = xOffset;
        //     
        //     while (animating)
        //     {
        //         var translationFactor = speed * Time.deltaTime;
        //         if (Mathf.Abs(newPosition.x - _battlePosition.x) < translationFactor) {
        //             animating = false;
        //             newPosition = _battlePosition;
        //         }
        //         else {
        //             newPosition.x += direction * translationFactor;
        //         }
        //         _image.transform.localPosition = newPosition;
        //         yield return null;
        //     }
        // }
    }
}