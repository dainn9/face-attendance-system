import type { ButtonHTMLAttributes } from "react";

type Props = ButtonHTMLAttributes<HTMLButtonElement> & {
    variant?: "primary" | "secondary";
};

const Button = ({
    variant = "primary",
    children,
    className = "",
    ...props
}: Props) => {
    const styles = {
        primary: "bg-blue-600 text-white hover:bg-blue-700",

        secondary: "border border-gray-200 text-gray-600 hover:bg-gray-50",
    };

    return (
        <button
            className={`rounded-xl px-5 py-2.5 text-sm font-semibold ${styles[variant]} ${className}`}
            {...props}
        >
            {children}
        </button>
    );
};

export default Button;
