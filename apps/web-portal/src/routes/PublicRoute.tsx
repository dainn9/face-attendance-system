import { Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "../features/auth/store/auth.store";
import { getHomePathByRole } from "../features/auth/utils/roleRedirect";

const PublicRoute = () => {
    const isAuthenticated = useAuthStore(s => s.isAuthenticated);
    const isAuthLoading = useAuthStore(s => s.isAuthLoading);
    const role = useAuthStore(s => s.user?.role);

    if (isAuthLoading) return null;
    
    return isAuthenticated ? <Navigate to={getHomePathByRole(role)} replace /> : <Outlet/>
}

export default PublicRoute;
