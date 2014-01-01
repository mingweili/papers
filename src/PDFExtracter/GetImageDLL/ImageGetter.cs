using System.Drawing;
using pdftron.PDF;
using System.Drawing.Imaging;
using System;

namespace GetImageDLL
{
    public class ImageGetter
    {
        static PDFDoc doc;
        static string currentFileName = string.Empty;
        static int currentPageIndex = -1;
        static PDFDraw draw = new PDFDraw();
        static Page page;
        static Bitmap pageImage;

        public static void setDPI(double dpi)
        {
            draw.SetDPI(dpi);
        }
        public static Bitmap getImage(string fileName, int pageIndex, Rectangle rect, PixelFormat imageFormat)
        {
            //判断文件是否发生了改变
            if (!currentFileName.Equals(fileName))
            {
                currentFileName = fileName;
                doc = new PDFDoc(currentFileName);
                currentPageIndex = -1;
            }
            //判断页码是否发生了改变
            if (currentPageIndex != pageIndex)
            {
                currentPageIndex = pageIndex;
                page = doc.GetPage(currentPageIndex);
                Console.WriteLine("DPI:{0}", 1 / page.GetUserUnitSize());

                pageImage = null;
                //draw.SetImageSize((int)page.GetPageWidth(), (int)page.GetPageHeight());
                pageImage = draw.GetBitmap(page);
            }
            //处理坐标的转化
            rect.Y = (int)page.GetPageHeight() - rect.Y;
            Bitmap image = pageImage.Clone(rect, imageFormat);

            return image;
        }

        ///思路：
        ///1.判断文件名，如果文件名未变，不需做出改变
        ///2.如果改变了，则新建一个PDFDoc
        ///3.判断pageIndex，如果没有改变，不需做出改变
        ///4.如果改变了，重新获取设置pageSize
        ///5.返回结果
    }
}
