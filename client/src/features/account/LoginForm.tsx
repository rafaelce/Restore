import { EmailOutlined, LockOutlined, Visibility, VisibilityOff } from "@mui/icons-material";
import {
  Container,
  Paper,
  Box,
  Typography,
  TextField,
  Button,
  IconButton,
  InputAdornment,
  useTheme} from "@mui/material";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import {
  loginSchema,
  type LoginSchema,
} from "../../app/lib/schemas/loginSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { useLazyUserInfoQuery, useLoginMutation } from "./accountApi";
import { useState } from "react";
import AppLoading from "../../app/shared/AppLoading";


export default function LoginForm() {
  const theme = useTheme();
  const [login, { isLoading }] = useLoginMutation();
  const [fetchUserInfo] = useLazyUserInfoQuery()
  const location = useLocation();
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginSchema>({
    mode: "onTouched",
    resolver: zodResolver(loginSchema),
  });

  const navigate = useNavigate();

  const onSubmit = async (data: LoginSchema) => {
    await login(data);
    await fetchUserInfo();
    navigate(location.state?.from || "/catalog");
  };

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        p: 2,
      }}
    >
      <Container maxWidth="xs">
        <Paper
          elevation={3}
          sx={{
            p: 4,
            borderRadius: 3,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            gap: 2,
          }}
        >
          <LockOutlined
            sx={{
              fontSize: 40,
              color: theme.palette.primary.main,
              mb: 1,
            }}
          />
          <Typography variant="h5" fontWeight="bold">
            Welcome
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Please enter your credentials
          </Typography>

          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            width="100%"
            display="flex"
            flexDirection="column"
            gap={2}
            mt={2}
          >
            <TextField
              fullWidth
              label="Email"
              {...register("email")}
              error={!!errors.email}
              helperText={errors.email?.message}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <EmailOutlined color="action" />
                  </InputAdornment>
                ),
              }}
            />
            <TextField
              fullWidth
              label="Password"
              type={showPassword ? "text" : "password"}
              {...register("password")}
              error={!!errors.password}
              helperText={errors.password?.message}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowPassword((prev) => !prev)}
                      edge="end"
                      aria-label="toggle password visibility"
                    >
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
            <Button
              type="submit"
              variant="contained"
              disabled={isLoading}
              fullWidth
              sx={{ fontWeight: "bold", py: 1 }}
            >
              Sign in
            </Button>
          </Box>

          <Typography variant="body2" sx={{ mt: 2 }}>
            Don’t have an account?
            <Link
              to="/register"
              style={{ marginLeft: 6, color: theme.palette.primary.main }}
            >
              Sign up
            </Link>
          </Typography>
        </Paper>
      </Container>
      {isLoading && <AppLoading />}
    </Box>
  );
}
