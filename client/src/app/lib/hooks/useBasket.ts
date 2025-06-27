import { useClearBasketMutation, useFetchBasketQuery } from "../../../features/basket/basketApi";
import type { Item } from "../../models/basket";

export default function useBasket() {
    const { data: basket } = useFetchBasketQuery();
    const [clearBasket] = useClearBasketMutation();
    const subtotal = basket?.items.reduce((sum: number, item: Item) => sum + item.quantity * item.price,  0) ?? 0;
    const deliveryFee = subtotal > 10000 ? 0 : 500;
    const total = subtotal + deliveryFee;
    
  return { basket, subtotal, deliveryFee, total, clearBasket };
}