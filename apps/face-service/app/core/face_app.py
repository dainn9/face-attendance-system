from insightface.app import FaceAnalysis
from app.core.config import settings

face_app = FaceAnalysis(
    name='buffalo_l',
    providers=['CUDAExecutionProvider'],
    allowed_modules=['detection','recognition']
)

face_app.prepare(ctx_id=settings.CTX_ID)