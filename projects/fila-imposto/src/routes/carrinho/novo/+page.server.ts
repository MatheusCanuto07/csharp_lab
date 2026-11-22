import { redirect } from '@sveltejs/kit';
import { saveCart } from '#lib/server/cartStore';
import { parseCartForm } from '#lib/server/parseCartForm';
import { publishCartToTaxQueue } from '#lib/server/rabbit';
import type { Actions } from './$types';

export const actions: Actions = {
	criar: async ({ request }) => {
		const data = await request.formData();
		const parsed = parseCartForm(data);
		if (!parsed.ok) return parsed.failure;

		const cart = saveCart({
			id: crypto.randomUUID(),
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
				error: `Carrinho salvo localmente, mas RabbitMQ falhou: ${message}`
			};
		}

		redirect(303, `/?queued=${cart.id}`);
	}
};
