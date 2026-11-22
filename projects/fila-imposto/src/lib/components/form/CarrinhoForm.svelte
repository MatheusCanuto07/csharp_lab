<script lang="ts">
	import { enhance } from '$app/forms';
	import type { CartRecord, Region } from '#lib/types';
	import type { SubmitFunction } from '@sveltejs/kit';

	type FormValues = {
		label?: string;
		region?: Region;
		itemName?: string;
		unitPrice?: number;
		quantity?: number;
		isTaxExempt?: boolean;
	};

	type Props = {
		entidade: CartRecord | null;
		form?: {
			errors?: Record<string, string>;
			values?: FormValues;
			queued?: boolean;
			error?: string;
		} | null;
	};

	let { entidade, form = null }: Props = $props();

	const isEdit = $derived(entidade !== null);
	let isLoading = $state(false);

	const seedItem = entidade?.items[0];
	const seed = form?.values;

	let label = $state(seed?.label ?? entidade?.label ?? '');
	let region = $state<Region>(seed?.region ?? entidade?.region ?? 'Midwest');
	let itemName = $state(seed?.itemName ?? seedItem?.name ?? '');
	let unitPrice = $state(seed?.unitPrice ?? seedItem?.unitPrice ?? 100);
	let quantity = $state(seed?.quantity ?? seedItem?.quantity ?? 1);
	let isTaxExempt = $state(seed?.isTaxExempt ?? seedItem?.isTaxExempt ?? false);

	const regions: { value: Region; label: string }[] = [
		{ value: 'Southeast', label: 'Sudeste (18%)' },
		{ value: 'South', label: 'Sul (17%)' },
		{ value: 'Northeast', label: 'Nordeste (12%)' },
		{ value: 'North', label: 'Norte (10%)' },
		{ value: 'Midwest', label: 'Centro-Oeste (15%)' },
		{ value: 'FreeTradeZone', label: 'Zona franca (0%)' }
	];

	const onEnhance: SubmitFunction = () => {
		isLoading = true;
		return async ({ update }) => {
			await update();
			isLoading = false;
		};
	};
</script>

{#if form?.queued}
	<div class="alert alert-success mb-4">
		<span>Salvo e enviado para a fila RabbitMQ (`tax.calculate`). O consumer TaxQueue calcula o imposto.</span>
	</div>
{/if}

{#if form?.error}
	<div class="alert alert-error mb-4">
		<span>{form.error}</span>
	</div>
{/if}

<form
	method="POST"
	action={isEdit ? '?/salvar' : '?/criar'}
	use:enhance={onEnhance}
	class="bg-white p-6 rounded-xl shadow-md space-y-4"
>
	<label class="form-control w-full">
		<span class="label-text mb-1">Nome do carrinho <span class="text-error">*</span></span>
		<input
			class="input input-bordered w-full {form?.errors?.label ? 'input-error' : ''}"
			name="label"
			bind:value={label}
			required
		/>
		{#if form?.errors?.label}
			<span class="mt-1 text-sm text-error">{form.errors.label}</span>
		{/if}
	</label>

	<label class="form-control w-full">
		<span class="label-text mb-1">Região (alíquota) <span class="text-error">*</span></span>
		<select
			class="select select-bordered w-full {form?.errors?.region ? 'input-error' : ''}"
			name="region"
			bind:value={region}
			required
		>
			{#each regions as r (r.value)}
				<option value={r.value}>{r.label}</option>
			{/each}
		</select>
		{#if form?.errors?.region}
			<span class="mt-1 text-sm text-error">{form.errors.region}</span>
		{/if}
	</label>

	<div class="divider">Item (enviado à fila)</div>

	<label class="form-control w-full">
		<span class="label-text mb-1">Produto <span class="text-error">*</span></span>
		<input
			class="input input-bordered w-full {form?.errors?.itemName ? 'input-error' : ''}"
			name="itemName"
			bind:value={itemName}
			required
		/>
		{#if form?.errors?.itemName}
			<span class="mt-1 text-sm text-error">{form.errors.itemName}</span>
		{/if}
	</label>

	<div class="flex flex-wrap gap-3">
		<label class="form-control md:w-4/12">
			<span class="label-text mb-1">Preço unitário</span>
			<input
				class="input input-bordered w-full {form?.errors?.unitPrice ? 'input-error' : ''}"
				type="number"
				step="0.01"
				min="0"
				name="unitPrice"
				bind:value={unitPrice}
				required
			/>
			{#if form?.errors?.unitPrice}
				<span class="mt-1 text-sm text-error">{form.errors.unitPrice}</span>
			{/if}
		</label>

		<label class="form-control md:w-4/12">
			<span class="label-text mb-1">Quantidade</span>
			<input
				class="input input-bordered w-full {form?.errors?.quantity ? 'input-error' : ''}"
				type="number"
				min="1"
				step="1"
				name="quantity"
				bind:value={quantity}
				required
			/>
			{#if form?.errors?.quantity}
				<span class="mt-1 text-sm text-error">{form.errors.quantity}</span>
			{/if}
		</label>

		<label class="form-control md:w-3/12 justify-end">
			<span class="label-text mb-1">Isento de imposto</span>
			<input
				class="checkbox checkbox-success checkbox-lg"
				type="checkbox"
				name="isTaxExempt"
				bind:checked={isTaxExempt}
			/>
		</label>
	</div>

	<div class="mt-6 flex w-full justify-end gap-3">
		<a class="btn btn-ghost border border-base-300" href="/">Cancelar</a>
		<button class="btn btn-success" disabled={isLoading}>
			{isLoading ? 'Enviando...' : isEdit ? 'Salvar e enfileirar' : 'Adicionar e enfileirar'}
		</button>
	</div>
</form>
