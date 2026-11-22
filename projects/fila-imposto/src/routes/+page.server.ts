import { listCarts } from '#lib/server/cartStore';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async () => {
	return {
		carts: listCarts()
	};
};
