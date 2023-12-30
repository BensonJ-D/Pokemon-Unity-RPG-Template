using System.ComponentModel;

namespace Menus.Party
{
    public enum PartyPopupMenuOption
    {
        None, Shift, Summary,
        Switch, [Description("Give Item")] GiveItem, [Description("Take Item")] TakeItem
    }
}