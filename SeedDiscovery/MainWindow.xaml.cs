using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;

namespace SeedDiscovery
{
    public partial class MainWindow : Window
    {
        private string seedBoxContent;
        private string prefixContent = "";
        private int prefixValue = 0;
        private bool prefixCharExists = false;
        private string generationInfo;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SeedBoxSet(string text)
        {
            seedBoxContent = text;
        }

        private void SeedBoxAdd(string text)
        {
            seedBoxContent = seedBoxContent + "\n" + text;
        }

        private void SeedBoxUpdate()
        {
            seedBox.Text = seedBoxContent;
        }

        private int Normalize(int num)
        {
            if (num == 33)
            {
                num = 48;
            }
            else if (num == 58)
            {
                num = 65;
            }
            return num;
        }

        private void fileSelect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".dat";
            dialog.Filter = "Replay Files (*.dat)|*.dat";
            dialog.CheckFileExists = true;
            string initial = "C:\\";
            bool cryptExists = false;
            if (Directory.Exists("C:\\Program Files (x86)\\Steam\\SteamApps\\common\\Crypt of the NecroDancer\\"))
            {
                initial += "Program Files (x86)\\Steam\\SteamApps\\common\\Crypt of the NecroDancer\\";
                cryptExists = true;
            }
            else if (Directory.Exists("C:\\Program Files\\Steam\\SteamApps\\common\\Crypt of the NecroDancer\\"))
            {
                initial += "Program Files\\Steam\\SteamApps\\common\\Crypt of the NecroDancer\\";
                cryptExists = true;
            }
            else if (Directory.Exists("C:\\Program Files (x86)\\Steam\\SteamApps\\common\\"))
            {
                initial += "Program Files (x86)\\Steam\\SteamApps\\common\\";
            }
            else if (Directory.Exists("C:\\Program Files\\Steam\\SteamApps\\common\\"))
            {
                initial += "Program Files\\Steam\\SteamApps\\common\\";
            }
            if (cryptExists && Directory.Exists(initial + "replays\\"))
            {
                initial += "replays\\";
            }
            dialog.InitialDirectory = initial;
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string path = dialog.FileName;
                filePathBox.Text = path;
                filePathBox.Visibility = System.Windows.Visibility.Visible;
                PopulateSeeds(path);
            }
        }

        private void PopulateSeeds(string path)
        {
            bool largerBox = false;
            string prefix = prefixContent;
            int min = 0;
            int max = 10;
            int inputMin = 0;
            if (prefix.Equals(""))
            {
                min = 1;
                max = 4;
            }
            else
            {
                min = prefix.Length;
                max = prefix.Length + 4;
                inputMin = min;
                if (max > 10)
                {
                    max = 10;
                }
            }

            if (prefix.Equals(""))
            {
                prefix = "no prefix.";
            }
            else
            {
                prefix = "the prefix \"" + prefix + "\".";
                largerBox = true;
            }
            if (min != max)
            {
                generationInfo = "All seeds with " + min + " to " + max + " characters generated, using " + prefix;
            }
            else
            {
                generationInfo = "All seeds with " + max + " characters generated, using " + prefix;
            }
            if (!largerBox)
            {
                generateInfo.Text = generationInfo;
                generateInfo.Visibility = System.Windows.Visibility.Visible;
                generationInfo2.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                generationInfo2.Text = generationInfo;
                generationInfo2.Visibility = System.Windows.Visibility.Visible;
                generateInfo.Visibility = System.Windows.Visibility.Hidden;
            }

            string[] replay = InitializeString(path).Split('\\');
            long codedSeed = long.Parse(replay[10].Substring(1));
            if (codedSeed != -2147483642)
            {
                SeedBoxSet("");
                spacesInfo.Visibility = System.Windows.Visibility.Hidden;
                if (codedSeed > 0)
                {
                    double seedCheck = 0.1;
                    long codedSeedLong = (long)codedSeed;
                    while (seedCheck != (long)seedCheck)
                    {
                        seedCheck = codedSeedLong - 6;
                        seedCheck /= 23987;
                        codedSeedLong += 4294967296;
                    }
                    codedSeed = (long)seedCheck;
                }
                if (codedSeed >= 0 && codedSeed <= 2147483647)
                {
                    SeedBoxSet(codedSeed.ToString());
                    if (!largerBox)
                    {
                        generationInfo = "Numeric seed generated.\n\n" + generationInfo;
                        generateInfo.Text = generationInfo;
                        generateInfo.Visibility = System.Windows.Visibility.Visible;
                        generationInfo2.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        generationInfo = "Numeric seed generated.\n\n" + generationInfo;
                        generationInfo2.Text = generationInfo;
                        generationInfo2.Visibility = System.Windows.Visibility.Visible;
                        generateInfo.Visibility = System.Windows.Visibility.Hidden;
                    }

                    if (codedSeed >= 32 && codedSeed <= 4950)
                    {
                        if (prefixValue == codedSeed)
                        {
                            SeedBoxAdd(prefixContent);
                        }
                        for (int i = inputMin; i <= max; i++)
                        {
                            SeedGenerate(prefixContent, inputMin, i, prefixValue, codedSeed, prefixCharExists);
                        }
                    }
                }
                else
                {
                    SeedBoxSet("This seed contains at least 1 space and no letters.");
                    if (codedSeed < 0)
                    {
                        generationInfo += "\n\n" + "Replay value: \n" + codedSeed;
                    }
                    generateInfo.Text = generationInfo;
                    generateInfo.Visibility = System.Windows.Visibility.Visible;
                    generationInfo2.Visibility = System.Windows.Visibility.Hidden;
                }
                SeedBoxUpdate();
            }
            else
            {
                SeedBoxSet("9000000001\n\nThis is basically the overflow seed. Most seeds which consist of 10 digits will generate this dungeon.\n\nThe above seed is just one example.");
                generateInfo.Text = "Seed generated successfully.";
                generateInfo.Visibility = System.Windows.Visibility.Visible;
                generationInfo2.Visibility = System.Windows.Visibility.Hidden;
                SeedBoxUpdate();
            }
        }

        private void SeedGenerate(string baseString, int chars, int targetChars, long total, long targetSeed, bool charExists)
        {
            if (chars < targetChars)
            {
                chars++;
                if (chars == targetChars)
                {
                    for (int i = 32; i <= 90; i++)
                    {
                        if (i == 33)
                        {
                            i = 48;
                        }
                        else if (i == 58)
                        {
                            i = 65;
                            charExists = true;
                        }

                        long mult = i * chars;

                        long check = total + mult;
                        
                        if (check == targetSeed && charExists)
                        {
                            if (i != 32)
                            {
                                SeedBoxAdd(baseString + (char)i);
                            }
                            else
                            {
                                SeedBoxAdd(baseString + "_");
                                spacesInfo.Visibility = System.Windows.Visibility.Visible;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 32; i <= 90; i++)
                    {
                        if (i == 33)
                        {
                            i = 48;
                        }
                        else if (i == 58)
                        {
                            i = 65;
                            charExists = true;
                        }

                        if (i != 32)
                        {
                            SeedGenerate(baseString + (char)i, chars, targetChars, total + (i * chars), targetSeed, charExists);
                        }
                        else
                        {
                            SeedGenerate(baseString + "_", chars, targetChars, total + (i * chars), targetSeed, charExists);
                        } 
                    }
                }
            }
        }

        private string InitializeString(string path)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            return file.ReadToEnd();
        }

        private void prefixBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (prefixBox.Text.Length == 0 )
            {
                prefixContent = "";
                prefixValue = 0;
                prefixCharExists = false;
            }
            else
            {
                int charSum = 0;
                bool charExists = false;
                bool valid = true;
                for (int i = 0; i < prefixBox.Text.Length && valid; i++)
                {
                    int currChar;
                    if (prefixBox.Text[i].Equals('_'))
                    {
                        currChar = 32;
                    }
                    else
                    {
                        currChar = (int)prefixBox.Text[i];
                    }
                    if (currChar == 32 || (currChar >= 48 && currChar <= 57) || (currChar >= 65 && currChar <= 90))
                    {
                        charSum += (currChar * (i + 1));
                        if (currChar >= 65 && currChar <= 90)
                        {
                            charExists = true;
                        }
                    }
                    else
                    {
                        valid = false;
                    }
                }

                if (!valid)
                {
                    prefixBox.Text = prefixContent;
                }
                else
                {
                    prefixContent = prefixBox.Text;
                    prefixValue = charSum;
                    prefixCharExists = charExists;
                }
            }
        }
    }
}