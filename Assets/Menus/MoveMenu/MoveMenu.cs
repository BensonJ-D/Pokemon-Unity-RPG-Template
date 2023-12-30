using System.Collections;
using System.Collections.Generic;
using System.Window;
using Characters.Moves;
using GameSystem.Window.Menu;
using GameSystem.Window.Menu.Grid;
using MyBox;
using UnityEngine;

namespace Menus.MoveMenu
{
    public class MoveMenu : GridMenu<Characters.Moves.Move>
    {
        [Separator("Move Menu Settings")] [SerializeField]
        private MoveDetails moveDetails;

        [SerializeField] private List<MoveMenuItem> menuItems;

        [Separator("Test data")] [SerializeField]
        private bool useTestData;

        [SerializeField] private List<MoveBase> exampleMoves;

        public override void Initialise() {
            OptionsGrid = new IMenuItem<Characters.Moves.Move>[,] {
                {menuItems[0], menuItems[2]},
                {menuItems[1], menuItems[3]}
            };

            base.Initialise();
        }

        public IEnumerator OpenWindow(List<Characters.Moves.Move> moveList) {
            SetMoves(moveList);
            yield return base.OpenWindow();
        }

        protected override void OnOptionChange(IMenuItem<Characters.Moves.Move> previousOption,
            IMenuItem<Characters.Moves.Move> newOption) {
            base.OnOptionChange(previousOption, newOption);
            moveDetails.SetMoveDetails(newOption.Value);
        }

        private void SetMoves(List<Characters.Moves.Move> moves) {
            using var enumMenuItems = menuItems.GetEnumerator();
            using var enumMoves = moves.GetEnumerator();
            while (enumMenuItems.MoveNext()) {
                enumMoves.MoveNext();
                // ReSharper disable once PossibleNullReferenceException
                enumMenuItems.Current.SetMenuItem(enumMoves.Current);
            }
        }

        protected override IEnumerator OnConfirm() {
            CloseReason = WindowCloseReason.Complete;
            yield return base.OnConfirm();
            yield return base.CloseWindow();
        }

        protected override IEnumerator OnCancel() {
            CloseReason = WindowCloseReason.Cancel;
            yield return base.OnCancel();
            yield return base.CloseWindow();
        }
    }
}