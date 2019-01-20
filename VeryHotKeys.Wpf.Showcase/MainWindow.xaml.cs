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

namespace VeryHotKeys.Wpf.Showcase
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var register = new HotKeyRegisterer(this, Surprise, HotKeyMods.Control | HotKeyMods.Alt, ConsoleKey.B);
        }

        private void Surprise(object sender, EventArgs e)
        {
            MessageBox.Show("Hello hello from your keyboard...? At least it works :p", "Nice", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
