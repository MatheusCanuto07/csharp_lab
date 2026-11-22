import { error, redirect } from '@sveltejs/kit';
import { getCart, saveCart } from '#lib/server/cartStore';
import { parseCartForm } from '#lib/server/parseCartForm';
import { publishCartToTaxQueue } from '#lib/server/rabbit';
import type { Actions, PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ params }) => {
	const cart = getCart(params.id);
	if (!cart) error(404, 'Carrinho não encontrado');
	return { cart };
};

export const actions: Actions = {
	salvar: async ({ request, params }) => {
		const existing = getCart(params.id);
		if (!existing) error(404, 'Carrinho não encontrado');

		const data = await request.formData();
		const parsed = parseCartForm(data);
		if (!parsed.ok) return parsed.failure;

		const cart = saveCart({
			...existing,
			label: parsed.label,
			region: parsed.region,
			items: parsed.items,
			updatedAt: new Date().toISOString()
		});

		try {
			await publishCartToTaxQueue({
				region: cart.region,
				items: cart.items
			});
		} catch (err) {
			const message = err instanceof Error ? err.message : 'Falha ao publicar na fila.';
			return {
				queued: false,
				error: `Carrinho atualizado localmente, mas RabbitMQ falhou: ${message}`
			};
		}

		redirect(303, `/?queued=${cart.id}`);
	}
};
