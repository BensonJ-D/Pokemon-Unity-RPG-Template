using System;
using System.ComponentModel;
using System.Reflection;

namespace ActionMenu
{
    public enum PartyPopupMenuOption
    {
        None,
        Shift,
        Summary,
        [Description("Give Item")]
        GiveItem,
        [Description("Take Item")]
        TakeItem
    }
}