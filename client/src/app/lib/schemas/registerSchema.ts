import z from "zod";

const passwordRegex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).{8,}$/;


export const registerSchema = z.object({
  email: z.string().email(),
  password: z.string().regex(passwordRegex, {
    message:
      "Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a number, and one special character (@, #, $, %, &).",
  }),
});

export type RegisterSchema = z.infer<typeof registerSchema>;