
using System;
using UnityEngine;

namespace CoreSystems.MenuSystem
{
    /// <summary>
    /// Testing class for the menu system
    /// </summary>
    public class TestMenuSystem : MonoBehaviour
    {
        public PageController pageController;
//This is a test class so, make sure that the code in this class is only run while in Unity editor and not in a build
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                pageController.TurnPageOn(PageType.SHOP_MENU);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                pageController.TurnPageOff(PageType.SHOP_MENU); 
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                pageController.TurnPageOff(PageType.SHOP_MENU, PageType.EXTRAS_MENU);
            }
            
            if (Input.GetKeyDown(KeyCode.J))
            {
                pageController.TurnPageOff(PageType.SHOP_MENU, PageType.EXTRAS_MENU, true);
            }
        }
#endif
    } 
}

