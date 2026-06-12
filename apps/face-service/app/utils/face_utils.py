import cv2
import numpy as np
from numpy.linalg import norm

from app.core.exceptions import AppException

def cosine_similarity(a, b):
    return float(np.dot(a, b) / (norm(a) * norm(b)))

def estimate_pose_ratio(kps):
    left_eye, right_eye, nose, _, _ = kps
    
    dist_left = abs(nose[0] - left_eye[0])
    dist_right = abs(right_eye[0] - nose[0])

    return dist_left / max(dist_right, 1e-6)
    
def check_image_quality(img, face, pose : str | None = None, blur_threshold=50):
    label = f" for pose {pose}" if pose else ""

    h, w = img.shape[:2]

    if w < 200 or h < 200:
        raise AppException(
            422,
            "IMAGE_TOO_SMALL",
            f"Image{label} is too small"
        )

    H, W = img.shape[:2]
    x1 = max(0, int(face.bbox[0]))
    y1 = max(0, int(face.bbox[1]))
    x2 = min(W, int(face.bbox[2]))
    y2 = min(H, int(face.bbox[3]))
    
    face_w = x2 - x1
    face_h = y2 - y1

    if face_w < 80 or face_h < 80:
        raise AppException(
            422,
            "FACE_TOO_SMALL",
            f"Face{label} is too small"
        )
    
    face_crop = img[y1:y2, x1:x2]

    gray = cv2.cvtColor(face_crop, cv2.COLOR_BGR2GRAY)

    brightness = gray.mean()
    if brightness < 50:
        raise AppException(
            422,
            "IMAGE_TOO_DARK",
            f"Image{label} is too dark"
        )

    if brightness > 220:
        raise AppException(
            422,
            "IMAGE_TOO_BRIGHT",
            f"Image{label} is too bright"
        )

    blur_score = cv2.Laplacian(gray, cv2.CV_64F).var()
    if blur_score < blur_threshold:   
        raise AppException(
            422,
            "IMAGE_TOO_BLURRY",
            f"Image{label} is too blurry"
        )
    

CENTER_MIN = 0.85
CENTER_MAX = 1 / CENTER_MIN

def check_pose(face, expected_pose: str):
    ratio = estimate_pose_ratio(face.kps)

    if ratio < 0.05 or ratio > 20:
        raise AppException(
            422,
            "INVALID_POSE",
            f"Unable to estimate face pose for {expected_pose} image"
        )

    if expected_pose == "center":
        if not (CENTER_MIN <= ratio <= CENTER_MAX):
            raise AppException(
                422,
                "INVALID_POSE",
                f"Center image for {expected_pose} must be front-facing"
            )

    elif expected_pose == "left":
        if not ratio < CENTER_MIN:
            raise AppException(
                422,
                "INVALID_POSE",
                f"Left image for {expected_pose} must be slightly turned left"
            )

    elif expected_pose == "right":
        if not CENTER_MAX < ratio:
            raise AppException(
                422,
                "INVALID_POSE",
                f"Right image for {expected_pose} must be slightly turned right"
            )

    else:
        raise AppException(
            422,
            "INVALID_POSE",
            f"Unsupported pose: {expected_pose}"
        )