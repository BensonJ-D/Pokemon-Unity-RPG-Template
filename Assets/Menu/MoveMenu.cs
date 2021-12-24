using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using PokemonScripts;
using PokemonScripts.Moves;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Menu
{ 
    public class MoveMenu : GridMenu<Move>
    {
        [Separator("Move UI")]
        [SerializeField] private MoveDetails moveDetails;
        [SerializeField] private List<MoveMenuItem> menuItems;
        [SerializeField] private List<MoveBase> exampleMoves;

        public void Start()
        {
            Initiate();
            
            OptionsMatrix = new IMenuItem<Move>[,]
            {
                {menuItems[0], menuItems[2]},
                {menuItems[1], menuItems[3]}
            };
            
            List<Move> moveTest = exampleMoves
                .Select(move => new Move(move) {Pp = Random.Range(0, move.Pp)})
                .ToList();

            StartCoroutine(ShowWindow(moveTest));
        }

        public IEnumerator ShowWindow(List<Move> moves)
        {
            SetMoves(moves);
            yield return base.ShowWindow();
        }

        protected override void OnOptionChange(IMenuItem<Move> previousOption, IMenuItem<Move> newOption)
        {
            base.OnOptionChange(previousOption, newOption);
            moveDetails.SetMoveDetails(newOption.Value);
        }

        private void SetMoves(List<Move> moves)
        {
            using var enumMenuItems = menuItems.GetEnumerator();
            using var enumMoves = moves.GetEnumerator();
            while (enumMenuItems.MoveNext())
            {
                enumMoves.MoveNext();
                // ReSharper disable once PossibleNullReferenceException
                enumMenuItems.Current.SetMove(enumMoves.Current);
            }
        }
    }
}