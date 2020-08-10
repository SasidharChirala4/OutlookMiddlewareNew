using System;
using System.Collections.Generic;
using Edreams.Outlook.TestPlugin.Views;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools;
using Office = Microsoft.Office.Core;

namespace Edreams.Outlook.TestPlugin
{
    public partial class ThisAddIn
    {
        public Explorer Explorer { get; private set; }

        private Dashboard _dashboard;
        private CustomTaskPane _customTaskPane;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            Explorer = Application.ActiveExplorer();
            Explorer.SelectionChange += Explorer_SelectionChange;

            _dashboard = new Dashboard();
            _customTaskPane = CustomTaskPanes.Add(_dashboard, "Outlook Middleware Test");
            _customTaskPane.Width = 500;
            _customTaskPane.Visible = true;
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        private async void Explorer_SelectionChange()
        {
            List<MailItem> selection = new List<MailItem>();

            foreach (object selected in Explorer.Selection)
            {
                if (selected is MailItem mail)
                {
                    selection.Add(mail);
                }
            }

            await _dashboard.UpdateSelection(selection);
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new EdreamsRibbon();
        }
    }
}