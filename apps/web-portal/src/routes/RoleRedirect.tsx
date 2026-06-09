import { Navigate } from "react-router-dom";
import { useAuthStore } from "../features/auth/store/auth.store";
import { getHomePathByRole } from "../features/auth/utils/roleRedirect";

const RoleRedirect = () => {
    const role = useAuthStore(s => s.user?.role);

    return <Navigate to={getHomePathByRole(role)} replace />;
};

export default RoleRedirect;
