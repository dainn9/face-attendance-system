def success_response(message: str, data: dict = None):
    return {
        "success": True,
        "message": message,
        "data": data,
    }

def error_response(message: str, errorCode: str = None):
    return {
        "success": False,
        "message": message,
        "errorCode": errorCode
    }