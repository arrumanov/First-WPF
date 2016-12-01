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
using System.Windows.Forms;
using System.Windows.Markup;
using System.IO;
using System.Windows.Media.Animation;

namespace WpfAppFlowDocument
{
    class MyFlowDocument
    {
        public string Info { get; set; }
        public bool Error { get; set; }
        public string Name { get; set; }
        public MyFlowDocument()
        {
            Error = false;
        }

        public void Load(FileInfo fInfo)
        {
            Info = fInfo.FullName;
            FlowDocument Doc = new FlowDocument();
            //определяем ту часть документа, которую надо изменить (весь документ)
            TextRange textRange = new TextRange(Doc.ContentStart, Doc.ContentEnd);
            
                //Doc = XamlReader.Load(new FileStream(fStream.FullName, FileMode.Open)) as FlowDocument;
                using(FileStream fs = new FileStream(fInfo.FullName, FileMode.Open))
                {
                    Name = fInfo.Name;
                    try
                    {
                        textRange.Load(fs, System.Windows.DataFormats.Xaml);
                    }
                    catch (System.Windows.Markup.XamlParseException ex)
                    {
                        Info = ex.Message;
                        Error = true;
                    }
                }
            
        }

        public override string ToString()
        {
            return Name;
        }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<MyFlowDocument> myListDocument { get; set; }

        public MainWindow()
        {
            myListDocument = new List<MyFlowDocument>();
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                //определение индекса выбранного элемента и вывод на экран через image1
                int index = listBox.SelectedIndex;
                MyFlowDocument fDoc = new MyFlowDocument();
                fDoc = listBox.Items[index] as MyFlowDocument;
                if(!fDoc.Error)
                {
                    TextRange textRange = new TextRange(docBox.ContentStart, docBox.ContentEnd);
                    //Doc = XamlReader.Load(new FileStream(fStream.FullName, FileMode.Open)) as FlowDocument;
                    using(FileStream fs = new FileStream(fDoc.Info, FileMode.Open))
                    {
                        textRange.Load(fs, System.Windows.DataFormats.Xaml);
                    }
                }
                else
                {
                    docBox.Blocks.Clear();
                    Paragraph paragraph = new Paragraph();
                    paragraph.Background = Brushes.Red;
                    paragraph.Inlines.Add(fDoc.Info);
                    docBox.Blocks.Add(paragraph);

                    DoubleAnimation buttonAnimation = new DoubleAnimation();
                    buttonAnimation.From = button.ActualHeight;
                    buttonAnimation.To = button.ActualHeight - 40;
                    buttonAnimation.AutoReverse = true;
                    buttonAnimation.RepeatBehavior = new RepeatBehavior(5);
                    buttonAnimation.FillBehavior = FillBehavior.Stop;
                    buttonAnimation.Duration = TimeSpan.FromSeconds(0.05);
                    button.BeginAnimation(System.Windows.Controls.Button.HeightProperty, buttonAnimation);
                }
                    
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            // через диалог открытия папки
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo dir = new DirectoryInfo(dlg.SelectedPath);
                
                // получаю путь и перебираю все файлы в полученной папке
                foreach (FileInfo fInfo in dir.GetFiles())
                {
                    if (fInfo.Extension == ".xaml") // если xaml
                    {
                        MyFlowDocument fDoc = new MyFlowDocument();
                        fDoc.Load(fInfo);
                        myListDocument.Add(fDoc);
                        this.listBox.Items.Add(fDoc);
                    }
                }
            }
        }
    }
}
