using System;
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
            if(string.IsNullOrWhiteSpace(GuiModelData.FolderPath))
                return;

            var pdfFiles = Directory.GetFiles(GuiModelData.FolderPath, "*.pdf");
            foreach (var file in pdfFiles)
            {
                if(file.EndsWith("-mod.pdf", StringComparison.OrdinalIgnoreCase)) continue;

                var newFileName = string.Format("{0}-mod.pdf", Path.GetFileNameWithoutExtension(file));
                new ReplacePdfLinks
                {
                    InputPdf = file,
                    OutputPdf = Path.Combine(GuiModelData.FolderPath, newFileName),
                    UriToNewUrl = uri =>
                    {
                        return GuiModelData.RemoveAllLinks ? string.Empty : GuiModelData.NewUrl;
                    }
                }.Start();
            }
        }
    }
}