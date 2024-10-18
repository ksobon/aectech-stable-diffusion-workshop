using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;

namespace StableDiffusionMc.Revit.StableDiffusionApi
{
    public class StableDiffusionApiModel
    {
        protected Document Doc { get; set; }
        protected UIDocument UiDoc { get; set; }

        public View View { get; set; }

        public StableDiffusionApiModel(UIApplication uiApp)
        {
            UiDoc = uiApp.ActiveUIDocument;
            Doc = uiApp.ActiveUIDocument.Document;
            View = Doc.ActiveView;
        }

        public async Task<string> ExportViewAsImagePath()
        {
            string sanitizedViewName = SanitizeFileName(View.Name);

            string uniqueFileName = $"{sanitizedViewName}_{DateTime.Now:yyyyMMddHHmmss}.png";

            string imagePath = Path.Combine(Path.GetTempPath(), uniqueFileName);

            var options = new ImageExportOptions
            {
                ExportRange = ExportRange.VisibleRegionOfCurrentView,
                FilePath = imagePath,
                FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_600,
                ZoomType = ZoomFitType.FitToPage,
                PixelSize = 1600,
            };

            Doc.ExportImage(options);

            await Task.Delay(100);

            //check if .png exists if not swap .png for .jpg and try again 
            if (!File.Exists(imagePath))
            {
                imagePath = imagePath.Replace(".png", ".jpg");
            }

            return imagePath;
        }

        public async Task<string> SendToServerAsync(
            string imagePath,
            string prompt,
            string negativePrompt = "low quality, bad quality, sketches",
            int numInferenceSteps = 15,
            double guidanceScale = 5.0,
            double contronetConditioningScale = 0.5,
            double strength = 0.5,
            int seed = 42
            )
        {
            throw new NotImplementedException();
        }

        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }
    }
}
