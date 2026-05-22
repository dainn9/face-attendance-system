import { useEffect} from "react"
import { useAuthStore } from "../../features/auth/store/auth.store";
import { useQueryClient } from "@tanstack/react-query";
import { authApi } from "../../features/auth/services/auth.api";

const AuthProvider = ({children} : {children: React.ReactNode}) => {
    const queryClient = useQueryClient();
    const setAuth = useAuthStore(s => s.setAuth);
    
    useEffect(() => {
        let mounted = true;

        const initAuth = async () => {
            try {
                await queryClient.fetchQuery({
                    queryKey: ["me"],
                    queryFn: authApi.getMe,
                    retry: false,
                });

                if (mounted) setAuth(true);
            } catch {
                if (mounted) setAuth(false);
            }
        };

        initAuth();

        return () => {
            mounted = false;
        };

    }, [queryClient, setAuth]);

    return <>{children}</>;
}

export default AuthProvider