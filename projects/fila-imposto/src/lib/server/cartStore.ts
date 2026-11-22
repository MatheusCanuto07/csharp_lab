import type { CartRecord } from '#lib/types';

/** In-memory store for the lab UI (no database). */
const carts = new Map<string, CartRecord>();

export function listCarts(): CartRecord[] {
	return [...carts.values()].sort((a, b) => b.updatedAt.localeCompare(a.updatedAt));
}

export function getCart(id: string): CartRecord | undefined {
	return carts.get(id);
}

export function saveCart(cart: CartRecord): CartRecord {
	carts.set(cart.id, cart);
	return cart;
}
