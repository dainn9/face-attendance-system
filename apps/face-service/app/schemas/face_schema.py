from pydantic import BaseModel
from typing import List

class FaceIdentifyRequest(BaseModel):
    image: str
    allowed_user_ids: List[str]