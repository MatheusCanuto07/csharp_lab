<script lang="ts">
	import CarrinhoForm from '#lib/components/form/CarrinhoForm.svelte';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
</script>

<nav class="breadcrumbs text-sm mb-4">
	<ul>
		<li><a href="/">Carrinhos</a></li>
		<li>Editar</li>
	</ul>
</nav>

<h1 class="text-2xl font-semibold mb-4">Editar carrinho</h1>

<CarrinhoForm entidade={data.cart} {form} />
