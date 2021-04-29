using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edreams.Outlook.TestPlugin.Model
{
    public class InspectorWrappers
    {
        #region <| Private Members |>

        private readonly IDashboardFactory _pluginControlFactory;
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IMailItemHelper _mailItemHelper;
        private readonly List<InspectorWrapper> _inspectorWrappers = new List<InspectorWrapper>();

        #endregion

        #region <| Public Indexers |>

        public InspectorWrapper this[string key]
        {
            get
            {
                return _inspectorWrappers.SingleOrDefault(x => x.Guid != null && x.Guid.Equals(key));
            }
        }

        public InspectorWrapper this[Inspector inspector]
        {
            get
            {
                return _inspectorWrappers.SingleOrDefault(x => x.Inspector == inspector);
            }
        }

        #endregion

        #region <| Construction |>

        public InspectorWrappers(
            IDashboardFactory pluginControlFactory,
            IConfigurationHelper configurationHelper,
            IMailItemHelper mailItemHelper)
        {
            _pluginControlFactory = pluginControlFactory;
            _configurationHelper = configurationHelper;
            _mailItemHelper = mailItemHelper;
        }

        #endregion

        #region <| Public Methods |>

        public void Add(Inspector inspector)
        {
            MailItem mailItem = inspector.CurrentItem as MailItem;
            if (mailItem != null)
            {
                InspectorWrapper inspectorWrapper = new InspectorWrapper(
                    _pluginControlFactory, inspector, _configurationHelper, _mailItemHelper, mailItem);
                _inspectorWrappers.Add(inspectorWrapper);
            }
        }

        public void Remove(Inspector inspector)
        {
            InspectorWrapper inspectorWrapperToRemove = this[inspector];
            if (inspectorWrapperToRemove != null)
            {
                _inspectorWrappers.Remove(inspectorWrapperToRemove);
            }
        }

        #endregion
    }
}