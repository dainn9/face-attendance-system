from pydantic import BaseModel
from typing import List

class FaceRequest(BaseModel):
    image: str  # base64

class VerifyMultiRequest(BaseModel):
    image: str
    embeddings: List[List[float]]  # list của nhiều embedding

class VerifyMultiResponse(BaseModel):
    success: bool
    match: bool
    best_score: float
    message: str = ""

class VerifyWithLivenessRequest(BaseModel):
    frames: List[str]        # 3 frame base64
    embeddings: List[List[float]]

class VerifyWithLivenessResponse(BaseModel):
    success: bool
    match: bool
    best_score: float
    liveness: bool
    message: str = ""