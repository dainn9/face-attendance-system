import torch
import torch.nn.functional as F
import numpy as np
from PIL import Image
from torchvision import transforms
from pymilvus import (
    connections,
    Collection, 
    CollectionSchema,
    FieldSchema,
    DataType,
    utility,
)

from app.services.face_embedder import FaceEmbedder
from app.core.config import settings

# ── Preprocessing giống lúc train ──────────────────────────────────────────
TRANSFORM = transforms.Compose([
    transforms.Resize((112, 112)),
    transforms.ToTensor(),
    transforms.Normalize(mean=[0.5, 0.5, 0.5], std=[0.5, 0.5, 0.5]),
])


class RecognitionService:
    def __init__(
        self,
        weight_path: str,
        embedding_dim: int = 256,
        device: str | None = None,
        threshold: float = 0.5,         # cosine similarity threshold
        uri: str | None = None,
        token: str | None = None,
        collection_name: str | None = None,
        top_k: int = 1,
    ):
        self.device = device or ("cuda" if torch.cuda.is_available() else "cpu")
        self.threshold = threshold
        self.embedding_dim = embedding_dim
        self.uri = uri
        self.token = token
        self.collection_name = collection_name
        self.top_k = top_k
        # Load model
        self.model = FaceEmbedder(embedding_dim=embedding_dim)
        state = torch.load(weight_path, map_location=self.device)
        self.model.load_state_dict(state)
        self.model.to(self.device)
        self.model.eval()

        # Kết nối Milvus
        connections.connect(alias="default", uri=uri, token=token)
        self.collection = self._get_or_create_collection()
        self.collection.load()
        print(f"[Milvus] ✓ Kết nối thành công — collection '{self.collection_name}' đã sẵn sàng")

    # ── Milvus setup ───────────────────────────────────────────────────────

    def _get_or_create_collection(self) -> Collection:
        if utility.has_collection(self.collection_name):
            return Collection(self.collection_name)

        fields = [
            FieldSchema(name="id",        dtype=DataType.INT64,        is_primary=True, auto_id=True),
            FieldSchema(name="student_id",     dtype=DataType.VARCHAR,       max_length=64),
            FieldSchema(name="class_id", dtype=DataType.VARCHAR, max_length=64),
            FieldSchema(name="pose", dtype=DataType.VARCHAR,  max_length=16),
            FieldSchema(name="embedding", dtype=DataType.FLOAT_VECTOR, dim=self.embedding_dim),
        ]
        schema = CollectionSchema(fields, description="Face embeddings")
        collection = Collection(self.collection_name, schema)

        # Index cho vector field
        collection.create_index(
            field_name="embedding",
            index_params={
                "metric_type": "COSINE",
                "index_type": "HNSW",
                "params": {"efConstruction": 200, "M": 16},
            },
        )
        print(f"[Milvus] ✓ Đã tạo collection '{self.collection_name}' mới với index HNSW")
        return collection

    # ── Helpers ────────────────────────────────────────────────────────────

    def _preprocess(self, img: np.ndarray) -> torch.Tensor:
        """numpy BGR (H,W,3) → tensor (1,3,H,W)"""
        rgb = img[:, :, ::-1].copy()           # BGR → RGB
        pil = Image.fromarray(rgb)
        tensor = TRANSFORM(pil).unsqueeze(0)   # (1,3,112,112)
        return tensor.to(self.device)

    @torch.inference_mode()
    def get_embedding(self, img: np.ndarray) -> list[float]:
        """Trả về embedding L2-normalized dạng list[float] (để insert vào Milvus)"""
        tensor = self._preprocess(img)
        emb = self.model(tensor).squeeze(0)    # (256,)
        return emb.cpu().tolist()

    # ── Database ───────────────────────────────────────────────────────────

    def register(self, student_id: str, class_id: str, pose: str, img: np.ndarray) -> None:
        """Đăng ký khuôn mặt mới vào Milvus."""
        emb = self.get_embedding(img)

        data = [{"student_id": student_id, "class_id": class_id, "pose": pose, "embedding": emb}]

        self.collection.insert(data)
        self.collection.flush()
        
        print(
            f"[register] ✓ "
            f"student_id='{student_id}' "
            f"pose='{pose}'")   

    def remove(self, student_id: str) -> None:
        """Xoá tất cả embedding của 1 student_id khỏi Milvus."""
        self.collection.delete(expr=f'student_id == "{student_id}"')
        self.collection.flush()
        print(f"[remove] ✓ Đã xoá '{student_id}'")

    # ── Core functions ─────────────────────────────────────────────────────
    def identify(
        self,
        img: np.ndarray,
        class_id: str | None = None,
    ) :
        """
        1-N Search: so sánh ảnh với tất cả embedding đã đăng ký trong Milvus, trả về kết quả tốt nhất.
         - Nếu có class_id thì chỉ search trong class đó
         - Trả về student_id, class_id, similarity, is_known (có match hay không), top_k (danh sách kết quả trả về)
         - Lưu ý: nếu best_score < threshold thì coi như không nhận diện được (is_known=False) dù vẫn trả về kết quả tốt nhất để tham khảo
        """
        emb = self.get_embedding(img)
        expr = None if class_id is None else f"class_id == '{class_id}'"

        results = self.collection.search(
            data=[emb],
            anns_field="embedding",
            param={"metric_type": "COSINE", "params": {"ef": 200}},
            limit=self.top_k,
            output_fields=["student_id", "class_id"],
            expr=expr,
        )

        hits = results[0]  # query đơn nên lấy kết quả đầu tiên

        top_results = [
            {"student_id": hit.entity.get("student_id"), "class_id": hit.entity.get("class_id"), "similarity": round(hit.score, 4)}
            for hit in hits
        ]

        if not top_results:
            return {"student_id": None, "class_id": None, "similarity": 0.0, "is_known": False, "top_k": []}

        best = top_results[0]
        is_known = best["similarity"] >= self.threshold

        return {
            "student_id": best["student_id"] if is_known else None,
            "class_id": best["class_id"] if is_known else None,
            "similarity": best["similarity"],
            "is_known": is_known,
            "top_k": top_results,
        }
    
    def verify(self, img: np.ndarray, student_id: str) -> dict:
        """So sánh ảnh với embedding đã đăng ký của student_id đó, trả về True nếu có match nào trên threshold."""
        emb = self.get_embedding(img)

        results = self.collection.search(
            data=[emb],
            anns_field="embedding",
            param={"metric_type": "COSINE", "params": {"ef": 200}},
            limit=1,
            expr=f'student_id == "{student_id}"',
        )

        hits = results[0]
        if not hits:
            return {"match": False, "score": 0.0}

        best_score = hits[0].score
        return {
            "match": best_score >= self.threshold,
            "score": round(best_score, 4)
        }
     