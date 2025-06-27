import { Grid} from "@mui/material";
import OrderSummary from "../../app/shared/OrderSummary";
import CheckoutStepper from "./CheckoutStepper";
import { loadStripe, type StripeElementsOptions } from "@stripe/stripe-js";
import { useFetchBasketQuery } from "../basket/basketApi";
import { useEffect, useMemo, useRef } from "react";
import { Elements } from "@stripe/react-stripe-js";
import { useCreatePaymentIntentMutation } from "./checkoutApi";
import { useAppSelector } from "../../app/store/store";
import AppLoading from "../../app/shared/AppLoading";

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PK);

export default function CheckoutPage() {
    const {data: basket} = useFetchBasketQuery();
    const [createPaymentIntent, {isLoading}] = useCreatePaymentIntentMutation();
    const created = useRef(false);
    const {darkMode} = useAppSelector(state => state.ui);

    useEffect(() => {
        if (!created.current) createPaymentIntent();
        created.current = true;
    }, [createPaymentIntent]);

    const options: StripeElementsOptions | undefined = useMemo(() => {
      if (!basket?.clientSecret) return undefined;

      return {
        clientSecret: basket.clientSecret,
        appearance: {
          labels: "floating",
          theme: darkMode ? "night": "stripe",
        },
      };
    }, [basket?.clientSecret, darkMode]);

    return (
      <Grid container spacing={2}>
        <Grid size={8}>
          {!stripePromise || !options || isLoading ? (
            <AppLoading text="Loading checkout" />
          ) : (
            <Elements stripe={stripePromise} options={options}>
              <CheckoutStepper />
            </Elements>
          )}
        </Grid>
        <Grid size={4}>
          <OrderSummary />
        </Grid>
      </Grid>
    );
}