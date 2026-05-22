type ToastType = "success" | "error" | "warning";

type ToastPayload = {
  message: string;
  type?: ToastType;
};

type Listener = (payload: ToastPayload) => void;

let listener: Listener | null = null;

export const toastEmitter = {
  subscribe(fn: Listener) {
    listener = fn;
  },
  unsubscribe() {
    listener = null;
  },
  success(message: string) {
    listener?.({ message, type: "success" });
  },
  error(message: string) {
    listener?.({ message, type: "error" });
  },
  warning(message: string) {
    listener?.({ message, type: "warning" });
  },
};
