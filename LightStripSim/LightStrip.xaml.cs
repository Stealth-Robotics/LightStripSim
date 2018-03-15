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

namespace LightStripSim
{
    /// <summary>
    /// Interaction logic for LightStripSim.xaml
    /// </summary>
    public partial class LightStrip : Window
    {
        public MainWindow parent;

        public int Count
        {
            get
            {
                return strip.Children.Count;
            }
        }

        public int numPixels()
        {
            int ct = 0;
            Dispatcher.Invoke(() =>
            {
                ct = Count;
            }
            );
            return ct;
        }

        public void setPixelColor(int index, Color color)
        {
            Dispatcher.Invoke(() =>
            {
                Set(index, color);
            }
            );
        }

        public LightStrip(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
            Closed += LightStrip_Closed;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        double t = 0;
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);

            parent.Top = this.Top + this.Height + 10;
            parent.Left = (screenWidth / 2) - (parent.Width / 2);

            //t += 1.0f / 30.0f / parent.FadeTime;
            //double tad = 0;
            //foreach (UIElement elm in strip.Children)
            //{
            //    LED led = elm as LED;
            //    if (t <= 1 + tad)
            //    {
            //        byte R = colorLerp(StartColor.R, EndColor.R, t < tad ? 0 : t / (1 + tad));
            //        byte G = colorLerp(StartColor.G, EndColor.G, t < tad ? 0 : t / (1 + tad));
            //        byte B = colorLerp(StartColor.B, EndColor.B, t < tad ? 0 : t / (1 + tad));
            //        led.Color = Color.FromRgb(R, G, B);
            //    }
            //    else if (t <= 2 + 2 * tad)
            //    {
            //        byte R = colorLerp(EndColor.R, StartColor.R, t - 1 < tad ? 0 : t / (2 + 2 * tad));
            //        byte G = colorLerp(EndColor.G, StartColor.G, t - 1 < tad ? 0 : t / (2 + 2 * tad));
            //        byte B = colorLerp(EndColor.B, StartColor.B, t - 1 < tad ? 0 : t / (2 + 2 * tad));
            //        led.Color = Color.FromRgb(R, G, B);
            //    }
            //    else
            //    {
            //        led.Color = StartColor;
            //        if (led == strip.Children[strip.Children.Count - 1])
            //        {
            //            t = 0;
            //        }
            //    }
            //    tad += parent.FadeTime;
            //}
        }

        public void Set(int index, Color color)
        {
            if (index < Count && index >= 0)
            {
                (strip.Children[index] as LED).Color = color;
            }
        }

        public Color Get(int index)
        {
            if (index < Count && index >= 0)
            {
                return (strip.Children[index] as LED).Color;
            }
            return Colors.Black;
        }

        void LightStrip_Closed(object sender, EventArgs e)
        {
            parent.Close();
        }

        public LightStrip()
        {
            InitializeComponent();
        }

        public void ContentChanged()
        {
            strip.Children.Clear();
            for (int i = 0; i < parent.NumberLEDs; i++)
            {
                strip.Children.Add(new LED(Colors.Silver));
            }
            t = 0;
        }
    }
}
