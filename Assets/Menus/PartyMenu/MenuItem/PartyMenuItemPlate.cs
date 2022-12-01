using UnityEngine;
using UnityEngine.UI;

namespace Menus.Party.MenuItem
{
    public class PartyMenuItemPlate : MonoBehaviour
    {
        [SerializeField] private Sprite notSelectedSprite;
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Sprite notSelectedFaintedSprite;
        [SerializeField] private Sprite selectedFaintedSprite;
        [SerializeField] private Sprite switchFromSprite;
        [SerializeField] private Sprite switchToSprite;
        [SerializeField] private Image plateImage;

        public void SetSelected(bool fainted) => plateImage.sprite = fainted ? selectedFaintedSprite : selectedSprite;
        public void SetNotSelected(bool fainted) => plateImage.sprite = fainted ? notSelectedFaintedSprite : notSelectedSprite;

        public void SetShiftFrom() => plateImage.sprite = switchFromSprite;
        public void SetShiftTo() => plateImage.sprite = switchToSprite;
    }
}