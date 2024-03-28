/* [WIP]
 * to control the ui
 * TODO-list: resources aollcate and release
 */

using Shuile.UI;

namespace Shuile.Framework
{
    public static class UICtrlExt
    {
        public static void InitGameplay(this UICtrl self)
        {
            self.Register<EndGamePanel>(new EndGamePanel());
            self.Register<PlayingPanel>(new PlayingPanel());
            //self.Register<HUDHpBarElement>(new HUDHpBarElement()); // TODO: [!] generate only when you need
        }

        public static void DeInitGameplay(this UICtrl self)
        {
            self.UnRegister<EndGamePanel>();
            self.UnRegister<PlayingPanel>();
            //self.UnRegister<HUDHpBarElement>();
        }
    }
}
