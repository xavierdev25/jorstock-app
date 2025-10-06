// Script para insertar datos de ejemplo en MongoDB Atlas
// Ejecutar en MongoDB Compass o Atlas Data Explorer

// 1. Crear usuarios de ejemplo
db.users.insertMany([
  {
    username: "admin",
    password: "admin123", // En producción, esto debería estar hasheado
    email: "admin@jorstock.com",
  },
  {
    username: "demo",
    password: "demo123",
    email: "demo@jorstock.com",
  },
]);

// 2. Crear proveedores de ejemplo
db.proveedores.insertMany([
  {
    supplierId: "SUP001",
    name: "Autopartes del Norte",
    contactName: "Juan Pérez",
    email: "contacto@autopartesnorte.com",
    phone: "+1-555-0101",
    address: {
      street: "Av. Principal 123",
      city: "Ciudad Norte",
      state: "Estado Norte",
      zipCode: "12345",
    },
    category: "Repuestos Generales",
    status: "Activo",
  },
  {
    supplierId: "SUP002",
    name: "Repuestos Premium",
    contactName: "María García",
    email: "ventas@repuestospremium.com",
    phone: "+1-555-0102",
    address: {
      street: "Calle Secundaria 456",
      city: "Ciudad Sur",
      state: "Estado Sur",
      zipCode: "54321",
    },
    category: "Repuestos Premium",
    status: "Activo",
  },
]);

// 3. Crear productos de ejemplo
db.productos.insertMany([
  {
    nombre: "Filtro de Aceite Motor",
    precio_unitario: 25.5,
    stock: 50,
    fecha: "2024-01-15 10:30:00",
    serial: "FIL001",
    codigo_proveedor: "SUP001",
  },
  {
    nombre: "Pastillas de Freno Delanteras",
    precio_unitario: 45.75,
    stock: 30,
    fecha: "2024-01-16 14:20:00",
    serial: "PAS001",
    codigo_proveedor: "SUP001",
  },
  {
    nombre: "Bujía de Encendido",
    precio_unitario: 12.25,
    stock: 100,
    fecha: "2024-01-17 09:15:00",
    serial: "BUJ001",
    codigo_proveedor: "SUP002",
  },
  {
    nombre: "Aceite Motor 5W-30",
    precio_unitario: 35.0,
    stock: 75,
    fecha: "2024-01-18 11:45:00",
    serial: "ACE001",
    codigo_proveedor: "SUP002",
  },
  {
    nombre: "Filtro de Aire",
    precio_unitario: 18.9,
    stock: 40,
    fecha: "2024-01-19 16:30:00",
    serial: "AIR001",
    codigo_proveedor: "SUP001",
  },
]);

print("✅ Datos de ejemplo insertados correctamente en MongoDB Atlas!");
print("📊 Usuarios: 2");
print("🏢 Proveedores: 2");
print("📦 Productos: 5");
print("🌐 Base de datos: JorStock");
print("🔗 Cluster: jorstock-cluster.tq7rw02.mongodb.net");
