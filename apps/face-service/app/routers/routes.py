from fastapi import APIRouter, Depends, HTTPException
from functools import lru_cache
from app.services.face_service import FaceService
from app.schemas.face_schema import FaceRequest, VerifyMultiRequest, VerifyMultiResponse
from app.utils.image import base64_to_image

router = APIRouter()

@lru_cache()
def get_face_service():
    return FaceService()

@router.post("/extract-embedding")
def extract_embedding(
    req: FaceRequest,
    svc: FaceService = Depends(get_face_service)
):
    try:
        img = base64_to_image(req.image)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))
    
    emb = svc.get_embedding(img)
    if emb is None:
        raise HTTPException(status_code=422, detail="No face detected")

    return {"success": True, "embedding": emb.tolist()}


@router.post("/verify-multi", response_model=VerifyMultiResponse)
def verify_multi(
    req: VerifyMultiRequest,
    svc: FaceService = Depends(get_face_service)
):
    if not req.embeddings:
        raise HTTPException(status_code=400, detail="No embeddings provided")

    img = base64_to_image(req.image)
    if img is None:
        raise HTTPException(status_code=400, detail="Invalid image data")

    emb_new = svc.get_embedding(img)
    if emb_new is None:
        return VerifyMultiResponse(
            success=True,
            match=False,
            best_score=0.0,
            message="No face detected in image"
        )

    is_match, best_score = svc.verify_multi(emb_new, req.embeddings)

    return VerifyMultiResponse(
        success=True,
        match=is_match,
        best_score=best_score
    )