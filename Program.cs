using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Encoder = System.Drawing.Imaging.Encoder;

namespace FontImageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.Green;
            var path = string.Format(@"{0}\fonts\", new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory);//Server.MapPath("~/fonts/custom/");
            //var path = @"C:\Users\Emre\Downloads\10000 Huge Collection of Fonts - Honest\FreeBestFonts";
            var fontList = GetFontFiles(path);

            //GenerateFontHeaderImage(fontList);
            //GenerateFontFirstLetterImage(fontList);
            GeneratePoster(fontList);
        }

        private static void GeneratePoster(List<string> fontFiles)
        {
            var fontList = new List<FontInfo>();

            if (ExistFontList != null)
            {
                fontList.AddRange(ExistFontList);
            }

            var bgColor = Color.White;
            var frColor = Color.Black;

            var tupleFonts = fontFiles.Select(fontFile => new FileInfo(fontFile)).Select(fi => new Tuple<string, string>(fi.Name, fi.FullName)).ToList();

            var lorem1th = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque in mi sapien. Fusce efficitur venenatis ultricies. Etiam commodo vestibulum odio sed accumsan. Maecenas sed accumsan risus. Aenean non erat malesuada, placerat metus id, faucibus ligula. Nunc ut porttitor lorem. Sed massa purus, congue blandit massa in, consequat imperdiet libero.";
            var lorem2nd = "Cras nibh magna, dapibus id accumsan ac, fermentum vitae neque. Integer pellentesque condimentum nulla sed finibus. Morbi posuere augue sit amet tellus condimentum, sed suscipit mi fermentum. Curabitur ut nisi odio. Curabitur laoreet diam vel accumsan eleifend. Etiam hendrerit hendrerit velit, non venenatis leo sollicitudin a. Phasellus sodales lacus sed enim volutpat efficitur. ";
            var lowerCase = "abcdefghijklmnopqrstuvwxyz";
            var upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numbers = "0 1 2 3 4 5 6 7 8 9";

            var fonts = new PrivateFontCollection();
            tupleFonts.ForEach(x => fonts.AddFontFile(x.Item2));

            for (var index = 0; index < fonts.Families.Count(); index++)
            {
                var font = fonts.Families[index];

                if (fontList.FirstOrDefault(x => x.FontName == font.Name) != null)
                {
                    continue;
                }

                var f = new Font(font, 14);
                var bmp = new Bitmap(600, 600);
                Graphics graphic = Graphics.FromImage(bmp);
                graphic.Clear(bgColor);

                graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                var size = graphic.MeasureString(lorem1th, f, 500);

                var height = Convert.ToInt32(size.Height);

                var loremText = lorem1th;
                if (height > 200)
                {
                    loremText = loremText.Substring(0, loremText.Length / 2) + "...";
                }

                //Lorem Metni
                graphic.DrawString(loremText, f, new SolidBrush(frColor), new Rectangle(50, 300, 500, 400));

                f = new Font(font, 24);
                var stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                //Font Header
                graphic.DrawString(font.Name, f, new SolidBrush(frColor), new Rectangle(0, 0, 600, 150), stringFormat);

                //Horizontal Line
                graphic.FillRectangle(new SolidBrush(frColor), 50, 250, 500, 1);

                //Büyük harfler
                f = new Font(font, 12, FontStyle.Regular);
                graphic.DrawString(upperCase, f, new SolidBrush(frColor), new Rectangle(0, 100, 600, 50), stringFormat);

                //Küçük harfler
                graphic.DrawString(lowerCase, f, new SolidBrush(frColor), new Rectangle(0, 150, 600, 50), stringFormat);

                //f = new Font(font, 12, FontStyle.Regular);
                //graphic.DrawString(lorem2nd, f, new SolidBrush(frColor), new Rectangle(400, 250, 150, 600));

                f = new Font(font, 14);
                //graphic.FillRectangle(Brushes.Brown, 50, 600, 350, 50);
                graphic.DrawString(numbers, f, new SolidBrush(frColor), new Rectangle(0, 200, 600, 50), stringFormat);


                var posterImageName = string.Format("{0}_poster.png", font.Name.Replace(" ", "_"));
                var posterFileName = string.Format(@"{0}\img\{1}", new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory, posterImageName);

                FileInfo fi = new FileInfo(posterFileName);
                if (!fi.Directory.Exists)
                {
                    Directory.CreateDirectory(fi.Directory.FullName);
                }

                //bmp.Save(posterFileName, ImageFormat.Png);
                SaveImage(posterFileName, bmp);

                var headerImageName = string.Format("{0}_header.png", font.Name.Replace(" ", "_"));
                var headerFileName = string.Format(@"{0}\img\{1}", new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory, headerImageName);


                f = new Font(font, 36);

                bmp = new Bitmap(1, 1);

                graphic = Graphics.FromImage(bmp);

                //SizeF size = graphic.MeasureString(text, f);
                var width = 700;//Convert.ToInt32(size.Width);
                height = 90;//Convert.ToInt32(size.Height);

                bmp = new Bitmap(width, height);

                graphic = Graphics.FromImage(bmp);
                graphic.Clear(bgColor);

                graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphic.FillRectangle(new SolidBrush(bgColor), 0, 0, width, height);

                graphic.DrawString(font.Name, f, new SolidBrush(frColor), new Rectangle(0, 0, bmp.Width, bmp.Height), stringFormat);

                fi = new FileInfo(headerFileName);
                if (!fi.Directory.Exists)
                {
                    Directory.CreateDirectory(fi.Directory.FullName);
                }

                //bmp.Save(headerFileName, System.Drawing.Imaging.ImageFormat.Png);
                SaveImage(headerFileName, bmp);

                var otherImageName = string.Format("{0}_other.png", font.Name.Replace(" ", "_"));
                var otherFileName = string.Format(@"{0}\img\{1}", new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory, otherImageName);


                f = new Font(font, 24);

                bmp = new Bitmap(1, 1);

                graphic = Graphics.FromImage(bmp);

                //SizeF size = graphic.MeasureString(text, f);
                width = 263;//Convert.ToInt32(size.Width);
                height = 158;//Convert.ToInt32(size.Height);

                bmp = new Bitmap(width, height);

                graphic = Graphics.FromImage(bmp);
                graphic.Clear(bgColor);

                graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphic.FillRectangle(new SolidBrush(bgColor), 0, 0, width, height);

                graphic.DrawString(font.Name, f, new SolidBrush(frColor), new Rectangle(0, 0, bmp.Width, bmp.Height), stringFormat);

                fi = new FileInfo(otherFileName);
                if (!fi.Directory.Exists)
                {
                    Directory.CreateDirectory(fi.Directory.FullName);
                }

                //bmp.Save(otherFileName, System.Drawing.Imaging.ImageFormat.Png);
                SaveImage(otherFileName, bmp);

                var fontInfo = new FontInfo();
                fontInfo.FontName = font.Name;
                fontInfo.CreatedDate = DateTime.Now;

                var y = tupleFonts.FirstOrDefault(x => x.Item1.Substring(0, x.Item1.Length - 3) == font.Name);
                fontInfo.FontFile = new FileInfo(fontFiles[index]).Name;

                var fontImageInfo = new FontImageInfo();
                fontImageInfo.ImageType = "poster";
                fontImageInfo.ImageFile = posterImageName;

                fontInfo.FontImageInfo = new List<FontImageInfo>();
                fontInfo.FontImageInfo.Add(fontImageInfo);

                fontImageInfo = new FontImageInfo();
                fontImageInfo.ImageType = "header";
                fontImageInfo.ImageFile = headerImageName;
                fontInfo.FontImageInfo.Add(fontImageInfo);

                fontImageInfo = new FontImageInfo();
                fontImageInfo.ImageType = "other";
                fontImageInfo.ImageFile = otherImageName;
                fontInfo.FontImageInfo.Add(fontImageInfo);

                fontList.Add(fontInfo);
                 
            }

            var jsonData = JsonConvert.SerializeObject(fontList.OrderByDescending(x => x.CreatedDate).ToList(), Formatting.Indented);
            File.WriteAllText(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName + "\\data\\jsonData.json", jsonData);
        }

        private static List<string> GenerateFontFirstLetterImage(List<string> fontFiles)
        {
            var retVal = new List<string>();
            var alphabet = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            var fonts = new PrivateFontCollection();
            fontFiles.ForEach(x => fonts.AddFontFile(x));

            foreach (var font in fonts.Families)
            {
                //var imageFolder = Server.MapPath(string.Format("~/Content/img/{0}/lowercase/", font.Name));
                var imageFolder = string.Format(@"{0}\img\{1}\lowercase\", new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory, font.Name);
                foreach (var letter in alphabet)
                {
                    var imageName = string.Format("{0}{1}.png", imageFolder, letter);
                    SaveStringToImage(imageName, letter, font);
                    Console.WriteLine(string.Format("{0} created", imageName));
                }
                //imageFolder = Server.MapPath(string.Format("~/Content/img/{0}/uppercase/", font.Name));
                imageFolder = string.Format(@"{0}\img\{1}\uppercase\", new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory, font.Name);
                foreach (var letter in alphabet)
                {
                    var imageName = string.Format("{0}{1}.png", imageFolder, letter.ToUpperInvariant());
                    SaveStringToImage(imageName, letter.ToUpperInvariant(), font);
                    Console.WriteLine(string.Format("{0} created", imageName));
                }
            }

            return retVal;
        }
        public static List<FontInfo> ExistFontList
        {
            get
            {
                var filePath =
                    new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName + "\\data\\jsonData.json";

                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }


                var jsonText = File.ReadAllText(filePath);
                var jsonData = JsonConvert.DeserializeObject<List<FontInfo>>(jsonText);

                return jsonData;
            }
        }
        private static List<string> GenerateFontHeaderImage(List<string> fontFiles)
        {
            var retVal = new List<string>();

            var fonts = new PrivateFontCollection();
            fontFiles.ForEach(x => fonts.AddFontFile(x));

            foreach (var font in fonts.Families)
            {
                //var imageFolder = Server.MapPath(string.Format("~/Content/img/{0}/header/", font.Name));
                var imageFolder = string.Format(@"{0}\img\header\", new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory);
                var imageName = string.Format("{0}{1}.png", imageFolder, font.Name);
                SaveStringToImage(imageName, font.Name, font);
                Console.WriteLine(string.Format("{0} created", imageName));
            }

            return retVal;
        }

        private static void SaveStringToImage(string fileName, string text, FontFamily fontFamily)
        {

            var bgColor = Color.FromArgb(51, 51, 51);
            var frColor = Color.FromArgb(106, 197, 169);

            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            var f = new Font(fontFamily, 48);

            var bmp = new Bitmap(1, 1);

            Graphics graphic = Graphics.FromImage(bmp);

            SizeF size = graphic.MeasureString(text, f);
            var width = 700;//Convert.ToInt32(size.Width);
            var height = 300;//Convert.ToInt32(size.Height);

            bmp = new Bitmap(width, height);

            graphic = Graphics.FromImage(bmp);
            graphic.Clear(bgColor);

            graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            graphic.FillRectangle(new SolidBrush(bgColor), 0, 0, width, height);

            graphic.DrawString(text, f, new SolidBrush(frColor), new Rectangle(0, 0, bmp.Width, bmp.Height), stringFormat);

            FileInfo fi = new FileInfo(fileName);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.Directory.FullName);
            }

            //bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            SaveImage(fileName, bmp);
            graphic.Clear(Color.LightSlateGray);

        }

        private static List<string> GetFontFiles(string path)
        {
            var retVal = new List<string>();

            var directoryInfo = new DirectoryInfo(path);
            var fileInfo = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            retVal.AddRange(fileInfo.Select(x => string.Format("{1}", path, x.FullName)));

            return retVal;
        }

        private static void SaveImage(string path, Image img, int quality = 100)
        {
            var qualityParam = new EncoderParameter(Encoder.Quality, quality);

            var jpegCodec = GetEncoderInfo("image/png");

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);

        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(t => t.MimeType == mimeType);
        }

    }
}
