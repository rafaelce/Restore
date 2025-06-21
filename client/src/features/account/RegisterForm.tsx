import { zodResolver } from "@hookform/resolvers/zod";
import { registerSchema, type RegisterSchema } from "../../app/lib/schemas/registerSchema";
import { useRegisterMutation } from "./accountApi"
import { useForm } from "react-hook-form";
import { LockOutlined, EmailOutlined, VisibilityOff, Visibility } from "@mui/icons-material";
import { Box, Button, Container, IconButton, InputAdornment, Paper, TextField, Typography, useTheme } from "@mui/material";
import { Link } from "react-router-dom";
import AppLoading from "../../app/shared/AppLoading";
import { useState } from "react";

export default function RegisterForm() {
    const theme = useTheme();
    const [showPassword, setShowPassword] = useState(false);
    const [registerUser] = useRegisterMutation();

    const {register, handleSubmit, setError, formState: {errors, isValid, isLoading}} = useForm<RegisterSchema>({
        mode: "onTouched",
        resolver: zodResolver(registerSchema),
    });

    const onSubmit = async (data: RegisterSchema) => {
        try {
            await registerUser(data).unwrap();
        }catch (error) {
            const apiError = error as { message: string};
            
            if(apiError.message && typeof apiError.message === "string") {
               const errorArray = apiError.message.split(",");

               errorArray.forEach((err) => {
                   if(err.includes("Password")) {
                       setError("password", { message: err });
                   }else if(err.includes("Email")) {
                       setError("email", { message: err });
                   }
                });
            }
        }
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
           Register
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
              disabled={isLoading || !isValid}
              fullWidth
              sx={{ fontWeight: "bold", py: 1 }}
            >
              Register
            </Button>
          </Box>

          <Typography variant="body2" sx={{ mt: 2 }}>
            Already have an account?
            <Link
              to="/login"
              style={{ marginLeft: 6, color: theme.palette.primary.main }}
            >
              Sign in here
            </Link>
          </Typography>
        </Paper>
      </Container>
      {isLoading && <AppLoading />}
    </Box>
  )
}