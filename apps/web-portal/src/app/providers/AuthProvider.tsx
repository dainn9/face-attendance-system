import { useEffect} from "react"
import { useAuthStore } from "../../features/auth/store/auth.store";
import { useQueryClient } from "@tanstack/react-query";
import { authApi } from "../../features/auth/services/auth.api";
import { getMeFromPayload } from "../../features/auth/utils/roleRedirect";

const AuthProvider = ({children} : {children: React.ReactNode}) => {
    const queryClient = useQueryClient();
    const setUser = useAuthStore(s => s.setUser);
    const finishAuthLoading = useAuthStore(s => s.finishAuthLoading);
    
    useEffect(() => {
        let mounted = true;

        const initAuth = async () => {
            try {
                const payload = await queryClient.fetchQuery({
                    queryKey: ["me"],
                    queryFn: authApi.getMe,
                    retry: false,
                });

                if (mounted) setUser(getMeFromPayload(payload));
            } catch {
                if (mounted) setUser(null);
            } finally {
                if (mounted) finishAuthLoading();
            }
        };

        initAuth();

        return () => {
            mounted = false;
        };

    }, [queryClient, setUser, finishAuthLoading]);

    return <>{children}</>;
}

export default AuthProvider
