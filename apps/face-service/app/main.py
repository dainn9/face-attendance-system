from fastapi import FastAPI

app = FastAPI(title="Face Service")

@app.get("/health")
def health():
    return {"status": "ok"}