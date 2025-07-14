import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useUserInfoQuery } from "../../features/account/accountApi";
import AppLoading from "../shared/AppLoading";

export default function RequireAuth() {
  const { data: user, isLoading } = useUserInfoQuery();
  const location = useLocation();

  if (isLoading) return <AppLoading text="Loading" />;

  if (!user) {
    return <Navigate to="/login" state={{ from: location }} />;
  }

  const adminRoutes = ["/inventory", "/admin-dashboard"];

  if (
    adminRoutes.includes(location.pathname) &&
    !user.roles.includes("Admin")
  ) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
