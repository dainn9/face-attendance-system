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

def validate_face_verify_image(face_app, image):
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

    check_image_quality(image, face)

    return {
        "face": face,
        "embedding": face.normed_embedding,
        "crop": crop
    }

def validate_face_liveness_images(face_app, center_img, action_img, challenge):
    center_face = validate_face_verify_image(face_app, center_img)
    action_face = validate_face_verify_image(face_app, action_img)

    check_pose(center_face["face"], "center")
    check_pose(action_face["face"], challenge)

    sim = cosine_similarity(
        center_face["embedding"],
        action_face["embedding"]
    )

    if sim < 0.6:
        raise AppException(
            422,
            "DIFFERENT_PERSON",
            "Challenge image does not match center image"
        )

    return center_face["crop"]

