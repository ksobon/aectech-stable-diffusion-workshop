using CommunityToolkit.Mvvm.Input;
using StableDiffusionMc.Revit.Core.Utilities.WPF;
using System.Diagnostics;
using System.Windows;
using static StableDiffusionMc.Revit.StableDiffusionOnnx.StableDiffusionOnnxModel;

namespace StableDiffusionMc.Revit.StableDiffusionApi
{
    public class StableDiffusionApiViewModel : ExtendedViewModelBase
    {
        public StableDiffusionApiModel Model { get; set; }

        public StableDiffusionApiView Win { get; set; }

        public RelayCommand Generate { get; set; }

        public RelayCommand Capture { get; set; }

        public RelayCommand GenerateOnnxImg2Img { get; set; }

        public string WorkingDirectory { get; set; }

        private string _testImagePath;
        public string TestImagePath
        {
            get { return _testImagePath; }
            set { _testImagePath = value; OnPropertyChanged(); }
        }

        private string _generatedImagePath;

        public string GeneratedImagePath
        {
            get { return _generatedImagePath; }
            set { _generatedImagePath = value; OnPropertyChanged(); }
        }

        private string _prompt = "Enter a prompt...";
        public string Prompt
        {
            get { return _prompt; }
            set { _prompt = value; OnPropertyChanged(); }
        }

        private string _negativePrompt = "low quality, bad quality, sketches";

        public string NegativePrompt
        {
            get { return _negativePrompt; }
            set { _negativePrompt = value; OnPropertyChanged(); }
        }

        private int _numInferenceSteps = 50;
        public int NumInferenceSteps
        {
            get { return _numInferenceSteps; }
            set { _numInferenceSteps = value; OnPropertyChanged(); }
        }

        private double _guidanceScale = 7.5;

        public double GuidanceScale
        {
            get { return _guidanceScale; }
            set { _guidanceScale = value; OnPropertyChanged(); }
        }

        private double _strength = 0.85;

        public double Strength
        {
            get { return _strength; }
            set { _strength = value; OnPropertyChanged(); }
        }

        private double _contronetConditioningScale = 0.5;

        public double ConronetConditioningScale
        {
            get { return _contronetConditioningScale; }
            set { _contronetConditioningScale = value; OnPropertyChanged(); }
        }

        private int _seed = 42;

        public int Seed
        {
            get { return _seed; }
            set { _seed = value; OnPropertyChanged(); }
        }


        public StableDiffusionApiViewModel(StableDiffusionApiModel model)
        {
            Model = model;

            WindowLoaded = new RelayCommand<Window>(OnWindowLoaded);
            Ok = new RelayCommand<Window>(OnOk);
            Cancel = new RelayCommand<Window>(OnCancel);
            Help = new RelayCommand(OnHelp);
            Capture = new RelayCommand((OnCapture));
            Generate = new RelayCommand(OnGenerate);
            GenerateOnnxImg2Img = new RelayCommand(OnGenerateOnnxImg2Img);
        }

        private async void OnCapture()
        {
            var capturedImagePath = await Model.ExportViewAsImagePath();

            GeneratedImagePath = capturedImagePath;
        }

        private async void OnGenerate()
        {
            var capturedImagePath = await Model.ExportViewAsImagePath();
            if (string.IsNullOrEmpty(capturedImagePath))
            {
                MessageBox.Show("Failed to capture image");
                return;
            }

            var generatedImagePath = await Model.SendToServerAsync(
                capturedImagePath,
                Prompt,
                NegativePrompt,
                NumInferenceSteps,
                GuidanceScale,
                ConronetConditioningScale,
                Strength,
                Seed
                );

            if (string.IsNullOrEmpty(generatedImagePath))
            {
                MessageBox.Show("Failed to generate image");
                return;
            }

            GeneratedImagePath = generatedImagePath;
        }

        private async void OnGenerateOnnxImg2Img()
        {
            var capturedImagePath = await Model.ExportViewAsImagePath();
            if (string.IsNullOrEmpty(capturedImagePath))
            {
                MessageBox.Show("Failed to capture image");
                return;
            }

            string generatedImagePath = null;

            Debug.WriteLine("Starting Onnx Inference");
            try
            {
                generatedImagePath = await InferWithOnnxStack(
                capturedImagePath,
                Prompt,
                GuidanceScale,
                (float)Strength
                );

                Debug.WriteLine("Finished Onnx Inference");

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }


            if (string.IsNullOrEmpty(generatedImagePath))
            {
                MessageBox.Show("Failed to generate image");
                return;
            }

            GeneratedImagePath = generatedImagePath;
        }

        public override async void OnWindowLoaded(Window win)
        {
            Win = (StableDiffusionApiView)win;

            var capturedImagePath = await Model.ExportViewAsImagePath();

            GeneratedImagePath = capturedImagePath;
        }

        public override void OnOk(Window win)
        {
            throw new NotImplementedException();
        }

        public override void OnCancel(Window win)
        {
            Win.Close();
        }

        public override void OnHelp()
        {
            throw new NotImplementedException();
        }
    }
}
