import { useEffect, useMemo, useRef, useState } from "react";
import { FaCheck, FaSpinner, FaTimes, FaUpload } from "react-icons/fa";
import { useFaceStatus } from "../hooks/face.query";
import { useRegisterFace } from "../hooks/face.mutation";
import { ApiError } from "../../../shared/api/errors";

type FaceSlotKey = "center" | "left" | "right";
type FaceStatus = "empty" | "selected";

type FaceSlot = {
  key: FaceSlotKey;
  title: string;
  subtitle: string;
  hint: string;
  placeholderClass: string;
};

const faceSlots: FaceSlot[] = [
  {
    key: "left",
    title: "Nghiêng trái",
    subtitle: "Left",
    hint: "Quay mặt trái 15-30 độ",
    placeholderClass: "-rotate-12",
  },
  {
    key: "center",
    title: "Chính diện",
    subtitle: "Center",
    hint: "Nhìn thẳng vào camera",
    placeholderClass: "rotate-0",
  },
  {
    key: "right",
    title: "Nghiêng phải",
    subtitle: "Right",
    hint: "Quay mặt phải 15-30 độ",
    placeholderClass: "rotate-12",
  },
];

const statusConfig: Record<
  FaceStatus,
  { label: string; className: string; icon: "check" | "error" | "spinner" | "none" }
> = {
  empty: {
    label: "Chưa tải ảnh",
    className: "bg-gray-100 text-gray-600",
    icon: "none",
  },
  selected: {
    label: "Đã chọn ảnh",
    className: "bg-emerald-50 text-emerald-700",
    icon: "check",
  },
};

const checklistItems = [
  "Khuôn mặt rõ nét",
  "Không đeo khẩu trang",
  "Không đeo kính râm",
  "Ánh sáng đầy đủ",
  "Chỉ xuất hiện một người trong ảnh",
];

const initialPreviews: Record<FaceSlotKey, string | undefined> = {
  center: undefined,
  left: undefined,
  right: undefined,
};

const initialFiles: Record<FaceSlotKey, File | undefined> = {
  center: undefined,
  left: undefined,
  right: undefined,
};

const StatusBadge = ({ status }: { status: FaceStatus }) => {
  const config = statusConfig[status];

  return (
    <span
      className={`inline-flex min-h-8 items-center gap-2 rounded-md px-3 py-1 text-xs font-medium ${config.className}`}
    >
      {config.icon === "spinner" && <FaSpinner className="animate-spin" />}
      {config.icon === "check" && <FaCheck />}
      {config.icon === "error" && <FaTimes />}
      {config.label}
    </span>
  );
};

const FacePlaceholder = ({ className }: { className: string }) => (
  <div className={`relative flex size-28 items-center justify-center ${className}`}>
    <div className="absolute inset-0 rounded-full border-2 border-blue-200 bg-blue-50" />
    <div className="absolute top-8 flex w-12 justify-between">
      <span className="size-2 rounded-full bg-blue-500" />
      <span className="size-2 rounded-full bg-blue-500" />
    </div>
    <div className="absolute bottom-9 h-2 w-8 rounded-full border-b-2 border-blue-500" />
    <div className="absolute -bottom-4 h-8 w-20 rounded-t-full bg-blue-100" />
  </div>
);

const StudentFaceProfilePage = () => {
  const { data: hasRegisteredFace = false, isLoading: isFaceStatusLoading } =
    useFaceStatus();
  const registerFaceMutation = useRegisterFace();
  const [previews, setPreviews] =
    useState<Record<FaceSlotKey, string | undefined>>(initialPreviews);
  const [files, setFiles] =
    useState<Record<FaceSlotKey, File | undefined>>(initialFiles);
  const previewUrlsRef =
    useRef<Record<FaceSlotKey, string | undefined>>(initialPreviews);
  const [submitState, setSubmitState] = useState<"idle" | "success" | "error">(
    "idle",
  );
  const [submitErrorMessage, setSubmitErrorMessage] = useState("");

  const allImagesSelected = useMemo(
    () => faceSlots.every((slot) => files[slot.key]),
    [files],
  );
  const isFaceProfileCompleted = hasRegisteredFace || submitState === "success";
  const isFormDisabled =
    registerFaceMutation.isPending ||
    isFaceStatusLoading ||
    hasRegisteredFace;

  useEffect(
    () => () => {
      Object.values(previewUrlsRef.current).forEach((preview) => {
        if (preview) URL.revokeObjectURL(preview);
      });
    },
    [],
  );

  const handleFileChange = (slotKey: FaceSlotKey, file?: File) => {
    if (!file || isFormDisabled) return;

    setSubmitState("idle");
    setSubmitErrorMessage("");
    setFiles((current) => ({ ...current, [slotKey]: file }));
    setPreviews((current) => {
      const oldPreview = current[slotKey];
      if (oldPreview) URL.revokeObjectURL(oldPreview);
      const nextPreview = URL.createObjectURL(file);
      previewUrlsRef.current = {
        ...previewUrlsRef.current,
        [slotKey]: nextPreview,
      };

      return {
        ...current,
        [slotKey]: nextPreview,
      };
    });
  };

  const handleSubmit = async () => {
    if (!allImagesSelected || isFormDisabled) return;
    const { center, left, right } = files;
    if (!center || !left || !right) return;

    setSubmitState("idle");
    setSubmitErrorMessage("");

    try {
      await registerFaceMutation.mutateAsync({ left, center, right });
      setSubmitState("success");
    } catch (error) {
      setSubmitErrorMessage(
        error instanceof ApiError || error instanceof Error
          ? error.message
          : "Không thể cập nhật khuôn mặt, vui lòng thử lại",
      );
      setSubmitState("error");
    }
  };

  return (
    <div className="flex min-h-[calc(100vh-88px)] w-full">
      <section className="flex w-full flex-col bg-white shadow-sm ring-1 ring-blue-100">
        <div className="border-b border-blue-100 px-4 py-5 sm:px-6 lg:px-8">
          <div className="w-full">
            <h1 className="text-2xl font-semibold text-gray-900">
              Cập nhật khuôn mặt
            </h1>
            <p className="mt-2 max-w-3xl text-sm leading-6 text-gray-600">
              Vui lòng cung cấp 3 ảnh khuôn mặt (chính diện, nghiêng trái,
              nghiêng phải) để hệ thống nhận diện chính xác hơn.
            </p>
          </div>
        </div>

        {isFaceProfileCompleted ? (
          <div className="flex flex-1 items-center px-4 py-10 sm:px-6 lg:px-8">
            <div className="mx-auto flex max-w-2xl flex-col items-center text-center">
              <span className="inline-flex size-16 items-center justify-center rounded-full bg-emerald-100 text-2xl text-emerald-700 ring-8 ring-emerald-50">
                <FaCheck />
              </span>
              <h2 className="mt-5 text-xl font-semibold text-gray-900">
                Đã cập nhật khuôn mặt
              </h2>
              <p className="mt-2 max-w-xl text-sm leading-6 text-gray-600">
                Hồ sơ khuôn mặt của bạn đã được ghi nhận. Bạn có thể sử dụng
                nhận diện khuôn mặt cho các chức năng điểm danh.
              </p>
              <div className="mt-6 grid w-full gap-3 text-left sm:grid-cols-3">
                <div className="rounded-lg border border-emerald-100 bg-emerald-50 p-4">
                  <p className="text-xs font-medium text-emerald-700">
                    Nghiêng trái
                  </p>
                  <p className="mt-1 text-sm font-semibold text-emerald-950">
                    Đã ghi nhận
                  </p>
                </div>
                <div className="rounded-lg border border-emerald-100 bg-emerald-50 p-4">
                  <p className="text-xs font-medium text-emerald-700">
                    Chính diện
                  </p>
                  <p className="mt-1 text-sm font-semibold text-emerald-950">
                    Đã ghi nhận
                  </p>
                </div>
                <div className="rounded-lg border border-emerald-100 bg-emerald-50 p-4">
                  <p className="text-xs font-medium text-emerald-700">
                    Nghiêng phải
                  </p>
                  <p className="mt-1 text-sm font-semibold text-emerald-950">
                    Đã ghi nhận
                  </p>
                </div>
              </div>
            </div>
          </div>
        ) : (
        <fieldset disabled={isFormDisabled} className="contents">
          <div className="mx-auto w-full max-w-6xl flex-1 space-y-6 px-4 py-5 sm:px-6 lg:px-8">
            <div className="rounded-lg border border-blue-100 bg-blue-50 p-4">
              <h2 className="text-sm font-semibold text-blue-950">
                Hướng dẫn ảnh hợp lệ
              </h2>
              <div className="mt-3 grid gap-2 text-sm text-blue-900 sm:grid-cols-2 lg:grid-cols-5">
                {checklistItems.map((item) => (
                  <div key={item} className="flex items-start gap-2">
                    <span className="mt-0.5 inline-flex size-5 shrink-0 items-center justify-center rounded-full bg-blue-600 text-[10px] text-white">
                      <FaCheck />
                    </span>
                    <span>{item}</span>
                  </div>
                ))}
              </div>
            </div>

            {submitState === "error" && (
              <div className="flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
                <FaTimes className="mt-0.5 shrink-0" />
                <p>
                  {submitErrorMessage ||
                    "Không thể cập nhật khuôn mặt, vui lòng thử lại"}
                </p>
              </div>
            )}

            <div className="grid gap-4 lg:grid-cols-3">
              {faceSlots.map((slot) => (
                <article
                  key={slot.key}
                  className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm"
                >
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <h2 className="text-base font-semibold text-gray-900">
                        {slot.title}
                      </h2>
                      <p className="mt-1 text-xs text-gray-500">
                        {slot.subtitle} - {slot.hint}
                      </p>
                    </div>
                    <StatusBadge status={files[slot.key] ? "selected" : "empty"} />
                  </div>

                  <div className="mt-4 flex aspect-square w-full items-center justify-center overflow-hidden rounded-lg border border-dashed border-blue-200 bg-blue-50">
                    {previews[slot.key] ? (
                      <img
                        src={previews[slot.key]}
                        alt={`Ảnh ${slot.title}`}
                        className="h-full w-full object-cover"
                      />
                    ) : (
                      <FacePlaceholder className={slot.placeholderClass} />
                    )}
                  </div>

                  <label
                    className={`mt-4 inline-flex min-h-11 w-full cursor-pointer items-center justify-center gap-2 rounded-md border border-blue-200 bg-white px-4 py-2 text-sm font-semibold text-blue-700 transition hover:bg-blue-50 ${isFormDisabled ? "pointer-events-none opacity-60" : ""}`}
                  >
                    <FaUpload />
                    Tải ảnh lên
                    <input
                      type="file"
                      accept="image/*"
                      disabled={isFormDisabled}
                      className="sr-only"
                      onChange={(event) =>
                        handleFileChange(slot.key, event.target.files?.[0])
                      }
                    />
                  </label>

                  <p className="mt-3 text-xs leading-5 text-gray-500">
                    Hỗ trợ JPG, PNG. Ảnh nên rõ mặt và không bị che khuất.
                  </p>
                </article>
              ))}
            </div>

            {allImagesSelected && (
              <div className="flex items-start gap-3 rounded-lg border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-800">
                <span className="mt-0.5 inline-flex size-5 shrink-0 items-center justify-center rounded-full bg-emerald-600 text-[10px] text-white">
                  <FaCheck />
                </span>
                <p className="font-medium">
                  Đã chọn đủ 3 ảnh. Bạn có thể cập nhật khuôn mặt.
                </p>
              </div>
            )}

          </div>

          <div className="border-t border-gray-100 px-4 py-4 sm:px-6 lg:px-8">
            <div className="mx-auto flex w-full max-w-6xl flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <p className="text-xs text-gray-500">
              Cần đủ 3 ảnh hợp lệ trước khi cập nhật.
            </p>
            <button
              type="button"
              className="inline-flex min-h-11 w-full items-center justify-center gap-2 rounded-md bg-blue-600 px-5 py-2 text-sm font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300 sm:w-auto"
              disabled={!allImagesSelected || isFormDisabled}
              onClick={handleSubmit}
            >
              {(registerFaceMutation.isPending || isFaceStatusLoading) && (
                <FaSpinner className="animate-spin" />
              )}
              {hasRegisteredFace
                ? "Đã cập nhật"
                : registerFaceMutation.isPending
                  ? "Đang cập nhật..."
                  : "Cập nhật khuôn mặt"}
            </button>
            </div>
          </div>
        </fieldset>
        )}
      </section>
    </div>
  );
};

export default StudentFaceProfilePage;
