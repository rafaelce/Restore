import { Button, Container, Typography, Stack, Alert, AlertTitle, ListItem } from "@mui/material";
import {
  useLazyGet400ErrorQuery,
  useLazyGet401ErrorQuery,
  useLazyGet404ErrorQuery,
  useLazyGet500ErrorQuery,
  useLazyGetValidationErrorQuery,
} from "./errorApi";
import { useState } from "react";

export default function AboutPage() {
  const [trigger400Error] = useLazyGet400ErrorQuery();
  const [trigger401Error] = useLazyGet401ErrorQuery();
  const [trigger404Error] = useLazyGet404ErrorQuery();
  const [trigger500Error] = useLazyGet500ErrorQuery();
  const [triggerValidationError] = useLazyGetValidationErrorQuery();

  const [validationErrors, setValidationErrors] = useState<string[]>([]);

  const getValidationError = async () => {
    try {
      await triggerValidationError().unwrap();
    } catch (error: unknown) {
      if(error && 
        typeof error === "object" && 
        "message" in error && 
        typeof (error as {message: unknown}).message === "string") {

          const errorArray = (error as {message: string}).message.split(", ");
          setValidationErrors(errorArray);
      }
      
    }
  };

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" align="center" gutterBottom color="primary">
        Test API Error Responses
      </Typography>

      <Stack spacing={2}>
        <Button
          variant="contained"
          color="warning"
          onClick={() => trigger400Error().catch((err) => console.log(err))}
        >
          400 Bad Request
        </Button>
        <Button
          variant="contained"
          color="secondary"
          onClick={() => trigger401Error().catch((err) => console.log(err))}
        >
          401 Unauthorized
        </Button>
        <Button
          variant="contained"
          color="info"
          onClick={() => trigger404Error().catch((err) => console.log(err))}
        >
          404 Not Found
        </Button>
        <Button
          variant="contained"
          color="error"
          onClick={() => trigger500Error().catch((err) => console.log(err))}
        >
          500 Internal Server Error
        </Button>
        <Button
          variant="contained"
          color="success"
          onClick={getValidationError}
        >
          Validation Error
        </Button>
      </Stack>
      {validationErrors.length > 0 && (
        <Alert severity="error">
          <AlertTitle>Validation Errors:</AlertTitle>
          {validationErrors.map((error, index) => (
            <ListItem key={index} color="error">{error}</ListItem>
          ))}
        </Alert>
      )}

    </Container>
  );
}
