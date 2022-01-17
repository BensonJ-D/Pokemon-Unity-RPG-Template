using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Window.Menu;
using System.Window.Menu.Grid;
using MyBox;
using PokemonScripts.Moves;
using UnityEngine;

namespace Menus.Move
{ 
    public class MoveMenu : GridMenu<PokemonScripts.Moves.Move>
    {
        [Separator("Move Menu Settings")]
        [SerializeField] private MoveDetails moveDetails;
        [SerializeField] private List<MoveMenuItem> menuItems;
        
        [Separator("Test data")] 
        [SerializeField] private bool useTestData;
        [SerializeField] private List<MoveBase> exampleMoves;

        public void Start()
        {
            Initialise();
            
            OptionsGrid = new IMenuItem<PokemonScripts.Moves.Move>[,]
            {
                {menuItems[0], menuItems[2]},
                {menuItems[1], menuItems[3]}
            };

            if (!useTestData) return;
            
            List<PokemonScripts.Moves.Move> moveTest = exampleMoves
                .Select(move => new PokemonScripts.Moves.Move(move) {Pp = Random.Range(0, move.Pp)})
                .ToList();
            
            StartCoroutine(ShowWindow(moveTest));
        }

        public IEnumerator ShowWindow(List<PokemonScripts.Moves.Move> moves)
        {
            SetMoves(moves);
            yield return base.OpenWindow();
        }

        protected override void OnOptionChange(IMenuItem<PokemonScripts.Moves.Move> previousOption, IMenuItem<PokemonScripts.Moves.Move> newOption)
        {
            base.OnOptionChange(previousOption, newOption);
            moveDetails.SetMoveDetails(newOption.Value);
        }

        private void SetMoves(List<PokemonScripts.Moves.Move> moves)
        {
            using var enumMenuItems = menuItems.GetEnumerator();
            using var enumMoves = moves.GetEnumerator();
            while (enumMenuItems.MoveNext())
            {
                enumMoves.MoveNext();
                // ReSharper disable once PossibleNullReferenceException
                enumMenuItems.Current.SetMenuItem(enumMoves.Current);
            }
        }
    }
}