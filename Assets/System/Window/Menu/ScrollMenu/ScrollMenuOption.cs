using System.Linq;
using UnityEngine;

namespace System.Window.Menu.ScrollMenu
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
        public static ScrollMenuOption<T> GetNextScrollMenuOption<T>(this ScrollMenu<T> scrollMenu, bool allowEmptyRows = false)
        {
            var optionMenuItems = scrollMenu.OptionMenuItems.Where(item => item.IsNotNullOrEmpty()).ToList();
            var option = scrollMenu.CurrentOption;
            
            var cursorSize = optionMenuItems.Count - 1;
            var scrollSize = scrollMenu.OptionsList.Count - 1;
            var (_, cursorPosition) = scrollMenu.CurrentCursorPosition;
            var scrollPosition = scrollMenu.CurrentListPosition;
            
            var originalChoice = new ScrollMenuOption<T>(cursorPosition, scrollPosition, option);
            var newChoice = new ScrollMenuOption<T>(cursorPosition, scrollPosition, option);
            
            if (!DirectionPressed) return originalChoice;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (scrollPosition + 1 > scrollSize) return originalChoice;

                scrollPosition++;
                cursorPosition = Math.Min(cursorPosition + 1, cursorSize);

                newChoice = new ScrollMenuOption<T>(cursorPosition, scrollPosition, optionMenuItems[cursorPosition]);
            } 
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (scrollPosition - 1 < 0) return originalChoice;

                scrollPosition--;
                cursorPosition = Math.Max(cursorPosition - 1, 0);

                newChoice = new ScrollMenuOption<T>(cursorPosition, scrollPosition, optionMenuItems[cursorPosition]);
            }
        
            if (allowEmptyRows) return newChoice;
            
            return newChoice.Option == null ? originalChoice : newChoice;
        }
        
        public static ScrollMenuOption<T> GetInitialScrollPosition<T>(this ScrollMenu<T> scrollMenu)
        {
            return new ScrollMenuOption<T>(0, 0, scrollMenu.OptionMenuItems[0]);
        }
        
        private static bool DirectionPressed => Input.GetKeyDown(KeyCode.UpArrow) 
                                           || Input.GetKeyDown(KeyCode.DownArrow);
    }
}