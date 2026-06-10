from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    MODEL_NAME: str = "buffalo_l"
    CTX_ID: int = -1
    THRESHOLD: float = 0.8
    COLLECTION_NAME: str = "face_collection"
    URI_MILVUS: str | None = None
    TOKEN: str | None = None
    AI_SERVICE_KEY: str | None = None
    TOP_K: int = 1

    class Config:
        env_file = ".env"

settings = Settings()