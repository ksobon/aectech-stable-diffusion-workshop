using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;

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
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"The file at {imagePath} was not found.");
            }

            using (var client = new HttpClient())
            {
                var query = $"?prompt={prompt}&negative_prompt={negativePrompt}&num_inference_steps={numInferenceSteps}&guidance_scale={guidanceScale}&controlnet_conditioning_scale={contronetConditioningScale}&strength={strength}&seed={seed}";


                client.BaseAddress = new Uri("http://127.0.0.1:8000");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                using (var content = new MultipartFormDataContent())
                {
                    var streamContent = new StreamContent(imageStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    content.Add(streamContent, "file", Path.GetFileName(imagePath));

                    HttpResponseMessage response = await client.PostAsync($"/generate-controlnet-invert{query}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        {
                            string generatedImageFileName = $"generated_{DateTime.Now:yyyyMMddHHmmss}.png";
                            string generatedImagePath = Path.Combine(Path.GetTempPath(), generatedImageFileName);

                            using (var fileStream = new FileStream(generatedImagePath, FileMode.Create, FileAccess.Write))
                            {
                                await responseStream.CopyToAsync(fileStream);
                            }

                            return generatedImagePath;
                        }
                    }
                    else
                    {
                        throw new Exception($"Error occurred while sending the image: {response.ReasonPhrase}");
                    }
                }
            }
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
