from pydantic import BaseModel
from typing import List

class FaceImageItem(BaseModel):
    pose: str
    image: str

class FaceRegisterRequest(BaseModel):
    student_id: str
    class_id: str | None = None
    images: List[FaceImageItem]

class FaceSearchRequest(BaseModel):
    image: str
    class_id: str | None = None

# class VerifyMultiRequest(BaseModel):
#     image: str
#     embeddings: List[List[float]]  # list of multiple embeddings

# class VerifyMultiResponse(BaseModel):
#     success: bool
#     match: bool
#     best_score: float
#     message: str = ""

# class VerifyWithLivenessRequest(BaseModel):
#     frames: List[str]        # 3 frame base64
#     embeddings: List[List[float]]

# class VerifyWithLivenessResponse(BaseModel):
#     success: bool
#     match: bool
#     best_score: float
#     liveness: bool
#     message: str = ""