type ToastProps = {
  message: string;
  type?: "success" | "error" | "warning";
  onClose: () => void;
};

const bgMap = {
  success: "bg-green-600",
  error: "bg-red-600",
  warning: "bg-yellow-500",
};

const Toast = ({ message, type = "success", onClose }: ToastProps) => {
  return (
    <div
      className={`fixed top-5 right-5 z-50 ${bgMap[type]} text-white px-4 py-3 rounded shadow-lg animate-slideIn`}
    >
      <div className="flex items-center gap-3">
        <span>{message}</span>
        <button onClick={onClose} className="font-bold">
          ✕
        </button>
      </div>
    </div>
  );
};

export default Toast;
