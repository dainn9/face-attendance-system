import os
from pathlib import Path

import cv2
from fastapi import UploadFile
import numpy as np
from numpy.linalg import norm
import tempfile

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
    
    print("blur_score =", blur_score)
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

    print(f"Pose ratio for {expected_pose}: {ratio}")

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

    # 
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
    
async def extract_frames(
    video: UploadFile,
    challenge: str,
    face_app,
):
    video_bytes = await video.read()

    ext = os.path.splitext(video.filename or "")[1]
    
    if not ext:
        ext = ".tmp"

    fd, temp_path = tempfile.mkstemp(suffix=ext)

    # Lưu tạm video
    try:
        with os.fdopen(fd, "wb") as f:
            f.write(video_bytes)
     
        cap = cv2.VideoCapture(temp_path)

        if not cap.isOpened():
            raise AppException(
                422,
                "INVALID_VIDEO",
                "Cannot open uploaded video"
            )

        center_img = None
        action_img = None

        best_center_ratio = None
        best_action_ratio = None

        center_frame_idx = None
        action_frame_idx = None

        frame_index = 0

        valid_frame_indices = []

        while True:
            ret, frame = cap.read()

            if not ret:
                break

            frame_index += 1

            if frame_index % 3 != 0:
                continue

            faces = face_app.get(frame)

            if not faces:
                continue

            valid_frame_indices.append(frame_index)

            face = max(
                faces,
                key=lambda x: (x.bbox[2] - x.bbox[0]) * (x.bbox[3] - x.bbox[1])
            )

            ratio = estimate_pose_ratio(face.kps)

            # print(f"frame={frame_index}, ratio={ratio:.3f}, challenge={challenge}")

            if challenge == "left":
                # left = ratio tăng
                if (
                    ratio > CENTER_MAX
                    and (
                        best_action_ratio is None
                        or ratio > best_action_ratio
                    )
                ):
                    action_img = frame.copy()
                    best_action_ratio = ratio
                    action_frame_idx = frame_index
            
            elif challenge == "right":
                # right = ratio giảm
                if (
                    ratio < CENTER_MIN
                    and (
                        best_action_ratio is None
                        or ratio < best_action_ratio
                    )
                ):
                    action_img = frame.copy()
                    best_action_ratio = ratio
                    action_frame_idx = frame_index
            
            # center frame
            if CENTER_MIN <= ratio <= CENTER_MAX:
                if (
                    best_center_ratio is None
                    or abs(ratio - 1)
                    < abs(best_center_ratio - 1)
                ):
                    center_img = frame.copy()
                    best_center_ratio = ratio
                    center_frame_idx = frame_index
        
    finally:
        if 'cap' in locals():
                cap.release()

        if os.path.exists(temp_path):
            os.remove(temp_path)

    if center_img is None:
        raise AppException(
            422,
            "CENTER_FACE_NOT_FOUND",
            "Could not find a center face frame"
        )

    if action_img is None:
        raise AppException(
            422,
            "ACTION_FACE_NOT_FOUND",
            "Could not find an action face frame"
        )
    
    # cv2.imwrite("action.jpg", action_img)
    # cv2.imwrite("center.jpg", center_img)

    return (
        center_img,
        action_img,
        best_center_ratio,
        best_action_ratio,
        center_frame_idx,
        action_frame_idx,
        valid_frame_indices
    )