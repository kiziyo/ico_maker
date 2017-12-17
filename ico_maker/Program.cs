using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ico_maker
{
    class Program
    {
        const int HEADER_SIZE = 6;
        const int DIRECTORY_SIZE = 16;


        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                foreach (string arg in args)
                {
                    Console.WriteLine(arg);
                }

                Console.WriteLine("USAGE: ico_maker.exe IMAGE_DIRECTORY_PATH ICO_FILENAME");
                return;
            }

            string directory_path = args[0];
            string ico_filname = args[1];

            if (!Directory.Exists(directory_path))
            {
                Console.WriteLine("### " + directory_path + " is not directory!");
                Console.WriteLine("USAGE: ico_maker.exe IMAGE_DIRECTORY_PATH");
                return;
            }

            string[] filepathes = Directory.GetFiles(directory_path);

            if (filepathes.Length < 1)
            {
                Console.WriteLine("### There is no any file :" + directory_path);
                Console.WriteLine("USAGE: ico_maker.exe IMAGE_DIRECTORY_PATH");
                return;
            }

            BinaryWriter bw = new BinaryWriter(File.Open(ico_filname, FileMode.OpenOrCreate));

            // HEADERチャンク
            bw.Write((UInt16)0);                            // reserved 常に0
            bw.Write((UInt16)1);                            // ICOファイルなら1, CURファイルなら2
            bw.Write((UInt16)filepathes.Length);            // 画像ファイル数

            int data_offset = 0;
            data_offset += HEADER_SIZE;
            data_offset += DIRECTORY_SIZE * filepathes.Length;

            // DIRECTORYチャンク
            foreach (string filepath in filepathes)
            {
                Image image = Image.FromFile(filepath);

                bw.Write((byte)GetImageWidth(image));       // イメージ幅[pixel] 256なら0
                bw.Write((byte)GetImageHeight(image));      // イメージ高さ[pixel] 256なら0
                bw.Write((byte)GetNumberOfColor(image));    // 色数 256以上なら0
                bw.Write((byte)0);                          // reserved 常に0
                bw.Write((UInt16)0);                        // カラープレーン数 0または1
                bw.Write((UInt16)GetBitPerPixel(image));    // ピクセル毎のビット深度

                FileInfo file_info = new FileInfo(filepath);
                
                bw.Write((Int32)file_info.Length);          // イメージデータのバイト数
                bw.Write((Int32)data_offset);               // イメージデータまでのファイル内オフセット

                data_offset += (Int32)file_info.Length;
            }

            // DATAチャンク
            foreach (string filepath in filepathes)
            {
                byte[] data = File.ReadAllBytes(filepath);
                bw.Write(data);                             // イメージデータ
            }

            bw.Close();
        }

        static byte GetImageWidth(Image image)
        {
            int image_width = image.Width;

            if (image_width > 255)
            {
                image_width = 0;
            }

            return (byte)image_width;
        }

        static byte GetImageHeight(Image image)
        {
            int image_height = image.Height;

            if (image_height > 255)
            {
                image_height = 0;
            }

            return (byte)image_height;
        }
        
        static byte GetNumberOfColor(Image image)
        {
            byte number_of_color = 0;

            switch (image.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    number_of_color = 2;
                    break;
                case PixelFormat.Format4bppIndexed:
                    number_of_color = 16;
                    break;
            }

            return number_of_color;
        }

        static UInt16 GetBitPerPixel(Image image)
        {
            UInt16 bit_per_pixel = 0;

            switch (image.PixelFormat)
            {
                case PixelFormat.Format16bppArgb1555:
                    bit_per_pixel = 16;
                    break;
                case PixelFormat.Format16bppGrayScale:
                    bit_per_pixel = 16;
                    break;
                case PixelFormat.Format16bppRgb555:
                    bit_per_pixel = 16;
                    break;
                case PixelFormat.Format16bppRgb565:
                    bit_per_pixel = 16;
                    break;
                case PixelFormat.Format1bppIndexed:
                    bit_per_pixel = 1;
                    break;
                case PixelFormat.Format24bppRgb:
                    bit_per_pixel = 24;
                    break;
                case PixelFormat.Format32bppArgb:
                    bit_per_pixel = 32;
                    break;
                case PixelFormat.Format32bppPArgb:
                    bit_per_pixel = 32;
                    break;
                case PixelFormat.Format32bppRgb:
                    bit_per_pixel = 32;
                    break;
                case PixelFormat.Format48bppRgb:
                    bit_per_pixel = 48;
                    break;
                case PixelFormat.Format4bppIndexed:
                    bit_per_pixel = 4;
                    break;
                case PixelFormat.Format64bppArgb:
                    bit_per_pixel = 64;
                    break;
                case PixelFormat.Format64bppPArgb:
                    bit_per_pixel = 64;
                    break;
                case PixelFormat.Format8bppIndexed:
                    bit_per_pixel = 8;
                    break;
                case PixelFormat.Canonical:
                    bit_per_pixel = 32;
                    break;
            }

            return bit_per_pixel;
        }
    }
}
