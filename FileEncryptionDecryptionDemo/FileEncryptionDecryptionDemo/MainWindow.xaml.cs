using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace FileEncryptionDecryptionDemo
{
    public partial class MainWindow : Window
    {
        const string FilePath = "output.txt";

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            passwordBox.PreviewKeyDown += Password_PreviewKeyDown;
        }

        private void Password_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (File.Exists(FilePath))
                    input.Text = Encryption.DecryptFile(FilePath, passwordBox.Text);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FilePath))
                input.Text = File.ReadAllText(FilePath);

            passwordBox.Focus();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Encryption.EncryptFile(input.Text, FilePath, passwordBox.Text);
        }
    }
}
