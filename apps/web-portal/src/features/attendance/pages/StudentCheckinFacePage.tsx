import { useEffect, useMemo, useRef, useState } from "react";
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
        return "Quay mat sang trai";
    }
    if (
        normalizedChallenge.includes("right") ||
        normalizedChallenge.includes("phai")
    ) {
        return "Quay mat sang phai";
    }

    return challenge;
};

const captureVideoFrame = (video: HTMLVideoElement | null, fileName: string) =>
    new Promise<File | null>((resolve) => {
        if (!video || video.readyState < HTMLMediaElement.HAVE_CURRENT_DATA) {
            resolve(null);
            return;
        }

        const canvas = document.createElement("canvas");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        const context = canvas.getContext("2d");
        if (!context) {
            resolve(null);
            return;
        }

        context.save();
        context.translate(canvas.width, 0);
        context.scale(-1, 1);
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        context.restore();
        canvas.toBlob(
            (blob) => {
                if (!blob) {
                    resolve(null);
                    return;
                }

                resolve(new File([blob], fileName, { type: "image/jpeg" }));
            },
            "image/jpeg",
            0.92,
        );
    });

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
    const centerImageRef = useRef<File | null>(null);
    const challengeImageRef = useRef<File | null>(null);
    const challengeRef = useRef<string | undefined>(undefined);
    const hasSubmittedCheckInRef = useRef(false);
    const [stage, setStage] = useState<CheckinStage>("ready");
    const [countdown, setCountdown] = useState<number | null>(null);
    const [errorMessage, setErrorMessage] = useState(errorMessages[0]);
    const [cameraError, setCameraError] = useState("");
    const [isStartingCamera, setIsStartingCamera] = useState(false);
    const [hasCameraStream, setHasCameraStream] = useState(false);
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
            streamRef.current?.getTracks().forEach((track) => track.stop());
            streamRef.current = null;
        };
    }, []);

    useEffect(() => {
        if (!isSessionClosed) return;

        streamRef.current?.getTracks().forEach((track) => track.stop());
        streamRef.current = null;

        const resetTimer = window.setTimeout(() => {
            setHasCameraStream(false);
            setStage("ready");
        }, 0);

        return () => window.clearTimeout(resetTimer);
    }, [isSessionClosed]);

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
                                const centerImage = await captureVideoFrame(
                                    videoRef.current,
                                    "center.jpg",
                                );

                                if (!centerImage) {
                                    setErrorMessage(
                                        "Không thể chụp ảnh chính diện",
                                    );
                                    setStage("error");
                                    return;
                                }

                                centerImageRef.current = centerImage;
                                setStage("center-captured");
                                return;
                            }

                            const challengeImage = await captureVideoFrame(
                                videoRef.current,
                                "challenge.jpg",
                            );

                            if (!challengeImage) {
                                setErrorMessage("Không thể chụp ảnh thử thách");
                                setStage("error");
                                return;
                            }

                            challengeImageRef.current = challengeImage;
                            setStage("verifying");
                        })();
                    }, 250);
                    return null;
                }

                return current - 1;
            });
        }, 800);

        return () => window.clearInterval(timer);
    }, [stage]);

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
        if (!centerImageRef.current || !challengeImageRef.current) {
            setAsyncError("Thiếu ảnh điểm danh");
            return () => {
                if (errorTimer !== undefined) window.clearTimeout(errorTimer);
            };
        }

        hasSubmittedCheckInRef.current = true;
        checkInAttendanceMutation.mutate(
            {
                attendanceSessionId: sessionId,
                challenge,
                centerImage: centerImageRef.current,
                challengeImage: challengeImageRef.current,
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

        centerImageRef.current = null;
        challengeImageRef.current = null;
        hasSubmittedCheckInRef.current = false;
        setCountdown(3);
        setStage("center-countdown");
    };

    const retryCheckin = () => {
        if (!hasCameraStream) return;

        void (async () => {
            setErrorMessage(
                errorMessages[Math.floor(Math.random() * errorMessages.length)],
            );

            const refreshedChallenge = await challengeQuery.refetch();
            const nextChallenge = refreshedChallenge.data?.challenge;
            challengeRef.current = nextChallenge;

            if (!nextChallenge) {
                setErrorMessage("Đang tải thử thách điểm danh");
                setStage("error");
                return;
            }

            beginCapture();
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
