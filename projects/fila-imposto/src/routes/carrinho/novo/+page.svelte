<script lang="ts">
	import CarrinhoForm from '#lib/components/form/CarrinhoForm.svelte';
	import type { PageProps } from './$types';

	let { form }: PageProps = $props();
</script>

<nav class="breadcrumbs text-sm mb-4">
	<ul>
		<li><a href="/">Carrinhos</a></li>
		<li>Adicionar</li>
	</ul>
</nav>

<h1 class="text-2xl font-semibold mb-4">Adicionar carrinho</h1>

<CarrinhoForm entidade={null} {form} />
