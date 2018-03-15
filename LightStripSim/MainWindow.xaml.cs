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
using System.Reflection;

namespace LightStripSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double FadeTime
        {
            get
            {
                return double.Parse(time.Value);
            }
        }

        public int NumberLEDs
        {
            get
            {
                return int.Parse(num.Value);
            }
        }

        public MethodInfo SelectedSequence
        {
            get
            {
                return typeof(Controller).GetMethod(
                    (wombocombo.SelectedItem as ComboBoxItem).Content as string,
                    Type.EmptyTypes);
            }
        }

        LightStrip child;
        Controller c;

        public MainWindow()
        {
            InitializeComponent();
            child = new LightStrip(this);
            child.Show();
            child.ContentChanged();
            Type cType = typeof(Controller);
            foreach (MethodInfo inf in cType.GetMethods())
            {
                if (inf.ReturnType == typeof(void) &&
                    inf.GetParameters().Length == 0 &&
                    inf.Name != "Stop" && inf.Name != "ThreadMain")
                {
                    wombocombo.Items.Add(new ComboBoxItem() { Content = inf.Name });
                }
            }
            wombocombo.SelectedIndex = 0;
            c = new Controller(child);
            Closed += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, EventArgs e)
        {
            c.Stop();
            child.Close();
        }

        private void SettingsPanelItem_LostFocus(object sender, RoutedEventArgs e)
        {
            SettingsPanelItem s = sender as SettingsPanelItem;
            s.Value = System.Text.RegularExpressions.Regex.Replace(s.Value, "\\..*", "");
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            child.ContentChanged();
        }
    }
}
