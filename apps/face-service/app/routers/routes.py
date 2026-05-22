from fastapi import APIRouter, Depends, HTTPException
from app.schemas.face_schema import FaceIdentifyRequest, FaceRegisterRequest
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
    req: FaceRegisterRequest,
    _: None = Depends(verify_api_key)
):
    
    registered = []

    try:
        for item in req.images:
            # Chuyển base64 sang ảnh OpenCV
            img = base64_to_image(item.image)

            # Phát hiện khuôn mặt
            # Nếu không phát hiện được khuôn mặt nào hoặc phát hiện nhiều hơn 1 khuôn mặt, trả về lỗi
            faces = detect_faces(img, mode="single")
            if not faces:
                raise HTTPException(
                    status_code=422,
                    detail=f"No face detected in pose '{item.pose}'"
                )

            face_crop = faces[0]["crop"]     
            ## Đăng ký khuôn mặt vào hệ thống
            svc.register(user_id=req.user_id, pose=item.pose, img=face_crop)

            registered.append(item.pose)

    except HTTPException:
        raise

    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))
    
    return {
        "success": True,
        "message": "Face registered successfully",
        "result": {
            "user_id": req.user_id,
            "registered_poses": registered,
        }
    }

@router.post("/identify")
async def identify_face(
    req: FaceIdentifyRequest,
    _: None = Depends(verify_api_key)
):
    try:
        img = base64_to_image(req.image)
        faces = detect_faces(img, mode="single")
        if not faces:
            raise HTTPException(
                status_code=422,
                detail="No face detected in the image"
            )
        
        face_crop = faces[0]["crop"]
        result = svc.identify(img=face_crop, allowed_user_ids=req.allowed_user_ids)

    except HTTPException:
        raise

    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

    return {
        "success": True,
        "result": result
    }


@router.delete("/remove/{user_id}")
async def remove_face(
    user_id: str,
    _: None = Depends(verify_api_key)
):
    try:
        svc.remove(user_id=user_id)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

    return {
        "success": True,
        "message": "Face removed successfully"
    }

