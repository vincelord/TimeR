using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VTility.Logic;

namespace VTility.Windows
{
    /// <summary>
    /// Interaction logic for WPopupAbort.xaml
    /// </summary>
    public partial class WPopupAbort : BasePopup
    {
        public WPopupAbort() : base()
        {
            InitializeComponent();
        }

        override internal void ConfirmAction(object sender, ExecutedRoutedEventArgs e)
        {
            UtilTimer.Current.StopCountdown();
            UtilTimer.Current.Delete();
            UtilTimer.LoadAllFromRegistry();
            TimerTicker.ReloadTickerEntries();
            PopupCloseAnimated();
        }
    }
}