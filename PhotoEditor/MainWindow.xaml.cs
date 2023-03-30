using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

using System.IO;

using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace PhotoViewer
{
    public partial class MainWindow : Window
    {
        private BitmapImage originalBitmap;
        private TransformedBitmap rotatedBitmap;
        private int rotationAngle = 0;
        private OpenFileDialog openFileDialog;

        public MainWindow()
        {
            InitializeComponent();
            openFileDialog = new OpenFileDialog();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string copyFileName = Path.Combine(Path.GetDirectoryName(fileName),
                                                   "Copy_" + Path.GetFileName(fileName));

                // Создание копии файла
                File.Copy(fileName, copyFileName, true);

                // Чтение копии изображения в память
                using (var inputStream = File.OpenRead(copyFileName))
                {
                    originalBitmap = new BitmapImage();
                    originalBitmap.BeginInit();
                    originalBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    originalBitmap.StreamSource = inputStream;
                    originalBitmap.EndInit();
                }

                // Поворот изображения
                rotatedBitmap = new TransformedBitmap(originalBitmap, new RotateTransform(rotationAngle));
                image.Source = rotatedBitmap;
            }
        }


        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            rotationAngle += 90;
            if (rotationAngle >= 360)
            {
                rotationAngle = 0;
            }
            rotatedBitmap = new TransformedBitmap(originalBitmap, new RotateTransform(rotationAngle));
            image.Source = rotatedBitmap;
        }

        public void AdjustContrast(string imagePath, float contrastLevel)
        {
            // Создание временного файла
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");

            using (var inputStream = File.OpenRead(imagePath))
            {
                using (var outputStream = File.Create(tempPath))
                {
                    // Чтение изображения в память
                    var image = Image.Load(inputStream);

                    // Установка уровня контрастности
                    image.Mutate(x => x.Contrast(contrastLevel));

                    // Сохранение измененного изображения в временный файл
                    image.Save(outputStream, new JpegEncoder());
                }
            }

            // Замена исходного файла копией
            File.Delete(imagePath);
            File.Move(tempPath, imagePath);
        }

        private void contrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (openFileDialog != null)
            {
                // Создание временного файла с новым именем
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(openFileDialog.FileName) + "_temp" + Path.GetExtension(openFileDialog.FileName));

                // Копирование оригинального файла во временный файл
                File.Copy(openFileDialog.FileName, tempFilePath, true);

                // Установка уровня контрастности для временного файла
                AdjustContrast(openFileDialog.FileName, (float)e.NewValue);

                // Чтение изображения из временного файла в память
                using (var inputStream = File.OpenRead(tempFilePath))
                {
                    originalBitmap = new BitmapImage();
                    originalBitmap.BeginInit();
                    originalBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    originalBitmap.StreamSource = inputStream;
                    originalBitmap.EndInit();
                }

                // Поворот изображения
                rotatedBitmap = new TransformedBitmap(originalBitmap, new RotateTransform(rotationAngle));
                image.Source = rotatedBitmap;

                // Удаление временного файла
                File.Delete(tempFilePath);

            }

        }

        public void AdjustBrightness(string imagePath, float brightnessLevel)
        {
            // Создание временного файла
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");

            using (var inputStream = File.OpenRead(imagePath))
            {
                using (var outputStream = File.Create(tempPath))
                {
                    // Чтение изображения в память
                    var image = Image.Load(inputStream);

                    // Установка уровня яркости
                    image.Mutate(x => x.Brightness(brightnessLevel));

                    // Сохранение измененного изображения в временный файл
                    image.Save(outputStream, new JpegEncoder());
                }
            }

            // Замена исходного файла копией
            File.Delete(imagePath);
            File.Move(tempPath, imagePath);
        }

        private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (openFileDialog != null)
            {
                // Создание временного файла с новым именем
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(openFileDialog.FileName) + "_temp" + Path.GetExtension(openFileDialog.FileName));

                // Копирование оригинального файла во временный файл
                File.Copy(openFileDialog.FileName, tempFilePath, true);

                // Установка яркости для временного файла
                AdjustBrightness(tempFilePath, (float)e.NewValue);

                // Чтение изображения из временного файла в память
                using (var inputStream = File.OpenRead(tempFilePath))
                {
                    originalBitmap = new BitmapImage();
                    originalBitmap.BeginInit();
                    originalBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    originalBitmap.StreamSource = inputStream;
                    originalBitmap.EndInit();
                }

                // Поворот изображения
                rotatedBitmap = new TransformedBitmap(originalBitmap, new RotateTransform(rotationAngle));
                image.Source = rotatedBitmap;

                // Удаление временного файла
                File.Delete(tempFilePath);

            }
        }

        private void SaveImageToFile(BitmapSource image, string fileName) 
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "JPEG Image (*.jpg)|*.jpg|PNG Image (*.png)|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.FileName = "Changed_" + Path.GetFileName(openFileDialog.FileName);
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveImageToFile(rotatedBitmap, saveFileDialog.FileName);
            }
        }


    }
}
