import numpy as np
import cv2
from fastapi import APIRouter, Depends, File, Form, UploadFile
from app.core.face_app import face_app
from app.core.exceptions import AppException
from app.core.responses import success_response
from app.schemas.face_schema import FaceIdentifyRequest
from app.utils.face_register_validator import validate_face_register_images
from app.utils.image import base64_to_image
from app.services.recognition_service import RecognitionService
from app.services.detector_service import detect_faces
from app.core.auth import verify_api_key

from app.core.config import settings

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
            contents = await file.read()

            img = cv2.imdecode(
                np.frombuffer(contents, np.uint8),
                cv2.IMREAD_COLOR
            )

            if img is None:
                raise AppException(
                    status_code=422,
                    code="INVALID_IMAGE",
                    message=f"Invalid image for pose {pose}"
                )

            images_by_pose[pose] = img

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
        message="Face identified successfully", 
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
        