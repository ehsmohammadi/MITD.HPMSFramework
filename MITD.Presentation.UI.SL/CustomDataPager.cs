using System; 
using System.Net; 
using System.Windows; 
using System.Windows.Controls; 
using System.Windows.Documents; 
using System.Windows.Ink; 
using System.Windows.Input; 
using System.Windows.Media; 
using System.Windows.Media.Animation; 
using System.Windows.Shapes;

namespace MITD.Presentation.UI
{
    public class CustomDataPager : DataPager
    {
        TextBlock currentPagePrefixTextBlock;
        TextBlock currentPageSuffixTextBlock;
        TextBox currentPageTextBox;

        public CustomDataPager()
            : base()
        {
            this.PageIndexChanged += new EventHandler<EventArgs>(CustomDataPager_PageIndexChanged);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(CustomDataPager_MouseLeftButtonDown);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            currentPagePrefixTextBlock = GetTemplateChild("CurrentPagePrefixTextBlock") as TextBlock;
            currentPageSuffixTextBlock = GetTemplateChild("CurrentPageSuffixTextBlock") as TextBlock;
            currentPageTextBox = GetTemplateChild("CurrentPageTextBox") as TextBox;
            currentPageTextBox.TextChanged += new TextChangedEventHandler(currentPageTextBox_TextChanged);
            currentPageSuffixTextBlock.SizeChanged += new SizeChangedEventHandler(currentPageSuffixTextBlock_SizeChanged);
        }

        void currentPageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TranslateLabels();
        }

        void CustomDataPager_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TranslateLabels();
        }

        void CustomDataPager_PageIndexChanged(object sender, EventArgs e)
        {
            TranslateLabels();
        }

        void currentPageSuffixTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TranslateLabels();
        }

        private void TranslateLabels()
        {
            if (currentPagePrefixTextBlock != null)
            {
                currentPagePrefixTextBlock.Text = "صفحه";
                currentPageSuffixTextBlock.Text = string.Format("از {0}", this.PageCount);
            }
        }
    }
}
