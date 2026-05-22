import LoginPage from "../features/auth/pages/LoginPage";
import Layout from "../shared/components/Layout/Layout";
import PrivateRoute from "./PrivateRoute";
import PublicRoute from "./PublicRoute";

export const routes = [
    {
        path: "/",
        element: <PrivateRoute />,
        children: [
            {
                element: <Layout />,
                children: [
                    { index: true, element: <div>Admin Dashboard</div> },
                ],
            },
        ],
    },
    {
        path: "/login",
        element: <PublicRoute/>,
        children: [
            { path: "", element: <LoginPage /> },
            { path: "reset-password", element: <div>Reset Password</div> },
        ],
    },
    {
        path: "*",
        element: <h1>404 - Page Not Found</h1>,
    }
];
