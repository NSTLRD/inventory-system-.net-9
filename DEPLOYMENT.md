# Guía de Despliegue del Sistema de Inventario y Productos

Esta guía proporciona instrucciones detalladas para desplegar el sistema de microservicios de inventario y productos en diferentes entornos.

## Requisitos Previos

- [Docker](https://www.docker.com/get-started) y [Docker Compose](https://docs.docker.com/compose/install/) instalados
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) para desarrollo local
- [Git](https://git-scm.com/downloads) para control de versiones

## Despliegue Local con Docker Compose

1. **Clonar el repositorio**:
   ```bash
   git clone <url-del-repositorio>
   cd ms-inventory-sistem
   ```

2. **Construir y levantar los contenedores**:
   ```bash
   docker-compose build
   docker-compose up -d
   ```

3. **Verificar que los servicios están funcionando**:
   ```bash
   docker-compose ps
   ```

4. **Acceder a los servicios**:
   - Microservicio de Productos: http://localhost:5001
   - Microservicio de Inventario: http://localhost:5002

## Estructura del Docker Compose

El archivo `docker-compose.yml` incluye los siguientes servicios:

- **ms-products**: Microservicio de gestión de productos
- **ms-inventory**: Microservicio de gestión de inventario
- **sql-server**: Base de datos SQL Server para ambos microservicios
- **redis**: Cache Redis para mejorar el rendimiento
- **rabbitmq**: Message broker para la comunicación entre microservicios

## Variables de Entorno

Las variables de entorno están configuradas en el archivo `docker-compose.yml` y pueden ser modificadas según las necesidades del entorno:

### ms-products
- `ConnectionStrings__ProductsDatabase`: Cadena de conexión a la base de datos de productos
- `RabbitMQ__Host`: Host del servidor RabbitMQ
- `RabbitMQ__UserName`: Usuario para RabbitMQ
- `RabbitMQ__Password`: Contraseña para RabbitMQ

### ms-inventory
- `ConnectionStrings__InventoryDatabase`: Cadena de conexión a la base de datos de inventario
- `RabbitMQ__Host`: Host del servidor RabbitMQ
- `RabbitMQ__UserName`: Usuario para RabbitMQ
- `RabbitMQ__Password`: Contraseña para RabbitMQ

## Migración de Base de Datos

Las migraciones se ejecutan automáticamente al iniciar los microservicios. Si necesitas ejecutarlas manualmente:

```bash
# Para ms-products
cd ms-products
dotnet ef database update

# Para ms-inventory
cd ms-inventory
dotnet ef database update
```

## Monitoreo y Observabilidad

### Logs

Los logs de los contenedores pueden visualizarse con:

```bash
# Logs de ms-products
docker-compose logs ms-products

# Logs de ms-inventory
docker-compose logs ms-inventory
```


## Contacto y Soporte

Starling Diaz
Linkedin: [https://www.linkedin.com/in/starling-diaz-908225181]
