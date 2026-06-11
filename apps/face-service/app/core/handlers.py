from fastapi import Request
from fastapi.responses import JSONResponse

from .exceptions import AppException
from .responses import error_response

async def app_exception_handler(
    request: Request,
    exc: AppException
):
    return JSONResponse(
        status_code=exc.status_code,
        content=error_response(
            message=exc.message,
            errorCode=exc.code
        )
    )