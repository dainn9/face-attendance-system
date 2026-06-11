from pydantic import BaseModel
from typing import List, Literal

class FaceIdentifyRequest(BaseModel):
    image: str
    allowed_user_ids: List[str]

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