using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media;

namespace LightStripSim
{
    public class Controller
    {
        LightStrip strip;
        Thread runningThread;
        Color offColor = Colors.Silver;
        Color color = Colors.Crimson;
        //Constructor, just sets up the necessary thread.
        //This actually needs to run separate from the UI thread for immediate updates in display
        //Not a problem that needs to be worried about on the robot
        public Controller(LightStrip strip)
        {
            this.strip = strip;
            runningThread = new Thread(() => ThreadMain());
            runningThread.Start();
        }

        //Main thread executor. Just updates the running sequence, no need to worry about this
        public void ThreadMain()
        {
            while (true)
            {
                System.Reflection.MethodInfo inf = null;
                strip.Dispatcher.Invoke(() =>
                    {
                        inf = strip.parent.SelectedSequence;
                    });
                inf.Invoke(this, Type.EmptyTypes);
            }
        }

        //##### Pattern routine #####
        //Bounces a light back and forth between the ends of the strip.
        //This is not a good choice for a full loop around the frame
        public void NightRider()
        {
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, offColor);
            }
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, color); // Set new pixel 'on'
                Thread.Sleep((int)(FadeTime() * 1000));
                for (int j = 0; j <= 2; j++)
                {
                    strip.setPixelColor(i - j, colorLerp(color, offColor, 1.0f / 3.0f * (j + 1)));
                }
            }
            for (int i = strip.numPixels() - 1; i >= 0; i--)
            {
                strip.setPixelColor(i, color); // Set new pixel 'on'
                Thread.Sleep((int)(FadeTime() * 1000));
                for (int j = 0; j <= 2; j++)
                {
                    strip.setPixelColor(i + j, colorLerp(color, offColor, 1.0f / 3.0f * (j + 1)));
                }
            }
        }

        //##### Pattern routine #####
        //Runs the color along the strip, fading out behind it.
        //If the strip is looped around the frame, it will NOT look quite right without an adjustment.
        //(You'll see why this is inside)
        public void ColorChase()
        {
            //Reset everything
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, offColor);
            }
            //Note the numPixels + 2! we need to go past the end of the strip to finish fading out
            for (int i = 0; i < strip.numPixels() + 2; i++)
            {
                strip.setPixelColor(i, color); // Set new pixel 'on'
                Thread.Sleep((int)(FadeTime() * 1000));
                //step back 3 LEDs
                for (int j = 0; j <= 2; j++)
                {
                    //fade them out a 3rd more of the way.
                    strip.setPixelColor(i - j, colorLerp(color, offColor, 1.0f / 3.0f * (j + 1)));
                }
            }
        }

        //##### Pattern routine #####
        //Similar to color chase, but fades in, then out rather than just out.
        //Also a much wider fade distance
        public void FadeChase()
        {
            //turn off the LED
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, offColor);
            }
            //Work from really far back - we fade through the entire strip length
            for (int j = -strip.numPixels(); j < strip.numPixels(); j++)
            {
                for (int i = 0; i < strip.numPixels(); i++)
                {
                    //Fade halfway in proportional to position on the strip.
                    //The first LED gets full target color, and then isn't touched again
                    strip.setPixelColor(i + j, colorLerp(color, offColor, (double)i / strip.numPixels() / 2));
                }
                Thread.Sleep((int)(FadeTime() * 1000 / strip.numPixels() / 2));
            }
            for (int j = -strip.numPixels(); j < strip.numPixels(); j++)
            {
                for (int i = 0; i < strip.numPixels(); i++)
                {
                    //Now fade them back
                    strip.setPixelColor(i + j, colorLerp(offColor, color, (double)i / strip.numPixels() / 2));
                }
                Thread.Sleep((int)(FadeTime() * 1000 / strip.numPixels() / 2));
            }
        }

        //##### Pattern routine #####
        //Does the ColorChase routine but with rainbow colors
        public void RainbowChase()
        {
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, offColor);
            }
            for (int i = 0; i < strip.numPixels(); i++)
            {
                //This is the same as ColorChase but we use 'Wheel(position)' instead of 'color'
                strip.setPixelColor(i, Wheel(i * (384 / strip.numPixels()))); // Set new pixel 'on'
                Thread.Sleep((int)(FadeTime() * 1000));
                for (int j = 0; j <= 2; j++)
                {
                    strip.setPixelColor(i - j, colorLerp(Wheel((i - j) * (384 / strip.numPixels())), offColor, 1.0f / 3.0f * (j + 1)));
                }
            }
        }

        //##### Pattern routine #####
        //Fade the entire strip through the entire rainbow
        public void RainbowFade()
        {
            //384 is the number of colors in the color wheel
            for (int i = 0; i < 384; i++)
            {
                for (int j = 0; j < strip.numPixels(); j++)
                {
                    strip.setPixelColor(j, Wheel(i));
                }
                Thread.Sleep((int)(FadeTime() * 1000 / 128.0f));
            }
        }

        //##### Pattern routine #####
        //Rainbow wipes everything onto the strip, then fades the whole strip out
        //Does the same in the opposite direction
        public void TasteTheRainbow()
        {
            //Color in all the pixels with a delay between them
            for (int i = 0; i < strip.numPixels(); i++)
            {
                //Get the color at the fractional position on the wheel
                strip.setPixelColor(i, Wheel(i * (384 / strip.numPixels())));
                Thread.Sleep((int)(FadeTime() * 1000 / (strip.numPixels() / 5)));
            }
            //Fade off the color 1/10th of the way, then do it 10 times
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < strip.numPixels(); i++)
                {
                    //fade between the color at the fractional position on the wheel and the off-color another 10th of the way
                    strip.setPixelColor(i, colorLerp(Wheel(i * (384 / strip.numPixels())), offColor, 1.0f / 10.0f * (j + 1)));
                }
                //this fade time is *100 instead of *1000 since we're doing this 10 times. We want to retain the same fade time
                Thread.Sleep((int)(FadeTime() * 100 / (strip.numPixels() / 5)));
            }
            //repeat the same procedure but wipe in the other direction
            for (int i = strip.numPixels() - 1; i >= 0; i--)
            {
                strip.setPixelColor(i, Wheel(i * (384 / strip.numPixels())));
                Thread.Sleep((int)(FadeTime() * 1000 / (strip.numPixels() / 5)));
            }
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < strip.numPixels(); i++)
                {
                    strip.setPixelColor(i, colorLerp(Wheel(i * (384 / strip.numPixels())), offColor, 1.0f / 10.0f * (j + 1)));
                }
                Thread.Sleep((int)(FadeTime() * 100 / (strip.numPixels() / 5)));
            }
        }

        //##### Pattern routine #####
        //Does that fancy thing you see on tacky theater signs
        public void TheaterCrawl()
        {
            //Turn everything to an initial color
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, offColor);
            }

            //Since we'll be turning on every third pixel, we only need to do this 3 times to loop cleanly
            for (int q = 0; q < 3; q++)
            {
                for (int i = 0; i < strip.numPixels(); i += 3)
                {
                    strip.setPixelColor(i + q, color);    //turn every third LEDs on
                }

                Thread.Sleep((int)(FadeTime() * 1000));

                for (int i = 0; i < strip.numPixels(); i += 3)
                {
                    strip.setPixelColor(i + q, offColor); //turn those LEDs off to prepare for the next move
                }
            }
        }

        //##### Pattern routine #####
        //Wipes a color across the strip, then off
        public void ColorWipe()
        {
            //Turn the strip to an initial color (can be off, #000000)
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, offColor);
            }
            //Turn on one LED, then pause a while
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, color);
                Thread.Sleep((int)(FadeTime() * 1000));
            }
            //The strip is completely on. Now do the same thing, turning the LED off this time
            for (int i = 0; i < strip.numPixels(); i++)
            {
                strip.setPixelColor(i, offColor);
                Thread.Sleep((int)(FadeTime() * 1000));
            }
        }

        //##### Pattern routine #####
        //Fades from green to blue to purple.
        //Not super relevent, just showing off usage of ColorFade as a subroutine
        public void GreenBluePurple()
        {
            ColorFade(Colors.Green, Colors.Blue);
            ColorFade(Colors.Blue, Colors.Purple);
            ColorFade(Colors.Purple, Colors.Green);
        }

        //##### Pattern routine #####
        //Fades between 2 colors.
        void ColorFade(Color init, Color target)
        {
            //turn all LEDs to starting color
            for (int i = 0; i < strip.numPixels(); i++) strip.setPixelColor(i, init);
            //in 30 stages, between t=0 and t=1, interpolate between the two colors
            //obviously the number of stages and the fade time can be adjusted as needed
            for (double q = 0; q <= 1; q += 1.0f / 30.0f)
            {
                for (int i = 0; i < strip.numPixels(); i++)
                {
                    strip.setPixelColor(i, colorLerp(init, target, q));
                }
                Thread.Sleep((int)(FadeTime() * 1000 / 30.0f));
            }
        }

        //Math helper function. Gets a color at a position in the color wheel, 0 to 384.
        //0-127: shades between red and green
        //128-255: shades between green and blue
        //256-384: shades between blue and red
        Color Wheel(int WheelPos)
        {
            byte r = 0, g = 0, b = 0;
            switch (WheelPos / 128)
            {
                case 0:
                    r = (byte)(127 - WheelPos % 128);   //Red down
                    g = (byte)(WheelPos % 128);      // Green up
                    b = 0;                  //blue off
                    break;
                case 1:
                    g = (byte)(127 - WheelPos % 128);  //green down
                    b = (byte)(WheelPos % 128);      //blue up
                    r = 0;                  //red off
                    break;
                case 2:
                    b = (byte)(127 - WheelPos % 128);  //blue down 
                    r = (byte)(WheelPos % 128);      //red up
                    g = 0;                  //green off
                    break;
            }
            return Color.FromRgb(r, g, b);
        }

        //Math helper function. Interpolates linearly between two colors over t=0 to t=1
        Color colorLerp(Color init, Color target, double t)
        {
            byte R = (byte)((1 - t) * init.R + target.R * t);
            byte G = (byte)((1 - t) * init.G + target.G * t);
            byte B = (byte)((1 - t) * init.B + target.B * t);
            return Color.FromRgb(R, G, B);
        }
        
        //Stops the executing pattern
        public void Stop()
        {
            runningThread.Abort();
        }

        //Time it takes for an LED to fade off
        double FadeTime()
        {
            double ct = 0;
            strip.Dispatcher.Invoke(() =>
            {
                ct = strip.parent.FadeTime;
            }
            );
            return ct;
        }
    }
}
