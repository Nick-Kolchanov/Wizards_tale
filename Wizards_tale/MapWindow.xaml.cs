using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wizards_tale
{
    /// <summary>
    /// Логика взаимодействия для MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        int maxLevelInd;
        int curLevel;
        string fileName = "LevelSave.txt";

        public MapWindow()
        {
            InitializeComponent();
            LoadNewMaxLevel();
            LoadLevels();

        }

        public MapWindow(int newMaxLevelInd)
        {
            InitializeComponent();
            maxLevelInd = newMaxLevelInd;
            SaveNewMaxLevel();
            LoadLevels();

            
        }

        private void SaveNewMaxLevel()
        {
            using (StreamWriter sr = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), false))
            {
                sr.Write(maxLevelInd);
            }
        }

        private void LoadNewMaxLevel()
        {
            using (StreamReader sr = new StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)))
            {
                if (int.TryParse(sr.ReadLine(), out int _levelInd))
                {
                    maxLevelInd = _levelInd;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
        }

        private void LoadLevels()
        {
            foreach (var item in levelsGrid.Children)
            {
                if (item is Button button)
                {
                    int.TryParse(button.Content?.ToString(), out int levelInd);
                    if (levelInd > maxLevelInd)
                    {
                        button.IsEnabled = false;
                    }
                }
            }
        }
        
        private void LevelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse((sender as Button)?.Content.ToString(), out int levelInd))
            {
                return;
            }

            if (levelInd > 2)  //temp for demo
                playButton.IsEnabled = false;
            else
                playButton.IsEnabled = true;

            curLevel = levelInd;
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (curLevel == 2)
            {
                demoLabel.Visibility = Visibility.Visible;
                return;
            }

            LevelWindow levelWindow = new LevelWindow(curLevel);
            levelWindow.Top = Top;
            levelWindow.Left = Left;
            levelWindow.Show();
            Close();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MapWindow mapWindow = new MapWindow(1);
            mapWindow.Top = Top;
            mapWindow.Left = Left;
            mapWindow.Show();
            Close();
        }
    }
}
