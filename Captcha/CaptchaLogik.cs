using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Captcha
{
    public class CaptchaLogik
    {
        private const string Letters = "1234567890QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";

        public static string GenerateCaptchaCode()
        {
            Random rand = new Random();
            int maxRand = Letters.Length - 1;
            string str = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                int index = rand.Next(maxRand);
                str += Letters[index];
            }
            return str;
        }

        public static CaptchaResult GenerateCaptchaImage(int width, int height, string captchaCode)
        {
            using (Bitmap baseMap = new Bitmap(width, height))
            using (Graphics graph = Graphics.FromImage(baseMap))
            {
                Random random = new Random();
                graph.Clear(Color.AliceBlue);
                DrawCaptchaCode(graph, captchaCode, width, height);
                DrawDisorderLine(graph, random, width, height);
                AdjustRippleEffect(baseMap, width, height);

                using (MemoryStream ms = new MemoryStream())
                {
                    baseMap.Save(ms, ImageFormat.Png);
                    return new CaptchaResult { CaptchaCode = captchaCode, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };
                }
            }
        }

        private static void DrawCaptchaCode(Graphics graph, string captchaCode, int width, int height)
        {
            SolidBrush fontBrush = new SolidBrush(Color.Black);
            int fontSize = GetFontSize(width, captchaCode.Length);
            Font font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold, GraphicsUnit.World);

            for (int i = 0; i < captchaCode.Length; i++)
            {
                fontBrush.Color = GetRandomDeepColor();

                int shiftPx = fontSize / 6;

                float x = i * fontSize + random.Next(-shiftPx, shiftPx) + random.Next(-shiftPx, shiftPx);
                int maxY = height - fontSize;
                if (maxY < 0) maxY = 0;
                float y = random.Next(0, maxY);

                graph.DrawString(captchaCode[i].ToString(), font, fontBrush, x, y);
            }
        }

        private static void DrawDisorderLine(Graphics graph, Random random, int width, int height)
        {
            Pen linePen = new Pen(new SolidBrush(Color.Black), 3);

            for (int i = 0; i < random.Next(3, 5); i++)
            {
                linePen.Color = GetRandomDeepColor();

                Point startPoint = new Point(random.Next(0, width), random.Next(0, height));
                Point endPoint = new Point(random.Next(0, width), random.Next(0, height));

                graph.DrawLine(linePen, startPoint, endPoint);
            }
        }

        private static void AdjustRippleEffect(Bitmap baseMap, int width, int height)
        {
            short nWave = 6;
            int nWidth = baseMap.Width;
            int nHeight = baseMap.Height;

            Point[,] pt = new Point[nWidth, nHeight];

            for (int x = 0; x < nWidth; ++x)
            {
                for (int y = 0; y < 5; ++y)
                {
                    var xo = nWave * Math.Sin(2.0 * 3.145 * y / 128.0);
                    var yo = nWave * Math.Sin(2.0 * 3.145 * y / 128.0);

                    var newX = x + xo;
                    var newY = y + yo;

                    if (newX > 0 && newY < nWidth)
                    {
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        pt[x, y].X = 0;
                    }

                    if (newY > 0 && newY < nHeight)
                    {
                        pt[x, y].Y += (int)newY;
                    }
                    else
                    {
                        pt[x, y].Y = 0;
                    }
                }
            }
            Bitmap bSrc = (Bitmap)baseMap.Clone();

            BitmapData bitmapData = baseMap.LockBits(new Rectangle(0, 0, baseMap.Width, baseMap.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, baseMap.Width, baseMap.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

            int scanline = bitmapData.Stride;

            IntPtr scan0 = bitmapData.Scan0;
            IntPtr srcScan0 = bmSrc.Scan0;

            byte[] buffer = new byte[scanline * nHeight];
            Marshal.Copy(srcScan0, buffer, 0, buffer.Length);

            int nOffSet = bitmapData.Stride - baseMap.Width * 3;

            for (int y = 0; y < nHeight; ++y)
            {
                for (int x = 0; x < nWidth; ++x)
                {
                    var xOffSet = pt[x, y].X;
                    var yOffSet = pt[x, y].Y;

                    if (yOffSet >= 0 && yOffSet < nHeight && xOffSet >= 0 && xOffSet < nWidth)
                    {
                        int offset = (yOffSet * scanline) + (xOffSet * 3);
                        if (offset >= 0 && offset < buffer.Length)
                        {
                            Marshal.WriteByte(scan0, offset, buffer[offset]);
                            Marshal.WriteByte(scan0, offset + 1, buffer[offset + 1]);
                            Marshal.WriteByte(scan0, offset + 2, buffer[offset + 2]);
                        }
                    }
                }
                scan0 += scanline;
            }
            baseMap.UnlockBits(bitmapData);
            bSrc.UnlockBits(bmSrc);
            bSrc.Dispose();
        }

        private static int GetFontSize(int imageWidth, int captchaCodeCount)
        {
            var averageSize = imageWidth / captchaCodeCount;
            return Convert.ToInt32(averageSize);
        }

        private static Color GetRandomDeepColor()
        {
            int redLow = 160, greenLow = 100, blueLow = 160;
            return Color.FromArgb(random.Next(redLow), random.Next(greenLow), random.Next(blueLow));
        }

        private static Random random = new Random();
    }
}
