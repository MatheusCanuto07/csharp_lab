import { fail } from '@sveltejs/kit';
import type { CartItem, Region } from '#lib/types';

const REGIONS: Region[] = [
	'Southeast',
	'South',
	'Northeast',
	'North',
	'Midwest',
	'FreeTradeZone'
];

export function parseCartForm(data: FormData) {
	const label = String(data.get('label') ?? '').trim();
	const region = String(data.get('region') ?? '') as Region;
	const itemName = String(data.get('itemName') ?? '').trim();
	const unitPrice = Number(data.get('unitPrice'));
	const quantity = Number(data.get('quantity'));
	const isTaxExempt = data.get('isTaxExempt') === 'on';

	const errors: Record<string, string> = {};

	if (!label) errors.label = 'Informe um nome para o carrinho.';
	if (!REGIONS.includes(region)) errors.region = 'Selecione uma região válida.';
	if (!itemName) errors.itemName = 'Informe o item.';
	if (!Number.isFinite(unitPrice) || unitPrice < 0) errors.unitPrice = 'Preço inválido.';
	if (!Number.isInteger(quantity) || quantity <= 0) errors.quantity = 'Quantidade inválida.';

	if (Object.keys(errors).length > 0) {
		return {
			ok: false as const,
			failure: fail(400, {
				errors,
				values: { label, region, itemName, unitPrice, quantity, isTaxExempt }
			})
		};
	}

	const items: CartItem[] = [
		{
			name: itemName,
			unitPrice,
			quantity,
			isTaxExempt
		}
	];

	return {
		ok: true as const,
		label,
		region,
		items
	};
}
