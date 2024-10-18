import io
from fastapi import FastAPI
from fastapi.responses import StreamingResponse
from ml import generate_image_from_text


app = FastAPI()

@app.get("/")
async def root():
    return {"message": "Hello World"}



@app.get("/text-to-image")
async def text_to_image(
    prompt: str = "An astronaut riding a green horse"
):
    # call a function that will generate an image from text
    images = generate_image_from_text(prompt)
    memory_stream = io.BytesIO()
    images.save(memory_stream, format="PNG")
    memory_stream.seek(0)

    return StreamingResponse(memory_stream, media_type="image/png")








