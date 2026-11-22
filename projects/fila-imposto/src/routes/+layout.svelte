<script lang="ts">
	import '../app.css';
	import favicon from '#lib/assets/favicon.svg';
	import type { LayoutProps } from './$types';

	let { children }: LayoutProps = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
	<title>Fila Imposto</title>
</svelte:head>

<div class="min-h-screen bg-base-100">
	<div class="navbar bg-white shadow-sm border-b border-base-300">
		<div class="flex-1 px-4">
			<a href="/" class="text-xl font-semibold text-base-content">Fila Imposto</a>
			<span class="ml-3 text-sm text-base-content/60">lab RabbitMQ · calcular imposto</span>
		</div>
	</div>

	<main class="mx-auto max-w-6xl p-6">
		{@render children()}
	</main>
</div>
