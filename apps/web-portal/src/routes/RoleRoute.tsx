import { Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "../features/auth/store/auth.store";
import { getHomePathByRole } from "../features/auth/utils/roleRedirect";

type RoleRouteProps = {
    allowedRoles: string[];
};

const RoleRoute = ({ allowedRoles }: RoleRouteProps) => {
    const role = useAuthStore(s => s.user?.role);
    const normalizedRole = role?.toLowerCase();
    const canAccess = Boolean(
        normalizedRole && allowedRoles.map(item => item.toLowerCase()).includes(normalizedRole),
    );

    return canAccess ? <Outlet /> : <Navigate to={getHomePathByRole(role)} replace />;
};

export default RoleRoute;
