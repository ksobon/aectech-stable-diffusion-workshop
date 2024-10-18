# import torch
# from diffusers import StableDiffusionXLImg2ImgPipeline
import onnx

print("Checking the model...")

model_path= r"C:\Users\patry\source\repos\aectech-stable-diffusion\onnx-utilities\onnx-stable-diffusion-v1-5\unet\model.onnx"

# loading the model is not necessary for checking and computationally expensive
# onnx_model = onnx.load(model_path)

onnx.checker.check_model(model_path)
print(f"ONNX model exported and checked successfully! Model path: {model_path}")

