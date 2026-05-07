# RabbitMQ Producer/Consumer Demo

Project ini menyediakan contoh sederhana `Producer` dan `Consumer` menggunakan RabbitMQ dengan dukungan DLQ (Dead Letter Queue).

## Struktur

- `RabbitMqDemo.Shared`: shared helper untuk konfigurasi RabbitMQ dan deklarasi topologi.
- `Producer`: aplikasi console yang mem-publish pesan ke queue utama.
- `Consumer`: aplikasi console yang mengkonsumsi pesan dari queue utama atau DLQ.

## Cara pakai

1. Pastikan RabbitMQ berjalan di `localhost:5672` dengan user `guest/guest`.
2. Jalankan producer:
   ```powershell
   dotnet run --project rabbitmq-demo\Producer\Producer.csproj -- 10
   ```
   Aplikasi akan mem-publish 10 pesan dan setiap pesan kelima akan ditandai sebagai `reject`.

3. Jalankan consumer normal:
   ```powershell
   dotnet run --project rabbitmq-demo\Consumer\Consumer.csproj
   ```
   Pesan yang berisi kata `reject` akan ditolak dan dikirim ke DLQ.

4. Jalankan consumer DLQ untuk membaca pesan yang sudah dead-lettered:
   ```powershell
   dotnet run --project rabbitmq-demo\Consumer\Consumer.csproj --dlq
   ```

## Konfigurasi

Anda dapat mengubah koneksi RabbitMQ lewat environment variable:
- `RABBITMQ_HOST`
- `RABBITMQ_PORT`
- `RABBITMQ_USERNAME`
- `RABBITMQ_PASSWORD`
- `RABBITMQ_VHOST`

Jika tidak diset, nilai default adalah `localhost:5672`, `guest/guest`, dan `/`.
