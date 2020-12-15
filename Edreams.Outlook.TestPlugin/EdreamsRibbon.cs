using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.Core;

namespace Edreams.Outlook.TestPlugin
{
    [ComVisible(true)]
    public class EdreamsRibbon : Office.IRibbonExtensibility
    {
        #region <| IRibbonExtensibility Members |>

        public string GetCustomUI(string RibbonID)
        {
            // Get the CustomUI for the Ribbon from resources.
            return GetResourceText("Edreams.Outlook.TestPlugin.EdreamsRibbon.xml");
        }

        #endregion

        #region <| Public Methods |>

        public Bitmap GetEdreamsIcon(Office.IRibbonControl control)
        {
            return new Bitmap(Properties.Resources.Edreams);
        }

        public string GetEdreamsLabel(Office.IRibbonControl control)
        {
            return "OutlookMiddlewareTest";
        }

        public string GetEdreamsSuperTip(Office.IRibbonControl control)
        {
            return "SUPERTIP";
        }

        #endregion

        #region <| Helper Methods |>

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            foreach (string name in resourceNames)
            {
                if (string.Compare(resourceName, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(name)))
                    {
                        return resourceReader.ReadToEnd();
                    }
                }
            }
            return null;
        }

        #endregion
    }
}