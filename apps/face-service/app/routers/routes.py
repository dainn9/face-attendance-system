from random import choice
from fastapi import APIRouter, Depends, File, Form, UploadFile
from app.core.face_app import face_app
from app.core.exceptions import AppException
from app.core.responses import success_response
from app.schemas.face_schema import FaceIdentifyRequest
from app.utils.face_utils import extract_frames
from app.utils.face_validator import validate_face_continuity, validate_face_liveness_images, validate_face_register_images, validate_face_verify_image, validate_pose_sequence
from app.utils.image import base64_to_image
from app.services.recognition_service import RecognitionService
from app.services.detector_service import detect_faces
from app.core.auth import verify_api_key

from app.core.config import settings
from app.utils.read_upload_image import read_upload_image

router = APIRouter()

svc = RecognitionService(
    weight_path="checkpoints/best_mobilenet_margin0.2_dim256.pth",
    threshold= settings.THRESHOLD,
    uri=settings.URI_MILVUS,
    token=settings.TOKEN,
    collection_name=settings.COLLECTION_NAME,
    top_k=settings.TOP_K,
)

# Đăng ký ảnh mới vào hệ thống

@router.post("/register")
async def register_face(
    user_id: str = Form(...),
    left: UploadFile = File(...),
    center: UploadFile = File(...),
    right: UploadFile = File(...),
    _: None = Depends(verify_api_key)
):
    
    if svc.exists(user_id):
        raise AppException(
            409,
            "FACE_ALREADY_REGISTERED",
            "Face already registered"
        )
    
    try:
        images_by_pose = {}

        files = {
                    "left": left,
                    "center": center,
                    "right": right,
                }

        # 1. Decode đủ 3 ảnh trước
        for pose, file in files.items():
            images_by_pose[pose] = await read_upload_image(file)

         # 2. Sau đó mới validate toàn bộ 3 ảnh
        validated = validate_face_register_images(
            face_app,
            images_by_pose
        )

    # 3. Register
        registered = []

        for pose in ["left", "center", "right"]:
            svc.register(
                user_id=user_id,
                pose=pose,
                img=validated[pose]["crop"]
            )

            registered.append(pose)

    except AppException:
        raise

    except Exception as e:
        raise AppException(status_code=400, code="INTERNAL_ERROR", message=str(e))
    
    return None

@router.get("/status/{user_id}")
async def get_face_status(
    user_id: str,
    _: None = Depends(verify_api_key)
):  
    isRegistered = svc.exists(user_id)

    return success_response(
        message="Face status retrieved successfully", 
        data=isRegistered
    )


@router.post("/verify-liveness")
async def verify_face_liveness(
    user_id: str = Form(...),
    challenge: str = Form(...),
    video: UploadFile = File(...),
    _: None = Depends(verify_api_key)
):

    (
        center_img,
        action_img,
        center_ratio,
        action_ratio,
        center_frame_idx,
        action_frame_idx,
        valid_frame_indices
    ) = await extract_frames(
        video,
        challenge,
        face_app,
    )

    validate_face_continuity(valid_frame_indices)

    validate_pose_sequence(
        center_ratio=center_ratio,
        action_ratio=action_ratio,
        center_frame_idx=center_frame_idx,
        action_frame_idx=action_frame_idx,
        challenge=challenge,
    )

    live_face_crop = validate_face_liveness_images(
        face_app,
        center_img, action_img,
    )

    result = svc.verify(
        user_id=user_id,
        img=live_face_crop
    )

    return success_response(
        message="Face verified completely",
        data=result
    )
    

@router.post("/identify")
async def identify_face(
    req: FaceIdentifyRequest,
    _: None = Depends(verify_api_key)
):
    try:
        img = base64_to_image(req.image)
        faces = detect_faces(img, mode="single")
        if not faces:
            raise AppException(
                status_code=422,
                code="NO_FACE_DETECTED",
                message="No face detected in the image"
            )
        
        face_crop = faces[0]["crop"]
        result = svc.identify(img=face_crop, allowed_user_ids=req.allowed_user_ids)

    except AppException:
        raise

    except Exception as e:
        raise AppException(status_code=400, code="INTERNAL_ERROR", message=str(e))

    return success_response(
        message="Face identified completely", 
        data=result
    )

@router.delete("/remove/{user_id}")
async def remove_face(
    user_id: str,
    _: None = Depends(verify_api_key)
):
    try:
        svc.remove(user_id=user_id)
    except Exception as e:
        raise AppException(status_code=400, code="INTERNAL_ERROR", message=str(e))

    return success_response(message="Face removed successfully")

@router.get("/challenge")
async def get_challenge(
    _: None = Depends(verify_api_key)
):
    return success_response(
        message="Challenge generated successfully",
        data={
            "challenge": choice(["left", "right"])
        }
    )