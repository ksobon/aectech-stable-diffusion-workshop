# import torch
# from diffusers import ControlNetModel

# # Load the model locally
# controlnet = ControlNetModel.from_pretrained(
#     "diffusers/controlnet-canny-sdxl-1.0",
#     torch_dtype=torch.float16
# )

# # Save it to your desired format
# controlnet.save_pretrained("controlnet_canny_sdxl_v1_0_optimized/")


from optimum.onnxruntime import ORTStableDiffusionPipeline

model_id = "runwayml/stable-diffusion-v1-5"
pipeline = ORTStableDiffusionPipeline.from_pretrained(model_id, export=True)
prompt = "sailing ship in storm by Leonardo da Vinci"
image = pipeline(prompt).images[0]
pipeline.save_pretrained("./onnx-stable-diffusion-v1-5")