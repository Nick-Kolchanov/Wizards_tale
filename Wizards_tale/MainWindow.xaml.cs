using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsMusicOn;
        private readonly MediaPlayer mpBgr;
        public MainWindow()
        {
            InitializeComponent();
            mpBgr = new MediaPlayer();
            mpBgr.MediaEnded += MpBgr_MediaEnded; ;
            mpBgr.Open(new Uri("assets/music/backgroundMelody.mp3", UriKind.Relative));
            IsMusicOn = false;
        }

        private void MpBgr_MediaEnded(object sender, EventArgs e)
        {
            mpBgr.Position = TimeSpan.FromMilliseconds(1);
        }

        private void MusicButton_Click(object sender, RoutedEventArgs e)
        {
            IsMusicOn = !IsMusicOn;
            Button button = (Button)sender;

            if (IsMusicOn)
                mpBgr.Play();
            else
                mpBgr.Stop();


            if (IsMusicOn)
                button.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/guiIcons/sound-on.png")));
            else
                button.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/guiIcons/sound-off.png")));
        }

        private void Button_Demo_Click(object sender, RoutedEventArgs e)
        {
            demo_label.Visibility = Visibility.Visible;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            demo_label.Visibility = Visibility.Hidden;
        }

        private void Button_Click_Play(object sender, RoutedEventArgs e)
        {
            MapWindow mapWindow = new MapWindow();
            mapWindow.Top = Top;
            mapWindow.Left = Left;
            mapWindow.Show();
            Close();
        }

        
    }
}
