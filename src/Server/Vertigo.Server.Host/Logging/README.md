# Límites del logging del servidor

Los destinos Technical y Security escriben archivos JSONL mediante Serilog.

- Un fallo de almacenamiento o una interrupción del proceso puede provocar la pérdida de registros.
- Si se interrumpe una escritura, la última línea del JSONL puede quedar parcial.
- La retención se mide por cantidad de archivos. Technical conserva hasta 30 y Security hasta 60.
- Ambos destinos rotan por día y por tamaño. El umbral de 10 MiB por archivo es aproximado y un evento puede sobrepasarlo.
