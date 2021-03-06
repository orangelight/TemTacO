﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace TemTacO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Global Variables
        List<TemTem> TemTems = new List<TemTem>();
        List<string> TemNames = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            //Fill lists with data from CSVs
            TemTems = PopulateList();
            TemNames = GetTemNameList();
            //Start the screen checking function.
            StartScreenChecker();
        }

        private void StartScreenChecker()
        {
            //DispatcherTimer setup
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            //Run the function every 3 seconds.
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Scan Screen for Tems
            ScanScreenTem(false);

            //Force the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private void ScanScreenTem(bool save)
        {
            Bitmap memoryImage;
            memoryImage = new Bitmap(150, 35);
            System.Drawing.Size s = new System.Drawing.Size(150, 35);

            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            //Scan TemTem Left
            memoryGraphics.CopyFromScreen(1166, 23, 0, 0, s);

            //Save image (Used for gathering Dataset)
            if (save)
            {
                string fileName = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    @"\TemTem\" +
                    DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt)") + "L.png");
                memoryImage.Save(fileName);
            }

            string TempTemScan = ImageCorrelation(memoryImage).ToString();
            //Set left Tem label text
            if (!ScanForMenu() || (EnemyTemLeft.Content.ToString() != TempTemScan && TempTemScan != ""))
            {
                EnemyTemLeft.Content = TempTemScan;
            }

            //If we found a tem update the table
            if (EnemyTemLeft.Content.ToString() != "")
            {
                TemTem TemLeft = GetMatchup(EnemyTemLeft.Content.ToString());

                LMNeutral.Content = TemLeft.TypeNeutral.ToString().TrimStart(new Char[] { '0' });
                LMFire.Content = TemLeft.TypeFire.ToString().TrimStart(new Char[] { '0' });
                LMWater.Content = TemLeft.TypeWater.ToString().TrimStart(new Char[] { '0' });
                LMNature.Content = TemLeft.TypeNature.ToString().TrimStart(new Char[] { '0' });
                LMElectric.Content = TemLeft.TypeElectric.ToString().TrimStart(new Char[] { '0' });
                LMEarth.Content = TemLeft.TypeEarth.ToString().TrimStart(new Char[] { '0' });
                LMMental.Content = TemLeft.TypeMental.ToString().TrimStart(new Char[] { '0' });
                LMWind.Content = TemLeft.TypeWind.ToString().TrimStart(new Char[] { '0' });
                LMDigital.Content = TemLeft.TypeDigital.ToString().TrimStart(new Char[] { '0' });
                LMMelee.Content = TemLeft.TypeMelee.ToString().TrimStart(new Char[] { '0' });
                LMCrystal.Content = TemLeft.TypeCrystal.ToString().TrimStart(new Char[] { '0' });
                LMToxic.Content = TemLeft.TypeToxic.ToString().TrimStart(new Char[] { '0' });
                AddColor(LeftMatchup.Children);
                LeftMatchup.Visibility = Visibility.Visible;
            }
            else
            {
                LeftMatchup.Visibility = Visibility.Collapsed;
            }

            //Scan TemTem Right
            memoryGraphics.CopyFromScreen(1564, 79, 0, 0, s);

            //Save image
            if (save)
            {
                string fileName = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    @"\TemTem\" +
                    DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt)") + "R.png");
                memoryImage.Save(fileName);
            }

            TempTemScan = ImageCorrelation(memoryImage).ToString();
            //Set left Tem label text
            if (!ScanForMenu() || (EnemyTemLeft.Content.ToString() != TempTemScan && TempTemScan != ""))
            {
                EnemyTemRight.Content = TempTemScan;
            };

            //If we found a Tem update the table
            if (EnemyTemRight.Content.ToString() != "")
            {
                TemTem TemRight = GetMatchup(EnemyTemRight.Content.ToString());

                RMNeutral.Content = TemRight.TypeNeutral.ToString().TrimStart(new Char[] { '0' });
                RMFire.Content = TemRight.TypeFire.ToString().TrimStart(new Char[] { '0' });
                RMWater.Content = TemRight.TypeWater.ToString().TrimStart(new Char[] { '0' });
                RMNature.Content = TemRight.TypeNature.ToString().TrimStart(new Char[] { '0' });
                RMElectric.Content = TemRight.TypeElectric.ToString().TrimStart(new Char[] { '0' });
                RMEarth.Content = TemRight.TypeEarth.ToString().TrimStart(new Char[] { '0' });
                RMMental.Content = TemRight.TypeMental.ToString().TrimStart(new Char[] { '0' });
                RMWind.Content = TemRight.TypeWind.ToString().TrimStart(new Char[] { '0' });
                RMDigital.Content = TemRight.TypeDigital.ToString().TrimStart(new Char[] { '0' });
                RMMelee.Content = TemRight.TypeMelee.ToString().TrimStart(new Char[] { '0' });
                RMCrystal.Content = TemRight.TypeCrystal.ToString().TrimStart(new Char[] { '0' });
                RMToxic.Content = TemRight.TypeToxic.ToString().TrimStart(new Char[] { '0' });
                AddColor(RightMatchup.Children);
                RightMatchup.Visibility = Visibility.Visible;
            }
            else
            {
                RightMatchup.Visibility = Visibility.Collapsed;
            }

            TemTacOverlay.Visibility = (LeftMatchup.Visibility == Visibility.Collapsed && RightMatchup.Visibility == Visibility.Collapsed) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AddColor(System.Windows.Controls.UIElementCollection collection)
        {
            foreach (Label label in collection)
            {
                if (label.Content.ToString() != "" && double.Parse(label.Content.ToString()) < 1)
                {
                    label.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 211, 111, 106));
                }
                else if (label.Content.ToString() != "" && double.Parse(label.Content.ToString()) > 1)
                {
                    label.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 169, 215, 157));
                }
                else
                {
                    label.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
                }
            }
        }

        private bool ScanForMenu()
        {
            Bitmap memoryImage;
            memoryImage = new Bitmap(454, 3);
            System.Drawing.Size s = new System.Drawing.Size(454, 3);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            //Scan for menu
            memoryGraphics.CopyFromScreen(471, 298, 0, 0, s);

            for (int y = 0; y < memoryImage.Height; y++)
            {
                for (int x = 0; x < memoryImage.Width; x++)
                {
                    System.Drawing.Color pixel = memoryImage.GetPixel(x, y);
                    if (pixel.R != 30 || pixel.G != 31 || pixel.B != 30 || pixel.A != 255)
                    {
                        return false;
                    }
                }
            }            
            return true;
        }

        public string ImageCorrelation(Bitmap image)
        {
            List<float> similaritiesWhite = new List<float>();
            List<float> similaritiesBlack = new List<float>();
            string[] FilePaths = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Dataset"));
            foreach (string filePath in FilePaths)
            {
                float similarWhite = 0;
                float similarBlack = 0;
                Bitmap datasetImage = new Bitmap(filePath);
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        System.Drawing.Color pixel1 = image.GetPixel(x, y);
                        System.Drawing.Color pixel2 = datasetImage.GetPixel(x, y);

                        //If there is a white pixel in the same position on both images add a point of similarity
                        if((pixel1.R == 255 && pixel1.G == 255 && pixel1.B == 255 && pixel1.A == 255) && (pixel2.R == 255 && pixel2.G == 255 && pixel2.B == 255 && pixel2.A == 255))
                        {
                            similarWhite += 1;                            
                        }
                        if ((pixel1.R < 35 && pixel1.G < 35 && pixel1.B < 35 && pixel1.A == 255) && (pixel2.R < 35 && pixel2.G < 35 && pixel2.B < 35 && pixel2.A == 255) && y < 26)
                        {
                            similarBlack += 1;
                        }
                    }
                }
                similaritiesWhite.Add(similarWhite);
                similaritiesBlack.Add(similarBlack);
            }
            //If there are less than 30 pixels equal we assume there is no Tem found.
            if (similaritiesWhite.Max() < 30 || similaritiesBlack.Max() < 30)
                return "";
            else
            {
                int WhiteIndex = similaritiesWhite.IndexOf(similaritiesWhite.Max());
                int BlackIndex = similaritiesBlack.IndexOf(similaritiesBlack.Max());
                if (WhiteIndex == BlackIndex)
                {
                    return Path.GetFileNameWithoutExtension(FilePaths[WhiteIndex]);
                }
                else
                    return "";
            }
        }
       
        //This method is used by the 'commented' button. Used for gathering dataset.
        private void BtnEnemyTemSave_Click(object sender, RoutedEventArgs e)
        {
            ScanScreenTem(true);
        }

        private List<TemTem> PopulateList()
        {
            List<TemTem> temTemps = File.ReadAllLines("Resources\\TemTemList.csv")
                                           .Skip(1)
                                           .Select(v => TemTem.FromCsv(v))
                                           .ToList();
            return temTemps;
        }

        private List<string> GetTemNameList()
        {
            List<string> temNames = File.ReadAllLines("Resources\\TemNames.csv")
                                           .Skip(1)
                                           .ToList();

            return temNames;
        }           
        

        private TemTem GetMatchup(string TemName)
        {
            int index = TemNames.IndexOf(TemName);
            TemTem TemInfo = TemTems[index];
            return TemInfo;
        }
    }
}
