using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace VTility.Controls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    [ContentProperty("SurfaceContent")]
    public partial class PopupControl : UserControl
    {
        public Window frame { get; set; }

        public PopupControl()
            : base()
        {
            InitializeComponent();
        }

        public object SurfaceContent
        {
            get { return surfaceContent.Content; }
            set { surfaceContent.Content = value; }
        }
    }
}