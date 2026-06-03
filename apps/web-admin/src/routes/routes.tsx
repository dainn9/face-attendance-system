import LoginPage from "../features/auth/pages/LoginPage";
import Layout from "../shared/components/Layout/Layout";
import PrivateRoute from "./PrivateRoute";
import PublicRoute from "./PublicRoute";
import FacultyListPage from "../features/faculties/pages/FacultyListPage";
import FacultyDetailPage from "../features/faculties/pages/FacultyDetailPage";
import UserPage from "../features/users/pages/UserPage";
import CreateUserPage from "../features/users/pages/CreateUserPage";
import UserDetailPage from "../features/users/pages/UserDetailPage";

export const routes = [
    {
        path: "/",
        element: <PrivateRoute />,
        // element: <PublicRoute />,
        children: [
            {
                element: <Layout />,
                children: [
                    { index: true, element: <div>Admin Dashboard</div> },
                    {
                        path: "faculties",
                        children: [
                            { index: true, element: <FacultyListPage /> },
                            { path: ":id", element: <FacultyDetailPage /> },
                        ],
                    },
                    {
                        path: "users",
                        children: [
                            { index: true, element: <UserPage /> },
                            { path: "create", element: <CreateUserPage /> },
                            { path: ":id", element: <UserDetailPage /> },
                        ],
                    },
                ],
            },
        ],
    },
    {
        path: "/login",
        element: <PublicRoute />,
        children: [
            { path: "", element: <LoginPage /> },
            { path: "reset-password", element: <div>Reset Password</div> },
        ],
    },
    {
        path: "*",
        element: <h1>404 - Page Not Found</h1>,
    },
];
