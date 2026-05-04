from fastapi import FastAPI
from app.routers.routes import router

app = FastAPI(title="Face Service")

@app.get("/health")
def health():
    return {"status": "ok"}

app.include_router(router, prefix="/api/v1")

