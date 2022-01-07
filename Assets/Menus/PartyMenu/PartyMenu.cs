using System.Collections;
using System.Collections.Generic;
using System.Window;
using System.Window.Menu;
using System.Window.Menu.GridMenu;
using MyBox;
using PokemonScripts;
using UnityEngine;
using VFX;

namespace Menus.PartyMenu
{ 
    public class PartyMenu : GridMenu<Pokemon>
    {
        [Separator("Party Menu")]
        [SerializeField] private List<PartyMenuItem> menuItems;
        [SerializeField] private PartyPopupMenu popupMenu;
        [SerializeField] private SummaryMenu.SummaryMenu summaryMenu;

        [Separator("Test data")] 
        [SerializeField] private bool useTestData;
        [SerializeField] private PokemonParty examplePokemon;
        
        public void Start()
        {
            Initiate();
            
            OptionsGrid = new IMenuItem<Pokemon>[,]
            {
                {menuItems[0], null, null, null, null, null},
                {null, menuItems[1], menuItems[2], menuItems[3], menuItems[4], menuItems[5]}
            };

            if(useTestData) StartCoroutine(ShowWindow(examplePokemon));
        }

        public IEnumerator ShowWindow(PokemonParty pokemon)
        {
            SetParty(pokemon.Party);
            yield return base.OpenWindow();
        }

        protected override IEnumerator OnConfirm()
        {
            var option = new List<PartyPopupMenuOption> {PartyPopupMenuOption.Shift, PartyPopupMenuOption.Summary};
            yield return popupMenu.ShowWindow(option, HandlePopupMenuResult);
        }

        private IEnumerator HandlePopupMenuResult(PartyPopupMenuOption choice)
        {
            if (choice == PartyPopupMenuOption.Summary)
            {
                StartCoroutine(TransitionController.Instance.RunTransition(Transition.BattleEnter));
                yield return TransitionController.Instance.WaitForTransitionPeak();
                yield return summaryMenu.ShowWindow(CurrentOption.Value);
            }
        }
        
        protected override void OnOptionChange(IMenuItem<Pokemon> previousOption, IMenuItem<Pokemon> newOption)
        {
            base.OnOptionChange(previousOption, newOption);
            ((PartyMenuItem)previousOption)?.SetNotSelected();
            ((PartyMenuItem)newOption).SetSelected();
        }

        private void SetParty(List<Pokemon> pokemon)
        {
            using var enumMenuItems = menuItems.GetEnumerator();
            using var enumPokemon = pokemon.GetEnumerator();
            while (enumMenuItems.MoveNext())
            {
                enumPokemon.MoveNext();
                // ReSharper disable once PossibleNullReferenceException
                enumMenuItems.Current.SetMenuItem(enumPokemon.Current);
            }
        }
    }
}