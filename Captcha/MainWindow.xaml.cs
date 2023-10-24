using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

namespace Captcha
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _captchaCode;
        public MainWindow()
        {
            InitializeComponent();
        }

        void LoadContent()
        {
            int width = 130;
            int height = 36;

            var captchaCode = CaptchaLogik.GenerateCaptchaCode();
            _captchaCode = captchaCode;

            var result = CaptchaLogik.GenerateCaptchaImage(width, height, captchaCode);

            Stream s = new MemoryStream(result.CaptchaByteData);

            luc.Source = BitmapFrame.Create(s

                                            );
        }

        private void button_click(object sender, RoutedEventArgs e)
        {
            LoadContent();
        }

        private void btn1_click(object sender, RoutedEventArgs e)
        {
            if(cnText.Text == _captchaCode)
            {
                MessageBox.Show("Accept Finish");
                
            }
            else MessageBox.Show("Accept Fail");
        }
    }
}
