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

namespace LightStripSim
{
    /// <summary>
    /// Interaction logic for SettingsPanelItem.xaml
    /// </summary>
    public partial class SettingsPanelItem : UserControl
    {
        public static DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(SettingsPanelItem));
        public string Label
        {
            get { return GetValue(LabelProperty) as string; }
            set { SetValue(LabelProperty, value); }
        }

        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(SettingsPanelItem));
        public string Value
        {
            get
            {
                return GetValue(ValueProperty) as string;
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static DependencyProperty NumericOnlyProperty = DependencyProperty.Register("NumericOnly", typeof(bool), typeof(SettingsPanelItem), new PropertyMetadata(false));
        public bool NumericOnly
        {
            get { return (bool)GetValue(NumericOnlyProperty); }
            set { SetValue(NumericOnlyProperty, value); }
        }

        public SettingsPanelItem()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (NumericOnly)
            {
                if (e.Text.Length == 1)
                {
                    if (!(char.IsDigit(e.Text[0]) ||
                        (e.Text[0] == '.' && !(sender as TextBox).Text.Contains('.'))))
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NumericOnly)
            {
                //make sure the number can be parsed
                double d;
                if (tb.Text.EndsWith("."))
                {
                    tb.Text = Value = tb.Text + "0";
                }
                if (tb.Text.StartsWith("."))
                {
                    tb.Text = Value = "0" + tb.Text;
                }
                if (tb.Text == "")
                {
                    tb.Text = Value = "0";
                }
                if (!double.TryParse(Value, out d))
                {
                    MessageBox.Show("Bad input");
                }
            }
        }
    }
}
