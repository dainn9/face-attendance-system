# services/liveness_service.py
import cv2
import numpy as np

class LivenessService:
    
    def check_multi_frame(self, frames: list[np.ndarray]) -> tuple[bool, str]:
        """
        Kiểm tra 3 frame:
        1. Có face không
        2. Có chuyển động nhỏ không (không phải ảnh tĩnh)
        3. Chất lượng ảnh đủ không
        """
        if len(frames) < 3:
            return False, "Not enough frames"

        # Kiểm tra blur — ảnh in ra thường bị blur hoặc có pattern
        for i, frame in enumerate(frames):
            if not self._is_quality_ok(frame):
                return False, f"Frame {i} quality too low"

        # Kiểm tra motion giữa các frame
        if not self._has_natural_motion(frames):
            return False, "No natural motion detected"

        return True, "OK"

    def _is_quality_ok(self, frame: np.ndarray) -> bool:
        """
        Laplacian variance — ảnh nét thì variance cao
        Ảnh in/photo of photo thường bị blur
        """
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        variance = cv2.Laplacian(gray, cv2.CV_64F).var()
        return variance > 100  # ngưỡng này cần test thực tế

    def _has_natural_motion(self, frames: list[np.ndarray]) -> bool:
        """
        So sánh frame 1 và frame 3
        Ảnh tĩnh → diff gần 0
        Mặt người thật → luôn có micro-movement nhỏ
        """
        gray1 = cv2.cvtColor(frames[0], cv2.COLOR_BGR2GRAY)
        gray3 = cv2.cvtColor(frames[2], cv2.COLOR_BGR2GRAY)
        
        diff = cv2.absdiff(gray1, gray3)
        motion_score = float(diff.mean())
        
        # Có chuyển động nhưng không quá nhiều (nếu quá nhiều = giả)
        return 1.0 < motion_score < 50.0