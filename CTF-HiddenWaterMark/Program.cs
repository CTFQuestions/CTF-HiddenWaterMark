using System.Drawing;
using System.Drawing.Drawing2D;
using static System.Net.Mime.MediaTypeNames;
namespace CTF_HiddenWaterMark
{
    internal class Program
    {
        private const int WIDTH = 1200;
        private const int HEIGHT = 300;
        private static Random rd = new Random();
        private const int PROBABILITY = 84; // 0 to 100

        private const int LOW_THRESHOLD = 220;

        private const int LOW_THRESHOLD_NORMAL = 250;

        private const int HIGH_THRESHOLD = 254;
        // 今天我就要古法编程
        static void Main(string[] args)
        {
            Bitmap bmp = new Bitmap(WIDTH, HEIGHT);
            WhiteBmp(bmp, WIDTH, HEIGHT);


            Bitmap referenceBmp = new Bitmap(WIDTH, HEIGHT);
            WhiteBmp(referenceBmp, WIDTH, HEIGHT);
            WriteText(referenceBmp, "c t f { a b c d e }", 0, HEIGHT / 2, WIDTH, HEIGHT / 2, Brushes.Black, "8514oem", 60); // abcde is most easy to recognize

            referenceBmp.Save("ref.png");

            int totalFlagPixels = 0;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    int singleProb = 0;
                    Color refC = referenceBmp.GetPixel(i, j);
                    if (refC.R > 5)
                    {
                        totalFlagPixels++;
                        singleProb = PROBABILITY;
                    }
                    else
                    {
                        singleProb = 100 - PROBABILITY;
                    }
                    if (rd.Next(0, 100) < singleProb)
                    {
                        int low = rd.Next(LOW_THRESHOLD, HIGH_THRESHOLD);

                        if (rd.Next(0, 3) == 0)
                        {
                            bmp.SetPixel(i, j, Color.FromArgb(low, rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD), rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD)));
                        }
                        else
                        {
                            if (rd.Next(0, 2) == 0)
                            {
                                bmp.SetPixel(i, j, Color.FromArgb(rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD), low, rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD)));
                            }
                            else
                            {
                                bmp.SetPixel(i, j, Color.FromArgb(rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD), rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD), rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD)));
                            }
                        }
                    }
                    else
                    {
                        bmp.SetPixel(i, j, Color.FromArgb(rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD), rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD), rd.Next(LOW_THRESHOLD_NORMAL, HIGH_THRESHOLD)));

                    }



                    // extreme noise
                    if(rd.Next(0, 9) == 5 && j > 3)
                    {
                        // Console.WriteLine(i);
                        bmp.SetPixel(i, j, GetRdColor());
                    }
                }
            }


            Console.WriteLine(totalFlagPixels);


            WriteText(bmp, "Hello there!", 0, 0, WIDTH, HEIGHT / 2, Brushes.Red, "Tahoma", 100);
            WriteText(bmp, "The threshold is 250：)", 0, (int)(HEIGHT / 1.1), WIDTH, (int)(HEIGHT * 0.1), Brushes.Red, "8514oem", 20);



            string binaryMsg = "01000110011011000110000101100111001000000110100101110011001000000110111001101111011101000010000001101000011001010111001001100101"; // Flag is not here

            int _i = 0;
            foreach (byte b in binaryMsg) { 
                if(b == '0')
                {
                    bmp.SetPixel(_i, 0, Color.FromArgb(254, 255, 255));
                }
                else
                {
                    bmp.SetPixel(_i, 0, Color.FromArgb(255, 255, 255));
                }
                _i++;
            }
            // for (int i = 0; i < 30; i++) bmp.SetPixel(i, 0, Color.FromArgb(rd.Next(254, 255 + 1), 255, 255)); // Trap for those who cheat with AI, AI would decode this first and ended up with burning token


            bmp.Save("output.png");

        }

        static Color GetRdColor()
        {
            return Color.FromArgb(rd.Next(0, 255), rd.Next(0, 255), rd.Next(0, 255));
        }

        static void WhiteBmp(Bitmap bmp, int w, int h)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    bmp.SetPixel(i, j, Color.White);
                }
            }
        }

        static void WriteText(Bitmap bmp, string text, int x, int y, int width, int height, Brush brush, string font, int font_size)
        {
            // Source - https://stackoverflow.com/a/6311628
            // Posted by danyolgiax, modified by community. See post 'Timeline' for change history
            // Retrieved 2026-07-28, License - CC BY-SA 3.0

            RectangleF rectf = new RectangleF(x, y, width, height);

            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(text, new System.Drawing.Font(font, font_size), brush, rectf);

            g.Flush();

        }
    }
}
