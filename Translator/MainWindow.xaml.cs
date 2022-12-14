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
using TranslateClass;

namespace Translator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TranslateViewModelClass translator;
        public MainWindow()
        {
            translator = new TranslateViewModelClass();
            InitializeComponent();
        }

        public void ClearText(object sender, RoutedEventArgs e)
        {
            TextToTranslate.Text = String.Empty;
        }
        
        public void TranslateText(object sender, RoutedEventArgs e)
        {
            TranslatedText.Text = translator.TranslateText(TextToTranslate.Text);
        }
    }
}