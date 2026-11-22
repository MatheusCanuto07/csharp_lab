<script lang="ts">
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let { carts } = $derived(data);
</script>

<div class="flex flex-wrap gap-y-3" style="align-items: flex-end;">
	<div class="w-full md:w-9/12">
		<h1 class="text-2xl font-semibold">Carrinhos</h1>
		<p class="text-sm text-base-content/70">
			Adicionar ou editar envia o payload para a fila <code class="text-xs">tax.calculate</code>.
		</p>
	</div>
	<div class="w-full md:w-3/12">
		<a class="btn btn-success w-full" href="/carrinho/novo">Adicionar</a>
	</div>
</div>

<div class="mt-5 overflow-x-auto rounded-xl border border-base-300">
	<table class="table w-full min-w-[720px] bg-white">
		<thead>
			<tr>
				<th>Nome</th>
				<th>Região</th>
				<th>Item</th>
				<th>Subtotal item</th>
				<th>Atualizado</th>
				<th class="w-24">Ações</th>
			</tr>
		</thead>
		<tbody>
			{#if carts.length === 0}
				<tr>
					<td colspan="6" class="py-8 text-center">Nenhum carrinho encontrado.</td>
				</tr>
			{:else}
				{#each carts as cart (cart.id)}
					{@const item = cart.items[0]}
					<tr>
						<td>
							<div class="font-medium">{cart.label}</div>
							<div class="text-xs text-base-content/60">{cart.id}</div>
						</td>
						<td><span class="badge badge-ghost">{cart.region}</span></td>
						<td>
							{#if item}
								{item.name}
								{#if item.isTaxExempt}
									<span class="badge badge-info badge-sm ml-1">isento</span>
								{/if}
							{:else}
								—
							{/if}
						</td>
						<td>
							{#if item}
								{(item.unitPrice * item.quantity).toLocaleString('pt-BR', {
									style: 'currency',
									currency: 'BRL'
								})}
							{:else}
								—
							{/if}
						</td>
						<td class="text-sm">{new Date(cart.updatedAt).toLocaleString('pt-BR')}</td>
						<td>
							<a class="btn btn-sm btn-success" href="/carrinho/{cart.id}">Editar</a>
						</td>
					</tr>
				{/each}
			{/if}
		</tbody>
	</table>
</div>
