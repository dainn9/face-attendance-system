from insightface.app import FaceAnalysis
import numpy as np
from app.core.config import settings

class FaceService:
    def __init__(self):
        self.app = FaceAnalysis(name=settings.MODEL_NAME)
        self.app.prepare(ctx_id=settings.CTX_ID)

    def get_embedding(self, image: np.ndarray) -> np.ndarray | None:
        faces = self.app.get(image)

        if len(faces) != 1:
            return None
        
        return faces[0].embedding

    def verify_multi(
        self, 
        emb_new: np.ndarray, 
        stored_embeddings: list[list[float]]
    ) -> tuple[bool, float]:
        """
        So sánh embedding mới với nhiều embedding lưu sẵn.
        Trả về (is_match, best_score).
        """
        best_score = 0.0

        for stored in stored_embeddings:
            emb_stored = np.array(stored, dtype=np.float32)
            score = float(
                np.dot(emb_new, emb_stored) / 
                (np.linalg.norm(emb_new) * np.linalg.norm(emb_stored))
            )
            if score > best_score:
                best_score = score

        return best_score >= settings.THRESHOLD, best_score