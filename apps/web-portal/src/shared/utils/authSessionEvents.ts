type AuthSessionExpiredListener = () => void;

const authSessionExpiredListeners = new Set<AuthSessionExpiredListener>();

export const subscribeAuthSessionExpired = (
    listener: AuthSessionExpiredListener,
) => {
    authSessionExpiredListeners.add(listener);

    return () => {
        authSessionExpiredListeners.delete(listener);
    };
};

export const emitAuthSessionExpired = () => {
    authSessionExpiredListeners.forEach((listener) => listener());
};
