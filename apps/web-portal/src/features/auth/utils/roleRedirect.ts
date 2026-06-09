import type { ApiResponse, Me } from "../../../shared/api/types";
import { PATHS } from "../../../shared/constants/path";

type MePayload = Me | ApiResponse<Me>;

export const getMeFromPayload = (payload: MePayload): Me => {
    if ("role" in payload) {
        return payload;
    }

    return payload.data;
};

export const getHomePathByRole = (role?: string | null) => {
    switch (role?.toLowerCase()) {
        case "lecturer":
            return PATHS.LECTURER;
        case "student":
            return PATHS.STUDENT;
        default:
            return PATHS.STUDENT;
    }
};
