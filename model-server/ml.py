import torch
from diffusers import (
    ControlNetModel,
    StableDiffusionXLPipeline,
    StableDiffusionXLImg2ImgPipeline,
    StableDiffusionXLControlNetPipeline,
    AutoencoderKL)
from diffusers.utils import load_image
from PIL import Image
import numpy as np

# model_id_text = "stabilityai/stable-diffusion-xl-base-1.0"
# model_id = "stabilityai/stable-diffusion-xl-refiner-1.0"

# pipe_text_to_image = StableDiffusionXLPipeline.from_pretrained(
#     model_id_text,
#     torch_dtype=torch.float16,
#     variant="fp16",
#     use_safetensors=True
# )
# pipe_text_to_image = pipe_text_to_image.to("cuda")

# def text_to_image(prompt: str) -> Image:
#     image: Image = pipe_text_to_image(prompt=prompt).images[0]
#     return image

control_net_model_id = "stabilityai/stable-diffusion-xl-base-1.0"
controlnet = ControlNetModel.from_pretrained(
    "diffusers/controlnet-canny-sdxl-1.0",
    torch_dtype=torch.float16
).to("cuda")
vae = AutoencoderKL.from_pretrained("madebyollin/sdxl-vae-fp16-fix", torch_dtype=torch.float16).to("cuda")
pipe_controlnet = StableDiffusionXLControlNetPipeline.from_pretrained(
    control_net_model_id,
    controlnet=controlnet,
    vae=vae,
    torch_dtype=torch.float16,
)
pipe_controlnet = pipe_controlnet.to("cuda")

def obtain_control_net_image(
        prompt: str,
        negative_prompt: str,
        init_image: Image,
        num_inference_steps: int = 50,
        guidance_scale: float = 5.0,
        controlnet_conditioning_scale: float = 0.5,
        strength: float = 0.85,
        seed: int = None

) -> Image:
    generator = None if seed is None else torch.Generator("cuda").manual_seed(seed)
    print(f"Using device: {pipe_controlnet.device}")
    print(f"ControlNet conditioning scale: {controlnet_conditioning_scale}")

    image: Image = pipe_controlnet(
        prompt,
        negative_prompt=negative_prompt,
        image=init_image,
        num_inference_steps=num_inference_steps,
        guidance_scale=guidance_scale,
        strength=strength,
        controlnet_conditioning_scale=controlnet_conditioning_scale,
        generator=generator
    ).images[0]
    return image