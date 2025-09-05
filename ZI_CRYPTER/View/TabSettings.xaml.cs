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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZI_CRYPTER.View
{
    /// <summary>
    /// Interaction logic for TabSettings.xaml
    /// </summary>
    public partial class TabSettings : UserControl
    {
        public TabSettings()
        {
            InitializeComponent();
        }

      

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var selectedItem = CodeAlgComboBox.SelectedItem as ComboBoxItem;


            if (selectedItem != null && selectedItem.Content.ToString() == "XTEA + OFB")
            {
                CodeIVTextBox.IsEnabled = true;
            }
            else
            {
                CodeIVTextBox.IsEnabled = false;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = CodeAlgComboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null && selectedItem.Content.ToString() == "XTEA + OFB")
            {
                CodeIVTextBox.IsEnabled = true;
            }
            else
            {
                CodeIVTextBox.IsEnabled = false;
            }
        }
    }
}
