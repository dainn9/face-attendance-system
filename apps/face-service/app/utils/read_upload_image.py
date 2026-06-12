import cv2
from fastapi import UploadFile
import numpy as np

from app.core.exceptions import AppException


async def read_upload_image(file: UploadFile):
    contents = await file.read()
    np_arr = np.frombuffer(contents, np.uint8)
    img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)

    if img is None:
        raise AppException(422, "INVALID_IMAGE", "Invalid image file")

    return img  