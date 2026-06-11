import cv2
import numpy as np
from numpy.linalg import norm

from app.core.exceptions import AppException


POSES = ["left", "center", "right"]


def cosine_similarity(a, b):
    return float(np.dot(a, b) / (norm(a) * norm(b)))

def estimate_pose_ratio(kps):
    left_eye, right_eye, nose, _, _ = kps
    
    dist_left = abs(nose[0] - left_eye[0])
    dist_right = abs(right_eye[0] - nose[0])

    return dist_left / max(dist_right, 1e-6)

def check_image_quality(img, face, pose):
    h, w = img.shape[:2]

    if w < 200 or h < 200:
        raise AppException(
            422,
            "IMAGE_TOO_SMALL",
            f"Image for pose {pose} is too small"
        )

    x1, y1, x2, y2 = face.bbox.astype(int)
    face_w = x2 - x1
    face_h = y2 - y1

    if face_w < 80 or face_h < 80:
        raise AppException(
            422,
            "FACE_TOO_SMALL",
            f"Face in pose {pose} is too small"
        )

    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    brightness = gray.mean()
    if brightness < 50:
        raise AppException(
            422,
            "IMAGE_TOO_DARK",
            f"Image for pose {pose} is too dark"
        )

    if brightness > 220:
        raise AppException(
            422,
            "IMAGE_TOO_BRIGHT",
            f"Image for pose {pose} is too bright"
        )

    blur_score = cv2.Laplacian(gray, cv2.CV_64F).var()
    if blur_score < 80:
        raise AppException(
            422,
            "IMAGE_TOO_BLURRY",
            f"Image for pose {pose} is too blurry"
        )

CENTER_MIN = 0.85
CENTER_MAX = 1 / CENTER_MIN

def check_pose(face, expected_pose: str):
    ratio = estimate_pose_ratio(face.kps)

    if ratio < 0.05 or ratio > 20:
        raise AppException(
            422,
            "INVALID_POSE",
            "Unable to estimate face pose"
        )

    if expected_pose == "center":
        if not (CENTER_MIN <= ratio <= CENTER_MAX):
            raise AppException(
                422,
                "INVALID_POSE",
                "Center image must be front-facing"
            )

    elif expected_pose == "left":
        if not ratio < CENTER_MIN:
            raise AppException(
                422,
                "INVALID_POSE",
                "Left image must be slightly turned left"
            )

    elif expected_pose == "right":
        if not CENTER_MAX < ratio:
            raise AppException(
                422,
                "INVALID_POSE",
                "Right image must be slightly turned right"
            )

    else:
        raise AppException(
            422,
            "INVALID_POSE",
            f"Unsupported pose: {expected_pose}"
        )
    
def validate_face_register_images(face_app, images_by_pose: dict):
    """
    images_by_pose:
    {
        "left": img_left,
        "center": img_center,
        "right": img_right
    }
    """

    for pose in POSES:
        if pose not in images_by_pose:
            raise AppException(
                422,
                "MISSING_IMAGE",
                f"Missing image for pose {pose}"
            )

    result = {}

    for pose in POSES:
        img = images_by_pose[pose]

        faces = face_app.get(img)

        if len(faces) == 0:
            raise AppException(
                422,
                "NO_FACE_DETECTED",
                f"No face detected in image for pose {pose}"
            )

        if len(faces) > 1:
            raise AppException(
                422,
                "MULTIPLE_FACES_DETECTED",
                f"Multiple faces detected in image for pose {pose}"
            )

        face = faces[0]

        check_image_quality(img, face, pose)
        check_pose(face, pose)

        H, W = img.shape[:2]
        x1 = max(0, int(face.bbox[0]))
        y1 = max(0, int(face.bbox[1]))
        x2 = min(W, int(face.bbox[2]))
        y2 = min(H, int(face.bbox[3]))

        crop = img[y1:y2, x1:x2]

        result[pose] = {
            "face": face,
            "embedding": face.normed_embedding,
            "crop": crop
        }

    # check cùng 1 người
    center_emb = result["center"]["embedding"]

    for pose in ["left", "right"]:
        sim = cosine_similarity(center_emb, result[pose]["embedding"])

        if sim < 0.6:
            raise AppException(
                422,
                "DIFFERENT_PERSON",
                f"Image for pose {pose} does not match center image"
            )

    return result