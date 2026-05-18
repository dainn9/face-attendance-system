from fastapi import Header, HTTPException
from app.core.config import settings

AI_SERVICE_KEY = settings.AI_SERVICE_KEY

def verify_api_key(x_api_key: str = Header(None)):
    if x_api_key != AI_SERVICE_KEY:
        raise HTTPException(status_code=401, detail="Unauthorized")