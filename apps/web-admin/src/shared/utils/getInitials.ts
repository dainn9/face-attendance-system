export const getInitials = (name?: string) => {
    if (!name) return "?";

    return name
        .trim()
        .split(" ")
        .map((w) => w[0])
        .join("")
        .toUpperCase();
};