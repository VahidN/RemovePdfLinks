using System;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;

namespace RemovePdfLinks.Core
{
    public class ReplacePdfLinks
    {
        PdfReader _reader;

        public string InputPdf { set; get; }
        public string OutputPdf { set; get; }
        public Func<string, string> UriToNewUrl { set; get; }

        public void Start()
        {
            updatePdfLinks();
            saveChanges();
        }

        private static bool hasAction(PdfDictionary annotationDictionary)
        {
            var subtype = annotationDictionary.Get(PdfName.SUBTYPE);
            return subtype != null && subtype.Equals(PdfName.LINK);
        }

        private static bool isUriAction(PdfDictionary annotationAction)
        {
            var uriObj = annotationAction.Get(PdfName.S);
            return uriObj != null && uriObj.Equals(PdfName.URI);
        }

        private static bool isUriLaunch(PdfDictionary annotationAction)
        {
            var uriObj = annotationAction.Get(PdfName.S);
            return uriObj != null && uriObj.Equals(PdfName.LAUNCH);
        }

        private PdfArray getAnnotationsOfCurrentPage(int pageNumber)
        {
            var pageDictionary = _reader.GetPageN(pageNumber);
            return pageDictionary != null ? pageDictionary.GetAsArray(PdfName.ANNOTS) : null;
        }

        private void replaceUriLaunch(PdfDictionary annotationAction)
        {
            var filespec = annotationAction.GetAsDict(PdfName.F);
            if (filespec == null)
                return;

            var uri = filespec.GetAsString(PdfName.F);
            if (uri == null)
                return;

            if (string.IsNullOrWhiteSpace(uri.ToString()))
                return;

            var newUrl = UriToNewUrl(uri.ToString());
            if (!string.IsNullOrWhiteSpace(newUrl))
            {
                annotationAction.Put(PdfName.F, new PdfString(newUrl));
            }
            else
            {
                annotationAction.Remove(PdfName.F);
                annotationAction.Remove(PdfName.S);
                annotationAction.Remove(PdfName.URI);
            }
        }

        private void replaceUris(PdfDictionary annotationAction)
        {
            var uri = annotationAction.Get(PdfName.URI) as PdfString;
            if (uri == null)
                return;

            if (string.IsNullOrWhiteSpace(uri.ToString()))
                return;


            var newUrl = UriToNewUrl(uri.ToString());
            if (!string.IsNullOrWhiteSpace(newUrl))
            {
                annotationAction.Put(PdfName.URI, new PdfString(newUrl));
            }
            else
            {
                annotationAction.Remove(PdfName.S);
                annotationAction.Remove(PdfName.URI);
            }
        }

        private void saveChanges()
        {
            using (var fileStream = new FileStream(OutputPdf, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var stamper = new PdfStamper(_reader, fileStream))
            {
                stamper.Close();
            }
        }

        private void updatePdfLinks()
        {
            _reader = new PdfReader(InputPdf);
            var pageCount = _reader.NumberOfPages;
            for (var i = 1; i <= pageCount; i++)
            {
                var annotations = getAnnotationsOfCurrentPage(i);
                if (annotations == null || !annotations.Any())
                    continue;

                foreach (var annotation in annotations.ArrayList)
                {
                    var annotationDictionary = PdfReader.GetPdfObject(annotation) as PdfDictionary;
                    if (annotationDictionary == null)
                    {
                        continue;
                    }

                    if (!hasAction(annotationDictionary))
                        continue;

                    var annotationActionObj = annotationDictionary.Get(PdfName.A);
                    if (annotationActionObj == null)
                    {
                        continue;
                    }

                    var annotationAction = (annotationActionObj.IsIndirect() ? PdfReader.GetPdfObject(annotationActionObj) : annotationActionObj) as PdfDictionary;
                    if (annotationAction == null)
                    {
                        continue;
                    }

                    if (isUriAction(annotationAction))
                    {
                        replaceUris(annotationAction);
                    }

                    if (isUriLaunch(annotationAction))
                    {
                        replaceUriLaunch(annotationAction);
                    }
                }
            }
        }
    }
}