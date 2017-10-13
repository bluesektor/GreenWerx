// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Omni.Base.Multimedia
{


    //  [DataContract]
    public class ImageEx 
    {
        #region Properties

        //       [DataMember]
        public string Name
        {
            get; set;
        }

        //      [DataMember]
        public byte[] Stream
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes);
            return Image.FromStream(ms, true);
        }

        public static MemoryStream Base64ToStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            return new MemoryStream(imageBytes);
        }

        //Returns path to thumbnail.
        //
        public static string CreateThumbnailImage(string pathToImage, int maxSize)
        {
            string res = "";
            try
            {
                // Load image.
                Image image = Image.FromFile(pathToImage);

                // Compute thumbnail size.
                Size thumbnailSize = GetThumbnailSize(image, maxSize);

                // Get thumbnail.
                Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width,
                    thumbnailSize.Height, null, IntPtr.Zero);

                // Save thumbnail.
                FileInfo fi = new FileInfo(pathToImage);
                res = pathToImage.Replace(fi.Extension, ".tmb" + fi.Extension);
                thumbnail.Save(res);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }

            return res;
        }

        public static Size GetThumbnailSize(Image original, int maxSize)
        {
            // Maximum size of any dimension.
            //The most difficult part of generating thumbnails is probably scaling them. It isn't a good idea just to decrease all dimensions by 50% if the images are of different sizes, because then some thumbnails will be much larger than others.
            //Instead:The scaling logic here caps each dimension to 40 pixels. Based on the larger dimension, it scales the entire image.
            //Note:I tested this code with different sizes of images, both horizontally larger and vertically larger, and it worked correctly.
            int maxPixels = maxSize; 

            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }

        

        //Slow and Simple
        //Convert Time: 1,135ms
        public static Bitmap MakeGrayscale(Bitmap original)
        {
            //make an empty bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    //get the pixel from the original image
                    Color originalColor = original.GetPixel(i, j);

                    //create the grayscale version of the pixel
                    int grayScale = (int)((originalColor.R * .3) + (originalColor.G * .59)
                        + (originalColor.B * .11));

                    //create the color object
                    Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);

                    //set the new image's pixel to the grayscale version
                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            return newBitmap;
        }

        
        //Short and Sweet
        //62ms
        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
              {
              new float[] {.3f, .3f, .3f, 0, 0},
              new float[] {.59f, .59f, .59f, 0, 0},
              new float[] {.11f, .11f, .11f, 0, 0},
              new float[] {0, 0, 0, 1, 0},
              new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
    

        #endregion Methods



       
    }
}