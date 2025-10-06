# 🏪 JorStock - Sistema de Gestión de Inventario

JorStock es una aplicación de escritorio desarrollada en C# que proporciona un sistema completo de gestión de inventario de autopartes con autenticación segura y funcionalidades avanzadas de búsqueda y organización.

## ✨ Características Principales

### 🔐 **Sistema de Autenticación Seguro**

- Inicio de sesión con usuario y contraseña
- Sistema de recuperación de contraseña mediante correo electrónico
- Verificación de código de seguridad
- Interfaz de cambio de contraseña

### 📦 **Gestión Completa de Inventario**

- **CRUD de Productos**: Crear, leer, actualizar y eliminar autopartes
- **Gestión de Proveedores**: Creación automática y vinculación de proveedores
- **Control de Stock**: Seguimiento de inventario en tiempo real
- **Sistema de Seriales**: Identificación única de productos
- **Fechas de Ingreso**: Registro temporal de productos

### 🔍 **Búsqueda y Filtrado Avanzado**

- Búsqueda por nombre de producto
- Búsqueda por proveedor
- Búsqueda combinada (producto + proveedor)
- Filtrado en tiempo real

### 📊 **Organización y Visualización**

- **Ordenamiento Múltiple**: Por nombre, stock, precio, fecha
- **Agrupación por Proveedor**: Visualización organizada por proveedor
- **Colores Alternados**: Distinción visual entre grupos de proveedores
- **DataGridView Interactivo**: Selección y edición directa

### 🎨 **Interfaz de Usuario Moderna**

- Diseño limpio y profesional
- Ventanas con bordes redondeados
- Sistema de placeholders inteligente
- Navegación intuitiva entre formularios
- Botones de control personalizados

## 🛠️ Tecnologías Utilizadas

- **Lenguaje:** C# (.NET Framework)
- **Framework UI:** Windows Forms
- **Base de Datos:** MongoDB
- **Paquetes Principales:**
  - `MongoDB.Driver` - Driver oficial de MongoDB
  - `MongoDB.Bson` - Serialización BSON
  - `Google.Apis.Gmail.v1` - API de Gmail para envío de correos
  - `Google.Apis.Auth` - Autenticación con Google
  - `Newtonsoft.Json` - Serialización JSON
  - `System.Net.Mail` - Envío de correos electrónicos

## 📋 Requisitos del Sistema

- **Sistema Operativo:** Windows 10/11
- **Framework:** .NET Framework 4.7.2 o superior
- **Base de Datos:** MongoDB Server 4.0+
- **Memoria:** Mínimo 4GB RAM
- **Conexión:** Internet (para recuperación de contraseña)

## 🗂️ Estructura del Proyecto

```
JorStock/
├── 📁 bin/Debug/           # Archivos compilados
├── 📁 Properties/          # Configuraciones del proyecto
├── 📄 Login.cs             # Formulario de inicio de sesión
├── 📄 ForgotPass.cs        # Recuperación de contraseña
├── 📄 FormSecurity.cs      # Verificación de código de seguridad
├── 📄 FormNewPass.cs       # Cambio de contraseña
├── 📄 Home.cs              # 🏠 Interfaz principal del sistema
├── 📄 Program.cs           # Punto de entrada de la aplicación
└── 📄 JorStock.csproj     # Archivo de proyecto
```

### 📋 Formularios Principales

| Formulario          | Descripción                | Funcionalidades                               |
| ------------------- | -------------------------- | --------------------------------------------- |
| **Login.cs**        | Autenticación de usuarios  | Validación de credenciales, navegación segura |
| **ForgotPass.cs**   | Recuperación de contraseña | Envío de códigos por email                    |
| **FormSecurity.cs** | Verificación de seguridad  | Validación de códigos de recuperación         |
| **FormNewPass.cs**  | Cambio de contraseña       | Actualización segura de credenciales          |
| **Home.cs**         | 🏠 Panel principal         | Gestión completa de inventario                |

## 🔒 Seguridad

- **Autenticación Robusta**: Validación contra MongoDB
- **Recuperación Segura**: Sistema de códigos por email
- **Validación de Entradas**: Sanitización de datos de usuario
- **Manejo de Sesiones**: Control de acceso a funcionalidades
- **Conexión Segura**: Comunicación encriptada con base de datos

## ⚙️ Configuración e Instalación

### 1. **Requisitos Previos**

```bash
# Instalar MongoDB
# Descargar desde: https://www.mongodb.com/try/download/community
# Iniciar servicio: mongod --dbpath "ruta_a_tu_directorio_de_datos"
```

### 2. **Configuración de Base de Datos**

```javascript
// Crear base de datos JorStock
use JorStock

// Crear colección de usuarios
db.createCollection("users")

// Crear colección de productos
db.createCollection("productos")

// Crear colección de proveedores
db.createCollection("proveedores")
```

### 3. **Estructura de Datos**

#### 👤 Colección `users`

```json
{
  "_id": ObjectId,
  "username": "string",
  "password": "string_hasheado",
  "email": "string"
}
```

#### 📦 Colección `productos`

```json
{
  "_id": ObjectId,
  "nombre": "string",
  "precio_unitario": "decimal",
  "stock": "int",
  "fecha": "datetime",
  "serial": "string",
  "codigo_proveedor": "string"
}
```

#### 🏢 Colección `proveedores`

```json
{
  "_id": ObjectId,
  "supplierId": "string",
  "name": "string",
  "contactName": "string",
  "email": "string",
  "phone": "string",
  "address": {
    "street": "string",
    "city": "string",
    "state": "string",
    "zipCode": "string"
  },
  "category": "string",
  "status": "string"
}
```

## 🚀 Uso del Sistema

### **Flujo de Trabajo Principal**

1. **🔐 Autenticación**

   - Iniciar sesión con credenciales
   - Recuperar contraseña si es necesario

2. **📦 Gestión de Productos**

   - Agregar nuevos productos
   - Editar productos existentes
   - Eliminar productos
   - Buscar y filtrar productos

3. **🏢 Gestión de Proveedores**

   - Los proveedores se crean automáticamente
   - Vinculación automática con productos

4. **📊 Organización**
   - Ordenar por diferentes criterios
   - Visualizar por grupos de proveedores

## 🎯 Funcionalidades Avanzadas

### **Sistema de Placeholders Inteligente**

- Guías visuales en campos de entrada
- Validación automática de datos
- Limpieza automática de campos

### **Búsqueda en Tiempo Real**

- Filtrado instantáneo mientras escribes
- Búsqueda por múltiples criterios
- Resultados organizados y visuales

### **Gestión Automática de Proveedores**

- Creación automática de proveedores
- Generación de IDs únicos
- Vinculación inteligente con productos

## 🛠️ Desarrollo

Este proyecto fue desarrollado en **Visual Studio** utilizando:

- **C#** y **.NET Framework**
- **Windows Forms** para la interfaz
- **MongoDB** como base de datos
- **Arquitectura MVC** en formularios

### **Características de Código**

- ✅ Código completamente documentado
- ✅ Manejo de errores robusto
- ✅ Operaciones asíncronas
- ✅ Validación de datos
- ✅ Interfaz moderna y responsive

---

## 🚀 Instalación y Configuración

### **Paso 1: Clonar el Repositorio**

```bash
git clone https://github.com/tu-usuario/jorstock-app.git
cd jorstock-app
```

### **Paso 2: Configurar MongoDB Atlas**

1. Crear cuenta en [MongoDB Atlas](https://cloud.mongodb.com)
2. Crear cluster gratuito M0
3. Configurar acceso de red (0.0.0.0/0)
4. Crear usuario de base de datos
5. Obtener string de conexión

### **Paso 3: Actualizar Conexión**

```csharp
// En Home.cs, reemplazar todas las conexiones con tu string de Atlas:
mongodb+srv://usuario:password@cluster.mongodb.net/?retryWrites=true&w=majority
```

### **Paso 4: Compilar y Ejecutar**

```bash
# En Visual Studio
Build -> Build Solution
F5 para ejecutar

# O desde línea de comandos
dotnet build
dotnet run
```

## 📊 Guía de Usuario

### **🔐 Primer Uso**

1. **Iniciar Sesión**: Usa las credenciales por defecto

   - Usuario: `admin`
   - Contraseña: `admin123`

2. **Insertar Datos de Ejemplo**:
   - Ejecuta el script `insert_atlas_data.js` en MongoDB Atlas
   - O agrega productos manualmente desde la interfaz

### **📦 Gestión de Productos**

#### **Agregar Producto**

1. Completa los campos: Nombre, Precio, Stock, Proveedor, Código
2. Click "Guardar"
3. El producto se guarda automáticamente en Atlas

#### **Editar Producto**

1. Selecciona el producto en la tabla
2. Click "Editar"
3. Modifica los campos necesarios
4. Click "Guardar"

#### **Eliminar Producto**

1. Selecciona el producto en la tabla
2. Click "Borrar"
3. Confirma la eliminación

#### **Buscar Productos**

1. Usa los campos de búsqueda por nombre o proveedor
2. Los resultados se filtran automáticamente
3. Click "Limpiar" para ver todos los productos

### **📊 Organización**

- **Ordenar**: Click en el botón de ordenamiento y selecciona criterio
- **Agrupación**: Los productos se agrupan automáticamente por proveedor
- **Colores**: Cada grupo de proveedor tiene un color distintivo

## 🔧 Solución de Problemas

### **❌ Error de Conexión a MongoDB**

```
Solución: Verificar string de conexión en Home.cs
```

1. Revisar que el string de Atlas sea correcto
2. Verificar que el cluster esté activo
3. Comprobar acceso de red (0.0.0.0/0)

### **❌ Productos Duplicados**

```
Solución: Ya corregido en la versión actual
```

- Los eventos duplicados fueron eliminados
- Se agregó control de llamadas duplicadas

### **❌ Error al Guardar**

```
Solución: Verificar campos requeridos
```

1. Asegurar que todos los campos estén completos
2. Verificar formato de precio (decimal)
3. Verificar formato de stock (entero)

### **❌ Búsqueda No Funciona**

```
Solución: Limpiar campos de búsqueda
```

1. Click "Limpiar" en la sección de búsqueda
2. Verificar que los campos no tengan placeholders

## 📈 Características Técnicas

### **🏗️ Arquitectura**

- **Patrón**: Modelo-Vista-Controlador (MVC)
- **Base de Datos**: MongoDB Atlas (Cloud)
- **Conexión**: Asíncrona con manejo de errores
- **UI**: Windows Forms con diseño moderno

### **🔒 Seguridad Implementada**

- Validación de entrada en todos los campos
- Sanitización de datos antes de guardar
- Conexión segura con MongoDB Atlas
- Manejo de excepciones robusto

### **⚡ Optimizaciones**

- Operaciones asíncronas para mejor rendimiento
- Control de llamadas duplicadas
- Carga eficiente de datos
- Interfaz responsiva

## 🎯 Roadmap Futuro

### **🔄 Versión 2.0 (Próximas Características)**

- [ ] **Reportes PDF**: Generación de reportes de inventario
- [ ] **Backup Automático**: Respaldos programados
- [ ] **Notificaciones**: Alertas de stock bajo
- [ ] **API REST**: Integración con sistemas externos
- [ ] **Dashboard**: Estadísticas y métricas

### **🚀 Versión 3.0 (Características Avanzadas)**

- [ ] **Multi-usuario**: Gestión de roles y permisos
- [ ] **Auditoría**: Registro de cambios
- [ ] **Integración**: APIs de proveedores
- [ ] **Mobile**: Aplicación móvil complementaria

## 📚 Documentación Adicional

### **📖 Guías de Desarrollo**

- [Configuración de MongoDB Atlas](docs/atlas-setup.md)
- [Estructura de Base de Datos](docs/database-schema.md)
- [Guía de Contribución](docs/contributing.md)

### **🔗 Enlaces Útiles**

- [MongoDB Atlas](https://cloud.mongodb.com)
- [.NET Framework](https://dotnet.microsoft.com/)
- [Windows Forms](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)

## 🤝 Contribuir

### **🐛 Reportar Bugs**

1. Ve a la sección "Issues" del repositorio
2. Describe el problema detalladamente
3. Incluye pasos para reproducir el error
4. Adjunta capturas de pantalla si es necesario

### **💡 Sugerir Mejoras**

1. Crea un "Feature Request"
2. Describe la funcionalidad deseada
3. Explica el beneficio para los usuarios
4. Proporciona ejemplos de uso

### **🔧 Contribuir al Código**

1. Fork del repositorio
2. Crea una rama para tu feature
3. Realiza los cambios
4. Envía un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 👥 Equipo de Desarrollo

- **Desarrollador Principal**: [Tu Nombre]
- **Base de Datos**: MongoDB Atlas
- **Framework**: .NET Framework 4.7.2+
- **IDE**: Visual Studio 2019/2022

## 📞 Soporte y Contacto

### **🆘 Soporte Técnico**

- **Email**: soporte@jorstock.com
- **GitHub Issues**: [Reportar Problema](https://github.com/tu-usuario/jorstock-app/issues)
- **Documentación**: [Wiki del Proyecto](https://github.com/tu-usuario/jorstock-app/wiki)

### **💬 Comunidad**

- **Discord**: [Servidor de JorStock](https://discord.gg/jorstock)
- **Reddit**: [r/JorStock](https://reddit.com/r/JorStock)
- **Twitter**: [@JorStockApp](https://twitter.com/JorStockApp)

---

## 🎉 ¡Gracias por usar JorStock!

**Desarrollado con ❤️ utilizando tecnologías Microsoft .NET**

_Si este proyecto te ha sido útil, considera darle una ⭐ en GitHub_
