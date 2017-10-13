// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Utilites.Security
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Security.Cryptography;

    public class Captcha
    {
        #region Fields

        // A-Z, but without letters that look similar to other letters (e.g. U, V) or
        // letters that resemble digits (e.g. O).
        private const string sLetters = "ABDEFHJKLMNSTXYZ";

        #endregion Fields

        #region Methods

        public static bool IsValidCaptcha(string key, string userInput)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (string.Compare(key, userInput, true) != 0)
                return false;

            return true;
        }

        // Number of letters displayed in CAPTCHA graphic.
        /// Returns the letters that will be displayed in the CAPTCHA graphic.
        /// <returns>text in CAPTCHA bitmap</returns>
        public static string RandomText(int length)
        {
            // Generate random bytes.
            RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();

            byte[] randomBytes = new byte[length];

            rngCSP.GetBytes(randomBytes);

            // Convert random bytes to letters.
            char[] letters = new char[randomBytes.Length];

            for (int i = 0; i < letters.Length; i++)
            {
                int index = randomBytes[i] % sLetters.Length;

                letters[i] = sLetters[index];
            }

            return new string(letters);
        }

        private static Color GetRandomColor()
        {
            Random randonGen = new Random(Guid.NewGuid().GetHashCode());
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[randonGen.Next(names.Length)];
            Color randomColor = Color.FromKnownColor(randomColorName);
            return randomColor;
        }

        #endregion Methods

        #region Nested Types

        public class CustomCanvas : XCaptcha.Canvas
        {
            public CustomCanvas()
                : base(150 /* width*/, 50 /*height*/, new HatchBrush(HatchStyle.Percent50, Color.LightGray, Color.LightGray) /* brush */)
            {
            }
        }

        public class CustomDistort : XCaptcha.Distort
        {
            public override GraphicsPath Create(GraphicsPath path, XCaptcha.ICanvas canvas)
            {
                var random = new Random(Guid.NewGuid().GetHashCode());
                var rect = new Rectangle(0, 0, canvas.Width, canvas.Height);

                const float wrapFactor = 8F;

                PointF[] points =
                {
                    new PointF(random.Next(rect.Width) / wrapFactor,random.Next(rect.Height) / wrapFactor),
                    new PointF(rect.Width - random.Next(rect.Width) / wrapFactor, random.Next(rect.Height) / wrapFactor),
                    new PointF(random.Next(rect.Width) / wrapFactor, rect.Height - random.Next(rect.Height) / wrapFactor),
                    new PointF(rect.Width - random.Next(rect.Width) / wrapFactor, rect.Height - random.Next(rect.Height) / wrapFactor)
                };

                var matrix = new Matrix();
                matrix.Translate(0F, 0F);

                path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

                return path;
            }

        }

        public class CustomNoise : XCaptcha.Noise
        {
            public CustomNoise()
                : base(new HatchBrush(HatchStyle.SmallConfetti, Color.Gray, Color.Gray))
            {
            }


            public override void Create(Graphics graphics, XCaptcha.ICanvas canvas)
            {
                var random = new Random(Guid.NewGuid().GetHashCode());

                for (var i = 0; i < (canvas.Width * canvas.Height / 10); i++)
                {
                    var x = random.Next(canvas.Width);
                    var y = random.Next(canvas.Height);
                    var w = random.Next(3);
                    var h = random.Next(3);

                    graphics.FillEllipse(Brush, x, y, w, h);
                }
            }
        }

        public class CustomTextStyle : XCaptcha.TextStyle
        {
            public CustomTextStyle()
                : base(new Font("Consolas", 36, FontStyle.Regular) /*font*/,
                new HatchBrush(HatchStyle.Percent50, Color.CadetBlue, Color.Blue) /*brush*/)
            {
            }
        }

        #endregion Nested Types
    }
}
