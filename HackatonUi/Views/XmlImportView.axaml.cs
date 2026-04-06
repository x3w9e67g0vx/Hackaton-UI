using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using HackatonUi.Services;

namespace HackatonUi.Views
{
    public partial class XmlImportView : UserControl
    {
        private readonly XmlImportService _xmlService;

        public XmlImportView()
        {
            InitializeComponent();
            _xmlService = new XmlImportService("Data Source=hackaton.db;Version=3;");
        }

        private async void SelectXmlFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите XML файл",
                Filters = { new FileDialogFilter { Name = "XML Files", Extensions = { "xml" } } }
            };
            var window = this.VisualRoot as Window;
            if (window != null)
            {
                var result = await dialog.ShowAsync(window);
                if (result != null && result.Length > 0)
                {
                    FilePathBox.Text = result[0];
                }
            }
        }

        private void ImportXml_Click(object sender, RoutedEventArgs e)
        {
            _xmlService.ImportBuildingsFromXml(FilePathBox.Text);
            StatusText.Text = "Импорт завершён!";
        }
    }
}