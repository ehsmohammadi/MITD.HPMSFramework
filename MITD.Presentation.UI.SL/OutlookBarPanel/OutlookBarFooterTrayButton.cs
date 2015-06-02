using System.Windows.Controls;
using MITD.Presentation.UI.OutlookBarPanel;

namespace MITD.Presentation.UI
{
    internal class OutlookBarFooterTrayButton : Button
    {
                /// <summary>
        /// Initializes a new instance of the OutlookBarFooterTrayButton.
        /// </summary>
        public OutlookBarFooterTrayButton()
            : base()
        {
            DefaultStyleKey = typeof(OutlookBarFooterTrayButton);
        }


        /// <summary>
        /// Builds the visual tree for the OutlookBarFooterTrayButton when a new template
        /// is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


    }
}
