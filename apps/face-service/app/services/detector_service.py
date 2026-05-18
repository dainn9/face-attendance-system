from insightface.app import FaceAnalysis
from app.core.config import settings

app = FaceAnalysis(
    name='buffalo_l',
    providers=['CUDAExecutionProvider'],
    allowed_modules=['detection']
)

app.prepare(ctx_id=settings.CTX_ID)


def detect_faces(img, mode="single"):
    faces = app.get(img)

    if len(faces) == 0:
        return []

    h, w = img.shape[:2]
    results = []

    for face in faces:
        x1, y1, x2, y2 = map(int, face.bbox)

        x1 = max(0, x1)
        y1 = max(0, y1)
        x2 = min(w, x2)
        y2 = min(h, y2)

        if x2 <= x1 or y2 <= y1:
            continue

        crop = img[y1:y2, x1:x2]

        results.append({
            "bbox": [x1, y1, x2, y2],
            "crop": crop,
            "score": float(face.det_score)
        })

    if mode == "single":
        if len(results) != 1:
            return []
        return results

    if mode == "largest":
        return [max(
            results,
            key=lambda r: (r["bbox"][2] - r["bbox"][0]) * (r["bbox"][3] - r["bbox"][1])
        )]

    if mode == "multiple":
        return results