from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    MODEL_NAME: str = "buffalo_l"
    CTX_ID: int = -1
    THRESHOLD: float = 0.8

    class Config:
        env_file = ".env"

settings = Settings()