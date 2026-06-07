export class ApiError extends Error {
    status: number

    constructor(status: number, message: string) {
        super(message);
        this.status = status;
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

    constructor(details: ValidationItem[]) {
        super(422, "Validation Error");
        this.errors = mapValidation(details);
    }

    get(field: string) {
        return this.errors[field];
    }
}

function mapValidation(details: ValidationItem[]) {
    const errors: Record<string, string[]> = {}

    details.forEach((item) => {
        const key = item.field.charAt(0).toLowerCase() + item.field.slice(1)
        if (!errors[key]) {
            errors[key] = []
        }
        errors[key].push(item.error)
    })
    return errors
}