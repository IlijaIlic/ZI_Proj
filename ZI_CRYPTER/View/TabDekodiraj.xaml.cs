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
    /// Interaction logic for TabDekodiraj.xaml
    /// </summary>
    public partial class TabDekodiraj : UserControl
    {
        public TabDekodiraj()
        {
            InitializeComponent();
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DecodeKeyTextBox.Text == "Unesite novi naziv...")
            {
                DecodeKeyTextBox.Text = "";
                DecodeKeyTextBox.Foreground = Brushes.White;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DecodeKeyTextBox.Text))
            {
                DecodeKeyTextBox.Text = "Unesite novi naziv...";
                DecodeKeyTextBox.Foreground = Brushes.LightGray;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DecodeKeyTextBox.Text))
            {
                DecodeKeyTextBox.Text = "Unesite novi naziv...";
                DecodeKeyTextBox.Foreground = Brushes.LightGray;

                DecodeIVTextBox.Text = "IV";
                DecodeIVTextBox.Foreground = Brushes.LightGray;

            }

            var selectedItem = DecodeAlgComboBox.SelectedItem as ComboBoxItem;


            if (selectedItem != null && selectedItem.Content.ToString() == "XTEA + OFB")
            {
                DecodeIVTextBox.IsEnabled = true;
            }
            else
            {
                DecodeIVTextBox.IsEnabled = false;
            }
        }

        private void DecodeIVTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DecodeIVTextBox.Text == "IV")
            {
                DecodeIVTextBox.Text = "";
                DecodeIVTextBox.Foreground = Brushes.White;
            }
        }

        private void DecodeIVTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(DecodeIVTextBox.Text))
            {
                DecodeIVTextBox.Text = "IV";
                DecodeIVTextBox.Foreground = Brushes.LightGray;
            }
        }

        private void DecodeAlgComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = DecodeAlgComboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null && selectedItem.Content.ToString() == "XTEA + OFB")
            {
                DecodeIVTextBox.IsEnabled = true;
            }
            else
            {
                DecodeIVTextBox.IsEnabled = false;
            }

        }
    }
}
