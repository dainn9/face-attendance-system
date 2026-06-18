from app.core.exceptions import AppException
from app.utils.face_utils import check_image_quality, check_pose, cosine_similarity


POSES = ["left", "center", "right"]

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

        H, W = img.shape[:2]
        x1 = max(0, int(face.bbox[0]))
        y1 = max(0, int(face.bbox[1]))
        x2 = min(W, int(face.bbox[2]))
        y2 = min(H, int(face.bbox[3]))

        crop = img[y1:y2, x1:x2]

        result[pose] = {
            "face": face,
            "embedding": face.normed_embedding,
            "crop": crop,
            "image": img
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

    for pose in POSES:
        img = result[pose]["image"]
        face = result[pose]["face"]

        check_image_quality(img, face, pose)
        check_pose(face, pose)

    return result

def validate_face_verify_image(face_app, image, blur_threshold=50):
    faces = face_app.get(image)

    if len(faces) == 0:
        raise AppException(
            422,
            "NO_FACE_DETECTED",
            "No face detected"
        )

    if len(faces) > 1:
        raise AppException(
            422,
            "MULTIPLE_FACES_DETECTED",
            "Multiple faces detected"
        )

    face = faces[0]

    H, W = image.shape[:2]
    x1 = max(0, int(face.bbox[0]))
    y1 = max(0, int(face.bbox[1]))
    x2 = min(W, int(face.bbox[2]))
    y2 = min(H, int(face.bbox[3]))

    crop = image[y1:y2, x1:x2]

    check_image_quality(image, face, blur_threshold=blur_threshold)

    return {
        "face": face,
        "embedding": face.normed_embedding,
        "crop": crop
    }

def validate_face_liveness_images(
        face_app,
        center_img,
        action_img,
    ):
    # validate center image, action image
    result_center = validate_face_verify_image(face_app, center_img, blur_threshold=20)
    result_action = validate_face_verify_image(face_app, action_img, blur_threshold=20)

    # validate pose center, action
    check_pose(result_center["face"], "center")

    # validate cùng 1 người
    sim = cosine_similarity(
        result_center["embedding"],
        result_action["embedding"]
    )

    if sim < 0.6:
        raise AppException(
            422,
            "DIFFERENT_PERSON",
            "Challenge image does not match center image"
        )

    return result_center["crop"]

def validate_pose_sequence(
    center_ratio,
    action_ratio,
    center_frame_idx,
    action_frame_idx,
    challenge,
    min_change=0.4,
    min_frame_gap=5,
):
    if center_frame_idx is None:
        raise AppException(
            422,
            "CENTER_FACE_NOT_FOUND",
            "Could not find center face frame"
        )

    if action_frame_idx is None:
        raise AppException(
            422,
            "ACTION_FACE_NOT_FOUND",
            "Could not find action face frame"
        )

    if action_frame_idx <= center_frame_idx:
        raise AppException(
            422,
            "INVALID_HEAD_MOVEMENT",
            "Action must happen after center pose"
        )

    frame_gap = (
        action_frame_idx
        - center_frame_idx
    )

    if frame_gap < min_frame_gap:
        raise AppException(
            422,
            "MOVEMENT_TOO_FAST",
            "Head movement is too short"
        )

    if challenge == "left":
        pose_change = (
            action_ratio
            - center_ratio
        )

        if pose_change < min_change:
            raise AppException(
                422,
                "INSUFFICIENT_POSE_CHANGE",
                "Head did not turn left enough"
            )

    elif challenge == "right":
        pose_change = (
            center_ratio
            - action_ratio
        )

        if pose_change < min_change:
            raise AppException(
                422,
                "INSUFFICIENT_POSE_CHANGE",
                "Head did not turn right enough"
            )

    else:
        raise AppException(
            422,
            "INVALID_CHALLENGE",
            f"Unsupported challenge: {challenge}"
        )

    return True

def validate_face_continuity(
    frame_indices,
    max_gap=15,
    min_frames=15
):
    if len(frame_indices) < min_frames:
        raise AppException(
            422,
            "INSUFFICIENT_FACE_FRAMES",
            "Not enough face frames detected"
        )

    gaps = [
        frame_indices[i] - frame_indices[i - 1]
        for i in range(1, len(frame_indices))
    ]

    largest_gap = max(gaps)

    if largest_gap > max_gap:
        raise AppException(
            422,
            "FACE_TRACKING_INTERRUPTED",
            "Face was not continuously visible during challenge"
        )

    return True