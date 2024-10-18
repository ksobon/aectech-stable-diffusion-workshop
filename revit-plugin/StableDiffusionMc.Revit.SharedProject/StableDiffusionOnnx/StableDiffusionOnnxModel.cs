using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace StableDiffusionMc.Revit.StableDiffusionOnnx
{
    public static class StableDiffusionOnnxModel
    {
        public static async Task<string> InferWithOnnxStack(
            string imagePath,
            string prompt,
            double guidanceScale = 7.5,
            float strength = 0.85f
            )
        {
            throw new NotImplementedException();
        }

        public static Image<Rgba32> CropAndResizeImage(Image<Rgba32> image, int width = 1024, int height = 1024)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            Rectangle cropRectangle;

            if (originalWidth > originalHeight)
            {
                int left = (originalWidth - originalHeight) / 2;
                cropRectangle = new Rectangle(left, 0, originalHeight, originalHeight);
            }
            else
            {
                int top = (originalHeight - originalWidth) / 2;
                cropRectangle = new Rectangle(0, top, originalWidth, originalWidth);
            }

            image.Mutate(x => x.Crop(cropRectangle));
            image.Mutate(x => x.Resize(width, height));

            return image;
        }
    }
}
