from fastapi import FastAPI
from app.core.exceptions import AppException
from app.core.handlers import app_exception_handler
from app.core.responses import success_response
from app.routers.routes import router

app = FastAPI(title="Face Service")

app.add_exception_handler(
    AppException,
    app_exception_handler
)   

@app.get("/api/v1/health")
def health():
    return success_response(message="Service is healthy")

app.include_router(router, prefix="/api/v1/internal/faces")

