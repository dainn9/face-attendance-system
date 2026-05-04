import base64
import cv2
import numpy as np

class ImageDecodeError(Exception):
    pass

def base64_to_image(base64_str: str) -> np.ndarray:
    try:
        # Strip prefix nếu FE gửi dạng data URL
        # "data:image/jpeg;base64,/9j/4AAQ..." → "/9j/4AAQ..."
        if ',' in base64_str:
            base64_str = base64_str.split(',', 1)[1]

        img_data = base64.b64decode(base64_str)
        np_arr = np.frombuffer(img_data, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)

        if img is None:
            raise ImageDecodeError("cv2 could not decode image")

        return img

    except ImageDecodeError:
        raise
    except Exception as e:
        raise ImageDecodeError(f"Invalid base64 image: {str(e)}")
