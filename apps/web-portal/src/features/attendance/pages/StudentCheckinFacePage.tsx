import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { FaExclamationTriangle } from "react-icons/fa";
import { Link, useSearchParams } from "react-router-dom";
import AttendanceClosedView from "../components/AttendanceClosedView";
import AttendanceSuccessView from "../components/AttendanceSuccessView";
import CameraPreview from "../components/CameraPreview";
import type { AttendanceInfoItem } from "../components/AttendanceInfoCard";
import { useCheckInAttendance } from "../hooks/attendance.mutation";
import {
    useAttendanceCheckInInfo,
    useChallengeCode,
} from "../hooks/attendance.query";
import { PATHS } from "../../../shared/constants/path";

type CheckinStage =
    | "ready"
    | "camera"
    | "center-countdown"
    | "center-captured"
    | "challenge-countdown"
    | "verifying"
    | "success"
    | "error";

const errorMessages = [
    "Không phát hiện khuôn mặt",
    "Phát hiện nhiều khuôn mặt",
    "Khuôn mặt không khớp",
    "Vui lòng quay mặt sang trái",
];

const getChallengeInstruction = (challenge?: string) => {
    if (!challenge) return "Dang tai thu thach...";

    const normalizedChallenge = challenge.toLowerCase();
    if (
        normalizedChallenge.includes("left") ||
        normalizedChallenge.includes("trai")
    ) {
        return "Quay mặt sang trái";
    }
    if (
        normalizedChallenge.includes("right") ||
        normalizedChallenge.includes("phai")
    ) {
        return "Quay mặt sang phải";
    }

    return challenge;
};

const getSupportedVideoMimeType = () => {
    if (typeof MediaRecorder === "undefined") return "";

    return (
        [
            "video/webm;codecs=vp9",
            "video/webm;codecs=vp8",
            "video/webm",
            "video/mp4",
        ].find((mimeType) => MediaRecorder.isTypeSupported(mimeType)) ?? ""
    );
};

const getVideoFileName = (mimeType: string) =>
    mimeType.includes("mp4") ? "attendance-checkin.mp4" : "attendance-checkin.webm";

const formatTimeToMinute = (time?: string | null) => {
    if (!time) return "--:--";

    return time.slice(0, 5);
};

const formatDate = (date?: string | null) => {
    if (!date) return "--";

    const parsedDate = new Date(date);
    if (Number.isNaN(parsedDate.getTime())) return date;

    return new Intl.DateTimeFormat("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
    }).format(parsedDate);
};

const getSessionStatusLabel = (status?: number) =>
    status === 1 ? "Đang mở" : "Đã đóng";

const StudentCheckinFacePage = () => {
    const [searchParams] = useSearchParams();
    const videoRef = useRef<HTMLVideoElement | null>(null);
    const streamRef = useRef<MediaStream | null>(null);
    const mediaRecorderRef = useRef<MediaRecorder | null>(null);
    const recordedChunksRef = useRef<Blob[]>([]);
    const recordedVideoRef = useRef<File | null>(null);
    const challengeRef = useRef<string | undefined>(undefined);
    const hasSubmittedCheckInRef = useRef(false);
    const isRetryingCheckinRef = useRef(false);
    const [stage, setStage] = useState<CheckinStage>("ready");
    const [countdown, setCountdown] = useState<number | null>(null);
    const [errorMessage, setErrorMessage] = useState(errorMessages[0]);
    const [cameraError, setCameraError] = useState("");
    const [isRetryingCheckin, setIsRetryingCheckin] = useState(false);
    const [isStartingCamera, setIsStartingCamera] = useState(false);
    const [hasCameraStream, setHasCameraStream] = useState(false);
    const [isFlashActive, setIsFlashActive] = useState(false);
    const courseId = searchParams.get("courseId");
    const sessionId = searchParams.get("sessionId");
    const hasCheckinContext = !!courseId && !!sessionId;
    const checkInInfoQuery = useAttendanceCheckInInfo(sessionId ?? undefined);
    const challengeQuery = useChallengeCode(hasCheckinContext);
    const checkInAttendanceMutation = useCheckInAttendance();
    const checkInInfo = checkInInfoQuery.data;
    const challenge = challengeQuery.data?.challenge;
    useEffect(() => {
        challengeRef.current = challenge;
    }, [challenge]);
    const challengeInstruction = getChallengeInstruction(challenge);
    const courseSectionCode =
        checkInInfo?.courseSectionCode ??
        checkInInfo?.CourseSectionCode ??
        "--";
    const classInfo: AttendanceInfoItem[] = [
        ["Môn học", checkInInfo?.subjectName ?? "--"],
        ["Lớp học phần", courseSectionCode],
        ["Ngày điểm danh", formatDate(checkInInfo?.date)],
        ["Giờ bắt đầu", formatTimeToMinute(checkInInfo?.startTime)],
        ["Trạng thái", getSessionStatusLabel(checkInInfo?.status)],
    ];
    const isSessionClosed =
        checkInInfo?.status !== undefined && checkInInfo.status !== 1;

    const cameraActive = hasCameraStream && stage !== "success";
    const centerCaptured = [
        "center-captured",
        "challenge-countdown",
        "verifying",
        "success",
    ].includes(stage);
    const livenessDone = ["verifying", "success"].includes(stage);

    const progressPercent = useMemo(() => {
        if (stage === "success") return 100;
        if (centerCaptured && livenessDone) return 85;
        if (centerCaptured) return 50;
        if (cameraActive) return 25;
        return 0;
    }, [cameraActive, centerCaptured, livenessDone, stage]);

    const cameraInstruction = useMemo(() => {
        if (stage === "ready") return "Bật camera để bắt đầu";
        if (stage === "center-countdown") return "Nhìn thẳng vào camera";
        if (stage === "center-captured") return "Đã chụp ảnh chính diện";
        if (stage === "challenge-countdown") return challengeInstruction;
        if (stage === "verifying") return "Đang xác thực khuôn mặt...";
        if (stage === "error") return "Điều chỉnh khuôn mặt và thử lại";
        return "Nhìn thẳng vào camera";
    }, [challengeInstruction, stage]);

    const statusMessage = useMemo(() => {
        if (stage === "center-captured") return "Đã chụp ảnh chính diện";
        if (stage === "challenge-countdown") return challengeInstruction;
        if (stage === "verifying") return "Đang xác thực khuôn mặt...";
        if (stage === "success") return "Điểm danh thành công";
        if (stage === "error") return errorMessage;
        if (cameraActive) return "Đưa khuôn mặt vào vùng hướng dẫn";
        return "Sẵn sàng điểm danh";
    }, [cameraActive, challengeInstruction, errorMessage, stage]);

    useEffect(() => {
        const video = videoRef.current;
        const stream = streamRef.current;

        if (!video || !stream) return;

        video.srcObject = stream;
    });

    useEffect(() => {
        return () => {
            if (mediaRecorderRef.current?.state === "recording") {
                mediaRecorderRef.current.stop();
            }
            streamRef.current?.getTracks().forEach((track) => track.stop());
            streamRef.current = null;
        };
    }, []);

    useEffect(() => {
        if (!isSessionClosed) return;

        if (mediaRecorderRef.current?.state === "recording") {
            mediaRecorderRef.current.stop();
        }
        streamRef.current?.getTracks().forEach((track) => track.stop());
        streamRef.current = null;

        const resetTimer = window.setTimeout(() => {
            setHasCameraStream(false);
            setStage("ready");
        }, 0);

        return () => window.clearTimeout(resetTimer);
    }, [isSessionClosed]);

    const playFlash = useCallback(() =>
        new Promise<void>((resolve) => {
            setIsFlashActive(true);
            window.setTimeout(() => {
                setIsFlashActive(false);
                resolve();
            }, 220);
        }), []);

    const stopRecording = useCallback(() =>
        new Promise<File | null>((resolve) => {
            const recorder = mediaRecorderRef.current;

            if (!recorder) {
                resolve(null);
                return;
            }

            const buildVideoFile = () => {
                const mimeType = recorder.mimeType || "video/webm";
                const blob = new Blob(recordedChunksRef.current, { type: mimeType });

                if (blob.size === 0) return null;

                return new File([blob], getVideoFileName(mimeType), {
                    type: mimeType,
                });
            };

            if (recorder.state === "inactive") {
                resolve(buildVideoFile());
                return;
            }

            recorder.onstop = () => {
                resolve(buildVideoFile());
            };
            recorder.onerror = () => {
                resolve(null);
            };
            recorder.stop();
        }), []);

    const finishRecording = useCallback(async () => {
        await playFlash();

        const video = await stopRecording();
        if (!video) {
            setErrorMessage("Không thể quay video điểm danh");
            setStage("error");
            return;
        }

        recordedVideoRef.current = video;
        setStage("verifying");
    }, [playFlash, stopRecording]);

    const startRecording = useCallback(() => {
        const recorder = mediaRecorderRef.current;

        if (!recorder || recorder.state !== "inactive") {
            setErrorMessage("Không thể quay video điểm danh");
            setStage("error");
            return false;
        }

        recorder.start();
        return true;
    }, []);

    useEffect(() => {
        if (stage !== "center-countdown" && stage !== "challenge-countdown")
            return;

        const timer = window.setInterval(() => {
            setCountdown((current) => {
                if (current === null || current <= 1) {
                    window.clearInterval(timer);
                    window.setTimeout(() => {
                        void (async () => {
                            if (stage === "center-countdown") {
                                if (!startRecording()) return;
                                await playFlash();
                                setStage("center-captured");
                                return;
                            }

                            await finishRecording();
                        })();
                    }, 250);
                    return null;
                }

                return current - 1;
            });
        }, 800);

        return () => window.clearInterval(timer);
    }, [finishRecording, playFlash, stage, startRecording]);

    useEffect(() => {
        if (stage !== "center-captured") return;

        const timer = window.setTimeout(() => {
            setCountdown(3);
            setStage("challenge-countdown");
        }, 900);

        return () => window.clearTimeout(timer);
    }, [stage]);

    useEffect(() => {
        if (stage !== "verifying" || hasSubmittedCheckInRef.current) return;

        let errorTimer: number | undefined;
        const setAsyncError = (message: string) => {
            errorTimer = window.setTimeout(() => {
                setErrorMessage(message);
                setStage("error");
            }, 0);
        };

        if (!sessionId || !challenge) {
            setAsyncError("Không thể tải thử thách điểm danh");
            return () => {
                if (errorTimer !== undefined) window.clearTimeout(errorTimer);
            };
        }
        if (!recordedVideoRef.current) {
            setAsyncError("Thiếu video điểm danh");
            return () => {
                if (errorTimer !== undefined) window.clearTimeout(errorTimer);
            };
        }

        hasSubmittedCheckInRef.current = true;
        checkInAttendanceMutation.mutate(
            {
                attendanceSessionId: sessionId,
                challenge,
                video: recordedVideoRef.current,
            },
            {
                onSuccess: () => {
                    setStage("success");
                },
                onError: (error) => {
                    setErrorMessage(
                        error instanceof Error
                            ? error.message
                            : "Không thể điểm danh",
                    );
                    setStage("error");
                },
            },
        );

        return () => {
            if (errorTimer !== undefined) window.clearTimeout(errorTimer);
        };
    }, [challenge, checkInAttendanceMutation, sessionId, stage]);

    const startCamera = async () => {
        setErrorMessage(errorMessages[0]);
        setCameraError("");
        setIsStartingCamera(true);

        try {
            const stream = await navigator.mediaDevices.getUserMedia({
                video: {
                    facingMode: "user",
                    width: { ideal: 1280 },
                    height: { ideal: 960 },
                },
                audio: false,
            });

            streamRef.current?.getTracks().forEach((track) => track.stop());
            streamRef.current = stream;
            setHasCameraStream(true);

            if (videoRef.current) {
                videoRef.current.srcObject = stream;
                videoRef.current.play().catch(() => {
                    // Ignore play() rejections when the stream is already attached.
                });
            }

            setStage("camera");
        } catch {
            setCameraError(
                "Không thể bật camera. Vui lòng cấp quyền camera và thử lại.",
            );
            setStage("ready");
        } finally {
            setIsStartingCamera(false);
        }
    };

    const beginCapture = () => {
        if (!hasCameraStream) return;
        if (!challengeRef.current) {
            setErrorMessage("Đang tải thử thách điểm danh");
            setStage("error");
            return;
        }

        const stream = streamRef.current;
        if (!stream) {
            setErrorMessage("Camera chưa sẵn sàng");
            setStage("error");
            return;
        }

        const mimeType = getSupportedVideoMimeType();
        if (typeof MediaRecorder === "undefined" || !mimeType) {
            setErrorMessage("Trình duyệt không hỗ trợ quay video điểm danh");
            setStage("error");
            return;
        }

        recordedChunksRef.current = [];
        recordedVideoRef.current = null;
        hasSubmittedCheckInRef.current = false;
        const recorder = new MediaRecorder(stream, { mimeType });
        recorder.ondataavailable = (event) => {
            if (event.data.size > 0) {
                recordedChunksRef.current.push(event.data);
            }
        };
        recorder.onerror = () => {
            setErrorMessage("Không thể quay video điểm danh");
            setStage("error");
        };
        mediaRecorderRef.current = recorder;
        setCountdown(3);
        setStage("center-countdown");
    };

    const retryCheckin = () => {
        if (!hasCameraStream) return;
        if (isRetryingCheckinRef.current) return;

        void (async () => {
            isRetryingCheckinRef.current = true;
            setIsRetryingCheckin(true);
            setErrorMessage(
                errorMessages[Math.floor(Math.random() * errorMessages.length)],
            );

            try {
                const refreshedChallenge = await challengeQuery.refetch();
                const nextChallenge = refreshedChallenge.data?.challenge;
                challengeRef.current = nextChallenge;

                if (!nextChallenge) {
                    setErrorMessage("Đang tải thử thách điểm danh");
                    setStage("error");
                    return;
                }

                beginCapture();
            } finally {
                isRetryingCheckinRef.current = false;
                setIsRetryingCheckin(false);
            }
        })();
    };

    if (!hasCheckinContext) {
        return (
            <div className="rounded-lg bg-white p-5 text-sm text-gray-600 shadow-sm">
                <h1 className="text-base font-semibold text-gray-900">
                    Chưa có phiên điểm danh đang mở
                </h1>
                <p className="mt-2">
                    Vui lòng vào điểm danh bằng nút trên thông báo lớp học đang
                    mở.
                </p>
                <Link
                    to={PATHS.STUDENT}
                    className="mt-4 inline-flex min-h-11 items-center justify-center rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-blue-700"
                >
                    Quay về lớp học của tôi
                </Link>
            </div>
        );
    }

    if (checkInInfoQuery.isLoading) {
        return (
            <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                Đang tải thông tin phiên điểm danh...
            </div>
        );
    }

    if (checkInInfoQuery.isError || !checkInInfo) {
        return (
            <div className="rounded-lg border border-red-100 bg-white p-5 text-sm text-gray-600 shadow-sm">
                <div className="flex items-start gap-3">
                    <span className="mt-0.5 text-red-600">
                        <FaExclamationTriangle />
                    </span>
                    <div>
                        <h1 className="text-base font-semibold text-gray-900">
                            Không thể tải thông tin điểm danh
                        </h1>
                        <p className="mt-1">
                            Đã có lỗi xảy ra khi tải thông tin điểm danh. Vui
                            lòng thử lại sau. Nếu lỗi vẫn tiếp diễn, vui lòng
                            liên hệ bộ phận hỗ trợ của chúng tôi.
                        </p>
                    </div>
                </div>
                <Link
                    to={PATHS.STUDENT}
                    className="mt-4 inline-flex min-h-11 items-center justify-center rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-blue-700"
                >
                    Quay về lớp học của tôi
                </Link>
            </div>
        );
    }

    if (isSessionClosed) {
        return (
            <AttendanceClosedView
                items={classInfo}
                studentPath={PATHS.STUDENT}
            />
        );
    }

    if (stage === "success") {
        return (
            <AttendanceSuccessView homePath={PATHS.HOME} items={classInfo} />
        );
    }

    return (
        <CameraPreview
            backPath={PATHS.HOME}
            cameraActive={cameraActive}
            cameraError={cameraError}
            cameraInstruction={cameraInstruction}
            centerCaptured={centerCaptured}
            challengeInstruction={challengeInstruction}
            countdown={countdown}
            errorMessage={errorMessage}
            hasCameraStream={hasCameraStream}
            isFlashActive={isFlashActive}
            isRetryingCheckin={isRetryingCheckin}
            isStartingCamera={isStartingCamera}
            livenessDone={livenessDone}
            onBeginCapture={beginCapture}
            onRetryCheckin={retryCheckin}
            onStartCamera={startCamera}
            progressPercent={progressPercent}
            sessionInfo={classInfo}
            stage={stage}
            statusMessage={statusMessage}
            videoRef={videoRef}
        />
    );
};

export default StudentCheckinFacePage;
