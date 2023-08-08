using System.Linq;
using UnityEngine;

namespace System.Window.Menu.Scroll
{
    public class ScrollMenuOption<T> 
    { 
        public readonly int CursorIndex;
        public readonly int ScrollIndex;
        public readonly IMenuItem<T> Option;

        public ScrollMenuOption(int cursorIndex, int scrollIndex, IMenuItem<T> option)
        {
            CursorIndex = cursorIndex;
            ScrollIndex = scrollIndex;
            Option = option;
        }
    }

    public static class ScrollMenuExtensionFunctions
    {
        public static ScrollMenuOption<T> GetNextScrollMenuOption<T>(this ScrollMenu<T> scrollMenu, int inputDirection, 
            bool allowEmptyRows = false)
        {
            var optionMenuItems = scrollMenu.OptionMenuItems.Where(item => item.IsNotNullOrEmpty()).ToList();
            var option = scrollMenu.CurrentOption;
            
            var cursorSize = optionMenuItems.Count - 1;
            var scrollSize = scrollMenu.OptionsList.Count - 1;
            var (_, cursorPosition) = scrollMenu.CurrentCursorPosition;
            var scrollPosition = scrollMenu.CurrentListPosition;
            
            ScrollMenuOption<T> newChoice;
            var originalChoice = new ScrollMenuOption<T>(cursorPosition, scrollPosition, option);

            if (!originalChoice.Option.IsNotNullOrEmpty() && scrollPosition > 0)
            {
                scrollPosition--;
                cursorPosition = Math.Max(cursorPosition - 1, 0);

                newChoice = new ScrollMenuOption<T>(cursorPosition, scrollPosition, optionMenuItems[cursorPosition]);
                return newChoice.Option == null ? originalChoice : newChoice;
            }
            
            if (inputDirection == 0) return originalChoice;

            if (scrollPosition - inputDirection > scrollSize ||
                scrollPosition - inputDirection < 0) return originalChoice;

            scrollPosition -= inputDirection;
            cursorPosition = Math.Clamp(cursorPosition - inputDirection, 0, cursorSize);

            newChoice = new ScrollMenuOption<T>(cursorPosition, scrollPosition, optionMenuItems[cursorPosition]);

            if (allowEmptyRows) return newChoice;
            
            return newChoice.Option == null ? originalChoice : newChoice;
        }
        
        public static ScrollMenuOption<T> GetInitialScrollPosition<T>(this ScrollMenu<T> scrollMenu, bool useFirstElement = true)
        {
            return useFirstElement ? 
                new ScrollMenuOption<T>(0, 0, scrollMenu.OptionMenuItems[0]) : 
                new ScrollMenuOption<T>(scrollMenu.OptionMenuItems.Count - 1, scrollMenu.OptionsList.Count - 1, scrollMenu.OptionMenuItems.Last());
        }
    }
}