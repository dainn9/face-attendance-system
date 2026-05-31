export const getInitials = (name: string) =>
    name
        .split(" ")
        .map((x) => x[0])
        .join("")
        .slice(-2)
        .toUpperCase();