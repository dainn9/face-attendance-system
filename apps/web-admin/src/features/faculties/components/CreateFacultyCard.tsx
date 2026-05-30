import { FiPlus } from "react-icons/fi";

const CreateFacultyCard = () => {
    return (
        <button
            className="
                flex
                min-h-105
                w-full
                flex-col
                items-center
                justify-center
                gap-4
                rounded-2xl
                border-2
                border-dashed
                border-gray-300
                bg-white
                text-gray-500
                shadow-sm
                transition
                hover:border-blue-500
                hover:bg-blue-50
                hover:text-blue-600
            "
        >
            <div
                className="
                    flex
                    h-16
                    w-16
                    items-center
                    justify-center
                    rounded-full
                    bg-gray-100
                "
            >
                <FiPlus size={28} />
            </div>

            <div className="text-lg font-semibold">Thêm khoa mới</div>

            <p className="max-w-55 text-center text-sm text-gray-400">
                Tạo khoa mới và quản lý ngành học, giảng viên, sinh viên.
            </p>
        </button>
    );
};

export default CreateFacultyCard;
