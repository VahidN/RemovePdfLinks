using System.IO;
using RemovePdfLinks.Core;
using RemovePdfLinks.Models;
using RemovePdfLinks.Utils;

namespace RemovePdfLinks.ViewModels
{
    public class MainWindowViewModel
    {
        public GuiModel GuiModelData { set; get; }
        public DelegateCommand<string> DoStart { set; get; }

        public MainWindowViewModel()
        {
            GuiModelData = new GuiModel();
            DoStart = new DelegateCommand<string>(data => processFiles(), s => true);
        }

        private void processFiles()
        {
            if (string.IsNullOrWhiteSpace(GuiModelData.InputFolderPath) ||
                string.IsNullOrWhiteSpace(GuiModelData.OutputFolderPath))
            {
                return;
            }

            if (!Directory.Exists(GuiModelData.OutputFolderPath))
            {
                Directory.CreateDirectory(GuiModelData.OutputFolderPath);
            }

            var pdfFiles = Directory.GetFiles(GuiModelData.InputFolderPath, "*.pdf");
            foreach (var file in pdfFiles)
            {
                var newFileName = Path.GetFileName(file);
                new ReplacePdfLinks
                {
                    InputPdf = file,
                    OutputPdf = Path.Combine(GuiModelData.OutputFolderPath, newFileName),
                    UriToNewUrl = uri =>
                    {
                        return GuiModelData.RemoveAllLinks ? string.Empty : GuiModelData.NewUrl;
                    }
                }.Start();
            }
        }
    }
}