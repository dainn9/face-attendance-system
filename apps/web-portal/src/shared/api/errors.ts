export class ApiError extends Error {
    status: number
    errorCode?: string

    constructor(status: number, message: string, errorCode?: string) {
        super(message);
        this.status = status;
        this.errorCode = errorCode;
    }
}

export class UnauthorizedError extends ApiError {
    constructor(message: string = "Unauthorized") {
        super(401, message);
    }
}

export class ForbiddenError extends ApiError {
    constructor(message: string = "Forbidden") {
        super(403, message);
    }
}

export class NotFoundError extends ApiError {
    constructor(message: string = "Not Found") {
        super(404, message);
    }
}

export class InternalServerError extends ApiError {
    constructor(message: string = "Internal Server Error") {
        super(500, message);
    }
}

type ValidationItem = {
    field: string;
    error: string;
};

export class ValidationError extends ApiError {
    errors: Record<string, string[]>

    constructor(details: unknown, message: string = "Validation Error") {
        super(422, message);
        this.errors = mapValidation(details);
    }

    get(field: string) {
        return this.errors[field];
    }
}

function mapValidation(details: unknown) {
    const errors: Record<string, string[]> = {}

    if (Array.isArray(details)) {
        details.forEach((item) => {
            if (!isValidationItem(item)) return;

            const key = normalizeField(item.field);
            if (!errors[key]) {
                errors[key] = []
            }
            errors[key].push(item.error)
        })
        return errors
    }

    if (details && typeof details === "object") {
        Object.entries(details).forEach(([field, value]) => {
            const key = normalizeField(field);
            const messages = Array.isArray(value) ? value : [value];

            messages.forEach((message) => {
                if (typeof message !== "string") return;

                if (!errors[key]) {
                    errors[key] = []
                }
                errors[key].push(message)
            })
        })
        return errors
    }

    if (typeof details === "string") {
        errors.general = [details]
    }

    return errors
}

function isValidationItem(value: unknown): value is ValidationItem {
    return (
        !!value &&
        typeof value === "object" &&
        "field" in value &&
        "error" in value &&
        typeof value.field === "string" &&
        typeof value.error === "string"
    )
}

function normalizeField(field: string) {
    if (!field) return "general";

    return field.charAt(0).toLowerCase() + field.slice(1)
}
