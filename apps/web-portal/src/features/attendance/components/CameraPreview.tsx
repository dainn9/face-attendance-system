import type { RefObject } from "react";
import {
    FaArrowLeft,
    FaCamera,
    FaCheck,
    FaExclamationTriangle,
    FaSpinner,
} from "react-icons/fa";
import { Link } from "react-router-dom";
import AttendanceInfoCard, {
    type AttendanceInfoItem,
} from "./AttendanceInfoCard";

type CameraPreviewProps = {
    backPath: string;
    cameraActive: boolean;
    cameraError: string;
    cameraInstruction: string;
    centerCaptured: boolean;
    challengeInstruction: string;
    countdown: number | null;
    errorMessage: string;
    hasCameraStream: boolean;
    isFlashActive: boolean;
    isRetryingCheckin: boolean;
    isStartingCamera: boolean;
    livenessDone: boolean;
    onBeginCapture: () => void;
    onRetryCheckin: () => void;
    onStartCamera: () => void;
    progressPercent: number;
    sessionInfo: AttendanceInfoItem[];
    stage:
        | "ready"
        | "camera"
        | "center-countdown"
        | "center-captured"
        | "challenge-countdown"
        | "verifying"
        | "success"
        | "error";
    statusMessage: string;
    videoRef: RefObject<HTMLVideoElement | null>;
};

const CameraPreview = ({
    backPath,
    cameraActive,
    cameraError,
    cameraInstruction,
    centerCaptured,
    challengeInstruction,
    countdown,
    errorMessage,
    hasCameraStream,
    isFlashActive,
    isRetryingCheckin,
    isStartingCamera,
    livenessDone,
    onBeginCapture,
    onRetryCheckin,
    onStartCamera,
    progressPercent,
    sessionInfo,
    stage,
    statusMessage,
    videoRef,
}: CameraPreviewProps) => {
    const showCameraError = cameraError && stage !== "error";

    return (
        <div className="flex min-h-[calc(100vh-88px)]">
            <section className="flex w-full flex-col bg-white shadow-sm ring-1 ring-blue-100">
                <div className="border-b border-blue-100 px-4 py-5 sm:px-6">
                    <h1 className="text-2xl font-semibold text-gray-900">
                        Điểm danh và xác thực khuôn mặt
                    </h1>
                    <p className="mt-2 text-sm leading-6 text-gray-600">
                        Vui lòng làm theo hướng dẫn để hoàn tất điểm danh cho
                        buổi học này.
                    </p>
                </div>

                <div className="space-y-6 px-4 py-5 sm:px-6">
                    <AttendanceInfoCard items={sessionInfo} />

                    <div
                        className={`relative mx-auto flex aspect-4/3 w-full max-w-3xl items-center justify-center overflow-hidden rounded-2xl border-2 bg-slate-950 shadow-sm transition ${
                            cameraActive
                                ? "border-blue-500"
                                : "border-slate-800"
                        }`}
                    >
                        <div className="absolute inset-0 opacity-30 [background:linear-gradient(90deg,rgba(255,255,255,.08)_1px,transparent_1px),linear-gradient(0deg,rgba(255,255,255,.08)_1px,transparent_1px)] bg-size-[32px_32px]" />
                        <div className="absolute inset-x-0 top-0 h-24 bg-linear-to-b from-blue-500/20 to-transparent" />

                        <video
                            ref={videoRef}
                            autoPlay
                            muted
                            playsInline
                            className={`absolute inset-0 h-full w-full -scale-x-100 object-cover transition-opacity ${
                                cameraActive ? "opacity-100" : "opacity-0"
                            }`}
                        />

                        <div className="relative flex h-[62%] w-[38%] min-w-40 items-center justify-center rounded-[50%] border-4 border-blue-300/90 shadow-[0_0_36px_rgba(59,130,246,.45)]">
                            <div className="absolute inset-4 rounded-[50%] border border-white/25" />
                            <span className="text-5xl text-blue-200/30">
                                <FaCamera />
                            </span>
                        </div>

                        <div className="absolute left-4 top-4 inline-flex items-center gap-2 rounded-full bg-black/45 px-3 py-1 text-xs font-medium text-white backdrop-blur">
                            <span
                                className={`size-2 rounded-full ${
                                    cameraActive
                                        ? "bg-emerald-400"
                                        : "bg-gray-400"
                                }`}
                            />
                            {cameraActive
                                ? "Camera hoạt động"
                                : "Camera chưa bật"}
                        </div>

                        {countdown !== null && (
                            <div className="absolute inset-0 flex items-center justify-center bg-black/35">
                                <span className="text-8xl font-semibold text-white drop-shadow-lg">
                                    {countdown}
                                </span>
                            </div>
                        )}

                        <div
                            className={`pointer-events-none absolute inset-0 bg-white transition-opacity duration-200 ${
                                isFlashActive ? "opacity-90" : "opacity-0"
                            }`}
                        />

                        <div className="absolute bottom-4 left-4 right-4 rounded-lg bg-black/55 px-4 py-3 text-center text-sm font-medium text-white backdrop-blur">
                            {cameraInstruction}
                        </div>
                    </div>

                    <div className="mx-auto max-w-3xl space-y-4">
                        {showCameraError && (
                            <div className="flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
                                <FaExclamationTriangle className="mt-0.5 shrink-0" />
                                <p className="font-medium">{cameraError}</p>
                            </div>
                        )}

                        {stage === "error" && (
                            <div className="flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
                                <FaExclamationTriangle className="mt-0.5 shrink-0" />
                                <p className="font-medium">{errorMessage}</p>
                            </div>
                        )}

                        <div className="rounded-lg border border-blue-100 bg-blue-50 p-4">
                            <div className="flex items-center justify-between gap-3 text-sm">
                                <span className="font-semibold text-blue-950">
                                    Tiến trình xác thực
                                </span>
                                <span className="font-semibold text-blue-700">
                                    {progressPercent}%
                                </span>
                            </div>
                            <div className="mt-3 h-2 rounded-full bg-white">
                                <div
                                    className="h-2 rounded-full bg-blue-600 transition-all duration-500"
                                    style={{ width: `${progressPercent}%` }}
                                />
                            </div>
                            <div className="mt-4 grid gap-3 sm:grid-cols-2">
                                <div className="flex items-center gap-3 rounded-md bg-white p-3">
                                    <span
                                        className={`flex size-8 items-center justify-center rounded-full text-xs ${
                                            centerCaptured
                                                ? "bg-emerald-100 text-emerald-700"
                                                : "bg-blue-100 text-blue-700"
                                        }`}
                                    >
                                        {centerCaptured ? <FaCheck /> : "1"}
                                    </span>
                                    <div>
                                        <p className="text-sm font-semibold text-gray-900">
                                            Bước 1: Chụp ảnh khuôn mặt chính
                                            diện (giữ khuôn mặt trong khung tròn
                                            cho đến khi hoàn tất)
                                        </p>
                                        <p className="text-xs text-gray-500">
                                            {centerCaptured
                                                ? "Hoàn tất"
                                                : "Đang chờ chụp..."}
                                        </p>
                                    </div>
                                </div>
                                <div className="flex items-center gap-3 rounded-md bg-white p-3">
                                    <span
                                        className={`flex size-8 items-center justify-center rounded-full text-xs ${
                                            livenessDone
                                                ? "bg-emerald-100 text-emerald-700"
                                                : "bg-blue-100 text-blue-700"
                                        }`}
                                    >
                                        {livenessDone ? <FaCheck /> : "2"}
                                    </span>
                                    <div>
                                        <p className="text-sm font-semibold text-gray-900">
                                            Bước 2: Kiểm tra liveness (di chuyển
                                            khuôn mặt theo hướng dẫn trên màn
                                            hình)
                                        </p>
                                        <p className="text-xs text-gray-500">
                                            {livenessDone
                                                ? "Hoàn tất"
                                                : challengeInstruction}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {stage !== "error" && (
                            <div className="flex items-start gap-3 rounded-lg border border-gray-200 bg-white p-4 text-sm text-gray-700 shadow-sm">
                                <span
                                    className={`mt-0.5 flex size-5 shrink-0 items-center justify-center rounded-full text-[10px] ${
                                        stage === "verifying"
                                            ? "bg-blue-100 text-blue-700"
                                            : "bg-emerald-100 text-emerald-700"
                                    }`}
                                >
                                    {stage === "verifying" ? (
                                        <FaSpinner className="animate-spin" />
                                    ) : (
                                        <FaCheck />
                                    )}
                                </span>
                                <p className="font-medium">{statusMessage}</p>
                            </div>
                        )}
                    </div>
                </div>

                <div className="border-t border-gray-100 px-4 py-4 sm:px-6">
                    <div className="mx-auto flex max-w-3xl flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                        <div className="flex flex-col gap-3 sm:flex-row">
                            {stage === "error" && (
                                <button
                                    type="button"
                                    className="inline-flex min-h-11 items-center justify-center gap-2 rounded-md border border-blue-200 px-4 py-2 text-sm font-semibold text-blue-700 transition hover:bg-blue-50 disabled:cursor-not-allowed disabled:border-blue-100 disabled:text-blue-300 disabled:hover:bg-transparent"
                                    disabled={isRetryingCheckin}
                                    onClick={onRetryCheckin}
                                >
                                    {isRetryingCheckin && (
                                        <FaSpinner className="animate-spin" />
                                    )}
                                    Thử lại
                                </button>
                            )}
                            <button
                                type="button"
                                className="inline-flex min-h-11 items-center justify-center gap-2 rounded-md bg-blue-600 px-5 py-2 text-sm font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
                                disabled={
                                    isStartingCamera ||
                                    (hasCameraStream && stage !== "camera")
                                }
                                onClick={
                                    hasCameraStream
                                        ? onBeginCapture
                                        : onStartCamera
                                }
                            >
                                {isStartingCamera ? (
                                    <FaSpinner className="animate-spin" />
                                ) : (
                                    <FaCamera />
                                )}
                                {!hasCameraStream
                                    ? "Bật Camera"
                                    : "Bắt đầu điểm danh"}
                            </button>
                        </div>
                    </div>
                    <Link
                        to={backPath}
                        className="mx-auto mt-3 inline-flex min-h-10 max-w-3xl items-center gap-2 text-sm font-medium text-gray-500 transition hover:text-gray-700"
                    >
                        <FaArrowLeft />
                        Về trang chủ
                    </Link>
                </div>
            </section>
        </div>
    );
};

export default CameraPreview;
