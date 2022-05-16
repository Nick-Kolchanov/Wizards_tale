using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для TextControl.xaml
    /// </summary>
    public partial class TextControl : UserControl
    {
        public TextControl() : this("") { }

        public TextControl(string text)
        {
            InitializeComponent();

            textLabel.Text = text;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
