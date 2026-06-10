import { Navigate, Outlet } from 'react-router-dom';
import { PATHS } from '../shared/constants/path';
import { useAuthStore } from '../features/auth/store/auth.store';

const PrivateRoute = () => {
    const isAuthenticated = useAuthStore(s => s.isAuthenticated);
    const isAuthLoading = useAuthStore(s => s.isAuthLoading);

    if (isAuthLoading) return null;

    return isAuthenticated ? <Outlet/> : <Navigate to={PATHS.LOGIN} replace />;
}

export default PrivateRoute 
