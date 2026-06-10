import LoginPage from "../features/auth/pages/LoginPage";
import LecturerAttendanceHistoryPage from "../features/attendance/pages/LecturerAttendanceHistoryPage";
import LecturerAttendanceSessionPage from "../features/attendance/pages/LecturerAttendanceSessionPage";
import StudentCheckinFacePage from "../features/attendance/pages/StudentCheckinFacePage";
import LecturerCourseDashboardPage from "../features/courses/pages/LecturerCourseDashboardPage";
import LecturerCourseDetailPage from "../features/courses/pages/LecturerCourseDetailPage";
import StudentCourseDashboardPage from "../features/courses/pages/StudentCourseDashboardPage";
import StudentCourseDetailPage from "../features/courses/pages/StudentCourseDetailPage";
import StudentFaceProfilePage from "../features/face-profile/pages/StudentFaceProfilePage";
import ProfilePage from "../features/profile/pages/ProfilePage";
import ResultPage from "../features/profile/pages/ResultPage";
import Layout from "../shared/components/Layout/Layout";
import PrivateRoute from "./PrivateRoute";
import PublicRoute from "./PublicRoute";
import RoleRedirect from "./RoleRedirect";
import RoleRoute from "./RoleRoute";

export const routes = [
    {
        path: "/",
        element: <PrivateRoute />,
        children: [
            {
                element: <Layout />,
                children: [
                    { index: true, element: <RoleRedirect /> },
                    {
                        element: <RoleRoute allowedRoles={["lecturer"]} />,
                        children: [
                            { path: "lecturer", element: <LecturerCourseDashboardPage /> },
                            { path: "lecturer/courses/:courseId/attendance-history", element: <LecturerAttendanceHistoryPage /> },
                            { path: "lecturer/courses/:courseId/attendance-sessions/:sessionId", element: <LecturerAttendanceSessionPage /> },
                            { path: "lecturer/courses/:courseId", element: <LecturerCourseDetailPage /> },
                            { path: "lecturer/courses/:courseId/session", element: <LecturerAttendanceSessionPage /> },
                        ],
                    },
                    {
                        element: <RoleRoute allowedRoles={["student"]} />,
                        children: [
                            { path: "student", element: <StudentCourseDashboardPage /> },
                            { path: "student/courses/:courseId", element: <StudentCourseDetailPage /> },
                            { path: "student/checkin", element: <StudentCheckinFacePage /> },
                            { path: "update-face", element: <StudentFaceProfilePage /> },
                        ],
                    },
                    { path: "profile", element: <ProfilePage /> },
                    { path: "results", element: <ResultPage /> },

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
