import { Navigate, Outlet } from "react-router-dom";
import { PATHS } from "../shared/constants/path";
import { useAuthStore } from "../features/auth/store/auth.store";

const PublicRoute = () => {
    const isAuthenticated = useAuthStore(s => s.isAuthenticated);
    
    return isAuthenticated ? <Navigate to={PATHS.HOME} replace /> : <Outlet/>
}

export default PublicRoute;