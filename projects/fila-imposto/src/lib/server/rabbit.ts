import amqp from 'amqplib';
import { env } from '$env/dynamic/private';
import type { QueueCartPayload } from '#lib/types';

/**
 * Publishes a shopping cart to the same queue TaxQueue consumes (`tax.calculate`).
 * Browser never talks to RabbitMQ — only this server module does.
 */
export async function publishCartToTaxQueue(payload: QueueCartPayload): Promise<void> {
	const url = env.RABBITMQ_URL ?? 'amqp://guest:guest@localhost:5672';
	const queue = env.RABBITMQ_QUEUE ?? 'tax.calculate';

	const connection = await amqp.connect(url);
	try {
		const channel = await connection.createChannel();
		await channel.assertQueue(queue, { durable: true });

		const body = Buffer.from(JSON.stringify(payload));
		channel.sendToQueue(queue, body, {
			contentType: 'application/json',
			persistent: true
		});

		await channel.close();
	} finally {
		await connection.close();
	}
}
