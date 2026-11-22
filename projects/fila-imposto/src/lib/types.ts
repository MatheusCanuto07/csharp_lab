/** Payload aligned with SalesSystem.Models.ShoppingCart JSON (camelCase). */
export type Region =
	| 'Southeast'
	| 'South'
	| 'Northeast'
	| 'North'
	| 'Midwest'
	| 'FreeTradeZone';

export type CartItem = {
	name: string;
	unitPrice: number;
	quantity: number;
	isTaxExempt: boolean;
};

export type CartRecord = {
	id: string;
	label: string;
	region: Region;
	items: CartItem[];
	updatedAt: string;
};

export type QueueCartPayload = {
	region: Region;
	items: CartItem[];
};
