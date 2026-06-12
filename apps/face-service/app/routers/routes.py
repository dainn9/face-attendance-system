import numpy as np
import cv2
from fastapi import APIRouter, Depends, File, Form, UploadFile
from torchvision.io import read_image
from app.core.face_app import face_app
from app.core.exceptions import AppException
from app.core.responses import success_response
from app.schemas.face_schema import FaceIdentifyRequest
from app.utils.face_validator import validate_face_liveness_images, validate_face_register_images, validate_face_verify_image
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

@router.post("/verify")
async def verify_face(
    user_id: str = Form(...),
    image: UploadFile = File(...),
    _: None = Depends(verify_api_key)
):
    img = await read_upload_image(image)

    face_crop = validate_face_verify_image(face_app, img)

    result = svc.verify(
        user_id=user_id,
        img=face_crop["crop"]
    )

    return success_response(
        message="Face verified completely",
        data=result
    )

@router.post("/verify-liveness")
async def verify_face_liveness(
    user_id: str = Form(...),
    challenge: str = Form(...),
    center: UploadFile = File(...),
    action: UploadFile = File(...),
    _: None = Depends(verify_api_key)
):
    img_center = await read_upload_image(center)
    img_action = await read_upload_image(action)

    face_center_crop = validate_face_liveness_images(face_app, img_center, img_action, challenge)

    result = svc.verify(
        user_id=user_id,
        img=face_center_crop
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
        