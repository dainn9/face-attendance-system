import torch.nn as nn
import torch.nn.functional as F
from torchvision import models
from torchvision.models import MobileNet_V3_Large_Weights

class FaceEmbedder(nn.Module):
    def __init__(self, embedding_dim=256):
        super().__init__()

        backbone = models.mobilenet_v3_large(weights=MobileNet_V3_Large_Weights.DEFAULT)
        self.backbone = backbone.features
        self.pool = nn.AdaptiveAvgPool2d((1, 1))
        self.projector = nn.Sequential(
            nn.Linear(960, embedding_dim),
            nn.BatchNorm1d(embedding_dim)
        )

    def forward(self, x):
        x = self.backbone(x)
        x = self.pool(x)
        x = x.flatten(1)
        x = self.projector(x)
        return F.normalize(x, p=2, dim=1)