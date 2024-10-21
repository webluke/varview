using System.Windows;
using System.Windows.Controls;

namespace VARView.Controls
{
    /// <summary>
    /// Interaction logic for VARTextBox.xaml
    /// </summary>
    public partial class VARTextBox : UserControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(VARTextBox), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(VARTextBox), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark", typeof(string), typeof(VARTextBox), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(int), typeof(VARTextBox), new PropertyMetadata(default(int)));

        public VARTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
        public int Height
        {
            get { return (int)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        private void VTBCopy_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text)) return;
            Clipboard.SetText(Text);
        }

        public void Clear()
        {
            Text = string.Empty;
        }
    }
}
