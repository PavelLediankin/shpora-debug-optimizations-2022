using System.Drawing;
using System.Drawing.Imaging;

namespace JPEG.Images
{

    class Matrix
    {
        public readonly Pixel[,] Pixels;
        public readonly int Height;
        public readonly int Width;

        public Matrix(int height, int width)
        {
            Height = height;
            Width = width;

            Pixels = new Pixel[height, width];
            for (var i = 0; i < height; ++i)
            for (var j = 0; j < width; ++j)
                Pixels[i, j] = Pixel.RgbPixel;
        }
       
        public static explicit operator Matrix(Bitmap bmp)
        {
            unsafe
            {
                var height = bmp.Height;
                var width = bmp.Width;
                var matrix = new Matrix(height, width);
                var bitmapData = bmp.LockBits(
                    new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                    bmp.PixelFormat);
                var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (var y = 0; y < height; y++)
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (var x = 0; x < width; x+=1)
                    {
                        var k = x * bytesPerPixel;
                        var blue = currentLine[k];
                        var green = currentLine[k + 1];
                        var red = currentLine[k + 2];
                        matrix.Pixels[y, x] = new Pixel(red, green, blue, PixelFormat.RGB);
                    }
                }
                bmp.UnlockBits(bitmapData);
                return matrix;
            }
        }

        public static explicit operator Bitmap(Matrix matrix)
        {
            unsafe
            {
                var bmp = new Bitmap(matrix.Width, matrix.Height);
                var height = bmp.Height;
                var width = bmp.Width;
                var bitmapData = bmp.LockBits(
                    new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                    bmp.PixelFormat);
                var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (var y = 0; y < height; y++)
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (var x = 0; x < width; x += 1)
                    {
                        var k = x * bytesPerPixel;
                        currentLine[k] = ToByte(matrix.Pixels[y, x].B);
                        currentLine[k + 1] = ToByte(matrix.Pixels[y, x].G);
                        currentLine[k + 2] = ToByte(matrix.Pixels[y, x].R);
                    }
                }
                bmp.UnlockBits(bitmapData);

                return bmp;
            }
        }
        
       public static byte ToByte(double d)
       {
           var val = (int) d;
           if (val > byte.MaxValue)
               return byte.MaxValue;
           if (val < byte.MinValue)
               return byte.MinValue;
           return (byte)val;
       }
   }

   /*
    public static explicit operator Matrix(Bitmap bmp)
       {
           var height = bmp.Height - bmp.Height % 8;
           var width = bmp.Width - bmp.Width % 8;
           var matrix = new Matrix(height, width);

           for (var j = 0; j < height; j++)
           {
               for (var i = 0; i < width; i++)
               {
                   var pixel = bmp.GetPixel(i, j);
                   matrix.Pixels[j, i] = new Pixel(pixel.R, pixel.G, pixel.B, PixelFormat.RGB);
               }
           }

           return matrix;
       }

       public static explicit operator Bitmap(Matrix matrix)
       {
           var bmp = new Bitmap(matrix.Width, matrix.Height);

           for (var j = 0; j < bmp.Height; j++)
           {
               for (var i = 0; i < bmp.Width; i++)
               {
                   var pixel = matrix.Pixels[j, i];
                   bmp.SetPixel(i, j, Color.FromArgb(ToByte(pixel.R), ToByte(pixel.G), ToByte(pixel.B)));
               }
           }

           return bmp;
       }
    */
    }