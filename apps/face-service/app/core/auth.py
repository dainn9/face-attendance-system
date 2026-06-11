from fastapi import Header, HTTPException
from app.core.config import settings
from app.core.exceptions import AppException

AI_SERVICE_KEY = settings.AI_SERVICE_KEY

def verify_api_key(x_api_key: str = Header(alias="X-Internal-Api-Key")):
    if x_api_key != AI_SERVICE_KEY:
        raise AppException(401, "INVALID_API_KEY", "Invalid API key")  