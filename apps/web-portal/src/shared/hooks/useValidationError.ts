import { ValidationError } from "../api/errors";

export const useValidationError = (error: unknown) => {
    const getError = (field: string) => {
        if (error instanceof ValidationError) {
            return error.errors[field];
        }
        return null;
    };

    return { getError };
};