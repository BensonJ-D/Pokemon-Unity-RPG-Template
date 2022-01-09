using System.ComponentModel;

namespace Menus.PartyMenu
{
    public enum PartyPopupMenuOption
    {
        None,
        Shift,
        Summary,
        Switch,
        [Description("Give Item")]
        GiveItem,
        [Description("Take Item")]
        TakeItem
    }
}