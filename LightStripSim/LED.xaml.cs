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
    /// Interaction logic for LED.xaml
    /// </summary>
    public partial class LED : UserControl
    {
        public Color Color
        {
            get
            {
                return (ellipse.Stroke as SolidColorBrush).Color;
            }
            set
            {
                ellipse.Stroke = new SolidColorBrush(value);
                ellipse.Fill = new SolidColorBrush(value);
            }
        }

        public LED()
        {
            InitializeComponent();
        }

        public LED(Color color)
        {
            InitializeComponent();
            Color = color;
        }
    }
}
