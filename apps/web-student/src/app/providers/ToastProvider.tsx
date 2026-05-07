import { useEffect, useState } from "react";
import { toastEmitter } from "../../shared/utils/toastEmitter";
import Toast from "../../shared/components/Toast/Toast";

const ToastProvider = () => {
    const [toast, setToast] = useState<{
        message: string;
        type?: "success" | "error" | "warning";
    } | null>(null);

    useEffect(() => {
        toastEmitter.subscribe(setToast);
        return () => toastEmitter.unsubscribe();
    }, []);

    if (!toast) return null;

    return (
        <Toast
        message={toast.message}
        type={toast.type}
        onClose={() => setToast(null)}
        />
    );
}

export default ToastProvider