from diffusers import DiffusionPipeline
from PIL import Image
import torch


pipe = DiffusionPipeline.from_pretrained("stabilityai/stable-diffusion-xl-base-1.0", torch_dtype=torch.float16, use_safetensors=True, variant="fp16")
pipe.to("cuda")

def generate_image_from_text(
        prompt: str = "An astronaut riding a green horse"
) -> Image:
    image = pipe(
            prompt=prompt,
            num_inference_steps=15
            ).images[0]
    return image
    