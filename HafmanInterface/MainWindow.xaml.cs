using System.Text;
using System.Windows;
using HafmanInterface.Algorithm;
using Microsoft.Win32;

namespace HafmanInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoCompression(object sender, RoutedEventArgs e)
        {
            OpenFileDialog win = new OpenFileDialog();

            if (win.ShowDialog() == true)
            {
                var filepath = win.FileName;
                EnCoder enCoder = new EnCoder();
                enCoder.LoadSource(filepath);
                enCoder.DoCoder();
            }
        }

        private void DoDecompression(object sender, RoutedEventArgs e)
        {
            OpenFileDialog win = new OpenFileDialog();
            if (win.ShowDialog() == true)
            {
                var filepath = win.FileName;
                DeCoder deCoder = new DeCoder();
                deCoder.ReadFile(filepath);
                deCoder.PrintRezult();
            }
        }
    }
}