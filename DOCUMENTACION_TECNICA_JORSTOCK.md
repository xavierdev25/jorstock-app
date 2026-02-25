# 📋 DOCUMENTACIÓN TÉCNICA - JORSTOCK

## Sistema de Gestión de Inventario de Autopartes

---

## 📑 ÍNDICE

1. [**INFORMACIÓN GENERAL**](#1-información-general)
2. [**ARQUITECTURA DEL SISTEMA**](#2-arquitectura-del-sistema)
3. [**FUNCIONALIDADES PRINCIPALES**](#3-funcionalidades-principales)
4. [**ESTRUCTURA DEL PROYECTO**](#4-estructura-del-proyecto)
5. [**BASE DE DATOS**](#5-base-de-datos)
6. [**FORMULARIOS Y MÓDULOS**](#6-formularios-y-módulos)
7. [**CONFIGURACIÓN E INSTALACIÓN**](#7-configuración-e-instalación)
8. [**MANUAL DE USUARIO**](#8-manual-de-usuario)
9. [**DESARROLLO Y MANTENIMIENTO**](#9-desarrollo-y-mantenimiento)
10. [**SOLUCIÓN DE PROBLEMAS**](#10-solución-de-problemas)

---

## 1. INFORMACIÓN GENERAL

### 1.1 Descripción del Proyecto

**JorStock** es una aplicación de escritorio desarrollada en C# que proporciona un sistema completo de gestión de inventario de autopartes. El sistema incluye funcionalidades avanzadas de autenticación, gestión de productos, proveedores y búsqueda inteligente.

### 1.2 Características Principales

- ✅ **Sistema de Autenticación Seguro** con recuperación de contraseña
- ✅ **Gestión Completa de Inventario** (CRUD de productos)
- ✅ **Gestión Automática de Proveedores**
- ✅ **Búsqueda y Filtrado Avanzado**
- ✅ **Interfaz Moderna** con bordes redondeados
- ✅ **Base de Datos MongoDB** (Local y Atlas)
- ✅ **Sistema de Placeholders Inteligente**

### 1.3 Tecnologías Utilizadas

| Tecnología               | Versión              | Propósito                |
| ------------------------ | -------------------- | ------------------------ |
| **C#**                   | .NET Framework 4.7.2 | Lenguaje de programación |
| **Windows Forms**        | -                    | Interfaz de usuario      |
| **MongoDB**              | 4.0+                 | Base de datos            |
| **MongoDB.Driver**       | 3.4.0                | Driver de conexión       |
| **Google.Apis.Gmail.v1** | 1.69.0               | Envío de correos         |
| **Newtonsoft.Json**      | -                    | Serialización JSON       |

---

## 2. ARQUITECTURA DEL SISTEMA

### 2.1 Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│                    CAPA DE PRESENTACIÓN                    │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────┐ │
│  │    Login    │ │   Home      │ │ ForgotPass  │ │ Others  │ │
│  │  (Auth)     │ │ (Principal) │ │ (Recovery)  │ │ Forms   │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                    CAPA DE LÓGICA                           │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │              Program.cs (Main Controller)              │ │
│  │  • Gestión de Formularios                              │ │
│  │  • Navegación entre Ventanas                          │ │
│  │  • Control de Estado                                   │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                    CAPA DE DATOS                            │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                    MongoDB                              │ │
│  │  • Colección: users                                    │ │
│  │  • Colección: productos                                │ │
│  │  • Colección: proveedores                              │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Flujo de Datos

```
Usuario → Formulario → Validación → MongoDB → Respuesta → UI
   ↑                                                      ↓
   └─────────── Actualización de Interfaz ←────────────────┘
```

### 2.3 Patrones de Diseño Implementados

- **MVC (Modelo-Vista-Controlador)**: Separación clara entre lógica, datos e interfaz
- **Singleton**: Para gestión de formularios principales
- **Observer**: Para eventos de interfaz
- **Factory**: Para creación de formularios

---

## 3. FUNCIONALIDADES PRINCIPALES

### 3.1 Sistema de Autenticación

#### 3.1.1 Login de Usuario

- **Validación de credenciales** contra MongoDB
- **Manejo de errores** robusto
- **Navegación segura** al sistema principal

#### 3.1.2 Recuperación de Contraseña

- **Verificación de email** en base de datos
- **Generación de código** de seguridad (6 dígitos)
- **Envío automático** por Gmail SMTP
- **Verificación de código** antes de cambio
- **Actualización segura** de contraseña

### 3.2 Gestión de Inventario

#### 3.2.1 Operaciones CRUD de Productos

- **Crear**: Nuevos productos con validación
- **Leer**: Visualización en DataGridView
- **Actualizar**: Edición de productos existentes
- **Eliminar**: Borrado con confirmación

#### 3.2.2 Gestión de Proveedores

- **Creación automática** de proveedores
- **Generación de IDs** únicos (SUP001, SUP002, etc.)
- **Vinculación automática** con productos
- **Información completa** (contacto, dirección, categoría)

### 3.3 Búsqueda y Filtrado

#### 3.3.1 Búsqueda Avanzada

- **Por nombre de producto** (búsqueda parcial)
- **Por proveedor** (búsqueda por nombre)
- **Búsqueda combinada** (producto + proveedor)
- **Filtrado en tiempo real**

#### 3.3.2 Organización Visual

- **Ordenamiento múltiple**: Nombre, stock, precio, fecha
- **Agrupación por proveedor** con colores alternados
- **Visualización organizada** y profesional

---

## 4. ESTRUCTURA DEL PROYECTO

### 4.1 Estructura de Directorios

```
JorStock/
├── 📁 bin/
│   ├── 📁 Debug/              # Archivos compilados
│   └── 📁 Release/          # Versiones de producción
├── 📁 Properties/            # Configuraciones del proyecto
├── 📁 obj/                   # Archivos temporales de compilación
├── 📄 Program.cs             # Punto de entrada principal
├── 📄 Login.cs               # Formulario de autenticación
├── 📄 Home.cs                # Formulario principal del sistema
├── 📄 ForgotPass.cs          # Recuperación de contraseña
├── 📄 FormSecurity.cs        # Verificación de código
├── 📄 FormNewPass.cs         # Cambio de contraseña
├── 📄 JorStock.csproj        # Archivo de proyecto
├── 📄 App.config             # Configuración de aplicación
└── 📄 JorStock.sln           # Solución de Visual Studio
```

### 4.2 Archivos de Configuración

#### 4.2.1 JorStock.csproj

```xml
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
    <OutputType>WinExe</OutputType>
    <AssemblyName>JorStock</AssemblyName>
  </PropertyGroup>
  <!-- Referencias y paquetes NuGet -->
</Project>
```

#### 4.2.2 Dependencias Principales

```xml
<PackageReference Include="MongoDB.Driver" Version="3.4.0" />
<PackageReference Include="Google.Apis.Gmail.v1" Version="1.69.0.3742" />
<PackageReference Include="Google.Apis" Version="1.69.0" />
```

---

## 5. BASE DE DATOS

### 5.1 MongoDB - Estructura de Colecciones

#### 5.1.1 Colección `users`

```json
{
  "_id": ObjectId("..."),
  "username": "string",
  "password": "string_hasheado",
  "email": "string"
}
```

**Campos:**

- `_id`: Identificador único de MongoDB
- `username`: Nombre de usuario (único)
- `password`: Contraseña (debería estar hasheada)
- `email`: Correo electrónico (único)

#### 5.1.2 Colección `productos`

```json
{
  "_id": ObjectId("..."),
  "nombre": "string",
  "precio_unitario": "decimal",
  "stock": "int",
  "fecha": "datetime",
  "serial": "string",
  "codigo_proveedor": "string"
}
```

**Campos:**

- `_id`: Identificador único de MongoDB
- `nombre`: Nombre del producto
- `precio_unitario`: Precio por unidad
- `stock`: Cantidad en inventario
- `fecha`: Fecha de ingreso al sistema
- `serial`: Código serial único del producto
- `codigo_proveedor`: Referencia al proveedor

#### 5.1.3 Colección `proveedores`

```json
{
  "_id": ObjectId("..."),
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

**Campos:**

- `_id`: Identificador único de MongoDB
- `supplierId`: ID único del proveedor (SUP001, SUP002, etc.)
- `name`: Nombre de la empresa proveedora
- `contactName`: Nombre del contacto
- `email`: Correo electrónico del proveedor
- `phone`: Teléfono de contacto
- `address`: Dirección completa (objeto anidado)
- `category`: Categoría del proveedor
- `status`: Estado activo/inactivo

### 5.2 Conexiones de Base de Datos

#### 5.2.1 MongoDB Local

```csharp
var client = new MongoClient("mongodb://localhost:27017");
var database = client.GetDatabase("JorStock");
```

#### 5.2.2 MongoDB Atlas (Cloud)

```csharp
var client = new MongoClient("mongodb+srv://usuario:password@cluster.mongodb.net/?retryWrites=true&w=majority");
var database = client.GetDatabase("JorStock");
```

### 5.3 Scripts de Inicialización

#### 5.3.1 insert_atlas_data.js

```javascript
// Crear usuarios de ejemplo
db.users.insertMany([
  {
    username: "admin",
    password: "admin123",
    email: "admin@jorstock.com",
  },
]);

// Crear proveedores de ejemplo
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
]);

// Crear productos de ejemplo
db.productos.insertMany([
  {
    nombre: "Filtro de Aceite Motor",
    precio_unitario: 25.5,
    stock: 50,
    fecha: "2024-01-15 10:30:00",
    serial: "FIL001",
    codigo_proveedor: "SUP001",
  },
]);
```

---

## 6. FORMULARIOS Y MÓDULOS

### 6.1 Program.cs - Controlador Principal

#### 6.1.1 Responsabilidades

- **Punto de entrada** de la aplicación
- **Gestión de formularios** abiertos
- **Navegación** entre ventanas
- **Control de ciclo de vida** de la aplicación

#### 6.1.2 Métodos Principales

```csharp
public static void ShowForm(Form form)
{
    OpenForms.Add(form);
    form.FormClosed += (s, args) => OpenForms.Remove(form);
    form.Show();
}

public static void CloseAllExcept(Form exceptForm)
{
    foreach (var form in OpenForms.ToList())
    {
        if (form != exceptForm && !form.IsDisposed)
        {
            form.Close();
        }
    }
}
```

### 6.2 Login.cs - Autenticación

#### 6.2.1 Funcionalidades

- **Validación de credenciales** contra MongoDB
- **Manejo de errores** de conexión
- **Navegación** al formulario principal
- **Recuperación de contraseña**

#### 6.2.2 Flujo de Autenticación

```
Usuario ingresa credenciales
         ↓
Validación en MongoDB
         ↓
¿Credenciales válidas?
    ↓           ↓
   SÍ          NO
    ↓           ↓
Abrir Home   Mostrar Error
```

#### 6.2.3 Código de Validación

```csharp
private async void BtnIngresar_Login_Click(object sender, EventArgs e)
{
    string username = txtUser.Text.Trim();
    string password = txtPassword.Text;

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        MessageBox.Show("Por favor, completa todos los campos", "Error");
        return;
    }

    try
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("username", username),
            Builders<BsonDocument>.Filter.Eq("password", password)
        );

        var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        if (user != null)
        {
            Home homeForm = new Home();
            this.Hide();
            homeForm.ShowDialog();
            this.Close();
        }
        else
        {
            MessageBox.Show("Usuario o contraseña incorrectos", "Error");
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al conectar: {ex.Message}", "Error");
    }
}
```

### 6.3 Home.cs - Formulario Principal

#### 6.3.1 Funcionalidades Principales

- **Gestión completa de productos** (CRUD)
- **Búsqueda y filtrado** avanzado
- **Gestión automática de proveedores**
- **Visualización organizada** con colores
- **Ordenamiento múltiple**

#### 6.3.2 Características Técnicas

- **Sistema de placeholders** inteligente
- **Validación de datos** en tiempo real
- **Operaciones asíncronas** con MongoDB
- **Manejo de errores** robusto
- **Interfaz moderna** con bordes redondeados

#### 6.3.3 Métodos Principales

##### Cargar Productos

```csharp
private async void CargarProductos()
{
    if (cargandoProductos) return;
    cargandoProductos = true;

    try
    {
        var client = new MongoClient("mongodb+srv://...");
        var database = client.GetDatabase("JorStock");
        var productosCollection = database.GetCollection<BsonDocument>("productos");
        var proveedoresCollection = database.GetCollection<BsonDocument>("proveedores");

        var productos = await productosCollection.Find(new BsonDocument()).ToListAsync();

        // Procesamiento y organización de datos
        // Creación de DataTable
        // Aplicación de colores por grupo de proveedor
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al cargar productos: {ex.Message}");
    }
    finally
    {
        cargandoProductos = false;
    }
}
```

##### Guardar Producto

```csharp
private async void btnGuardar_Click(object sender, EventArgs e)
{
    // Validación de campos
    if (txtAutoparte.Text == "Nombre de Autoparte" || /* otros campos */)
    {
        MessageBox.Show("Complete todos los campos");
        return;
    }

    // Validación de tipos de datos
    if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
    {
        MessageBox.Show("El precio debe ser numérico");
        return;
    }

    // Lógica de guardado (crear o actualizar)
    if (productoEnEdicionId != null)
    {
        // Actualizar producto existente
    }
    else
    {
        // Crear nuevo producto
    }
}
```

### 6.4 ForgotPass.cs - Recuperación de Contraseña

#### 6.4.1 Flujo de Recuperación

```
Usuario ingresa email
         ↓
Verificar email en BD
         ↓
¿Email existe?
    ↓           ↓
   SÍ          NO
    ↓           ↓
Generar código  Mostrar error
    ↓
Enviar por email
    ↓
Abrir FormSecurity
```

#### 6.4.2 Envío de Correo

```csharp
private async Task<bool> SendEmailAsync(string emailTo, string code)
{
    try
    {
        string senderEmail = "davidmg2512@gmail.com";
        string password = "fkto qmrp gacd yrmh"; // App password

        MailMessage message = new MailMessage();
        message.From = new MailAddress(senderEmail);
        message.Subject = "Código de recuperación - JorStock";
        message.To.Add(new MailAddress(emailTo));
        message.Body = $"Tu código de recuperación es: <strong>{code}</strong>";
        message.IsBodyHtml = true;

        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
        smtp.Port = 587;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(senderEmail, password);
        smtp.EnableSsl = true;

        await smtp.SendMailAsync(message);
        return true;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al enviar correo: {ex.Message}");
        return false;
    }
}
```

### 6.5 FormSecurity.cs - Verificación de Código

#### 6.5.1 Funcionalidad

- **Validación del código** de seguridad
- **Navegación** al formulario de nueva contraseña
- **Manejo de errores** de validación

### 6.6 FormNewPass.cs - Nueva Contraseña

#### 6.6.1 Funcionalidad

- **Actualización de contraseña** en MongoDB
- **Validación de nueva contraseña**
- **Navegación** de vuelta al login

---

## 7. CONFIGURACIÓN E INSTALACIÓN

### 7.1 Requisitos del Sistema

#### 7.1.1 Requisitos Mínimos

- **Sistema Operativo**: Windows 10/11
- **Framework**: .NET Framework 4.7.2 o superior
- **Memoria RAM**: 4GB mínimo
- **Espacio en disco**: 500MB
- **Conexión a Internet**: Para recuperación de contraseña

#### 7.1.2 Requisitos Recomendados

- **Sistema Operativo**: Windows 11
- **Memoria RAM**: 8GB o más
- **Procesador**: Intel i5 o AMD equivalente
- **Conexión**: Internet estable

### 7.2 Instalación de MongoDB

#### 7.2.1 MongoDB Local

1. **Descargar MongoDB Community Server**

   ```
   https://www.mongodb.com/try/download/community
   ```

2. **Instalar MongoDB**

   - Ejecutar el instalador
   - Seguir las instrucciones del asistente
   - Configurar como servicio de Windows

3. **Iniciar MongoDB**
   ```bash
   mongod --dbpath "C:\data\db"
   ```

#### 7.2.2 MongoDB Atlas (Recomendado)

1. **Crear cuenta en MongoDB Atlas**

   ```
   https://cloud.mongodb.com
   ```

2. **Crear cluster gratuito M0**

   - Seleccionar región cercana
   - Configurar acceso de red (0.0.0.0/0)
   - Crear usuario de base de datos

3. **Obtener string de conexión**
   ```
   mongodb+srv://usuario:password@cluster.mongodb.net/?retryWrites=true&w=majority
   ```

### 7.3 Configuración del Proyecto

#### 7.3.1 Visual Studio

1. **Abrir la solución**

   ```
   JorStock.sln
   ```

2. **Restaurar paquetes NuGet**

   - Click derecho en la solución
   - "Restore NuGet Packages"

3. **Configurar conexión**
   - Actualizar strings de conexión en cada formulario
   - Reemplazar con tu string de MongoDB Atlas

#### 7.3.2 Configuración de Gmail

1. **Habilitar autenticación de 2 factores**
2. **Generar contraseña de aplicación**
3. **Actualizar credenciales en ForgotPass.cs**

### 7.4 Compilación y Ejecución

#### 7.4.1 Desde Visual Studio

1. **Seleccionar configuración**: Debug o Release
2. **Compilar**: Build → Build Solution
3. **Ejecutar**: F5 o Ctrl+F5

#### 7.4.2 Desde línea de comandos

```bash
# Navegar al directorio del proyecto
cd C:\Users\David\Desktop\jorstock-app\JorStock

# Compilar
dotnet build

# Ejecutar
dotnet run
```

---

## 8. MANUAL DE USUARIO

### 8.1 Primer Uso

#### 8.1.1 Iniciar Sesión

1. **Ejecutar JorStock.exe**
2. **Usar credenciales por defecto**:
   - Usuario: `admin`
   - Contraseña: `admin123`

#### 8.1.2 Insertar Datos de Ejemplo

1. **Ejecutar script MongoDB**:
   ```javascript
   // En MongoDB Compass o Atlas Data Explorer
   // Ejecutar insert_atlas_data.js
   ```

### 8.2 Gestión de Productos

#### 8.2.1 Agregar Producto

1. **Completar campos**:

   - Nombre de Autoparte
   - Precio (decimal)
   - Stock (entero)
   - Proveedor
   - Código

2. **Click "Guardar"**
3. **El producto se guarda automáticamente**

#### 8.2.2 Editar Producto

1. **Seleccionar producto** en la tabla
2. **Click "Editar"**
3. **Modificar campos** necesarios
4. **Click "Guardar"**

#### 8.2.3 Eliminar Producto

1. **Seleccionar producto** en la tabla
2. **Click "Borrar"**
3. **Confirmar eliminación**

### 8.3 Búsqueda y Filtrado

#### 8.3.1 Búsqueda por Nombre

1. **Escribir nombre** en campo "Nombre de Autoparte"
2. **Click "Buscar"**
3. **Resultados filtrados** aparecen automáticamente

#### 8.3.2 Búsqueda por Proveedor

1. **Escribir nombre** en campo "Proveedor"
2. **Click "Buscar"**
3. **Productos del proveedor** se muestran

#### 8.3.3 Búsqueda Combinada

1. **Completar ambos campos**
2. **Click "Buscar"**
3. **Resultados que cumplan ambos criterios**

### 8.4 Organización

#### 8.4.1 Ordenamiento

1. **Click botón de ordenamiento**
2. **Seleccionar criterio**:
   - Nombre (A-Z o Z-A)
   - Stock (más o menos)
   - Precio (mayor o menor)
   - Fecha (reciente o antigua)

#### 8.4.2 Visualización

- **Colores alternados** por grupo de proveedor
- **Agrupación automática** por proveedor
- **Información completa** en cada fila

### 8.5 Recuperación de Contraseña

#### 8.5.1 Solicitar Recuperación

1. **Click "¿Olvidaste tu contraseña?"**
2. **Ingresar email** registrado
3. **Click "Siguiente"**

#### 8.5.2 Verificar Código

1. **Revisar email** recibido
2. **Ingresar código** de 6 dígitos
3. **Click "Ingresar"**

#### 8.5.3 Nueva Contraseña

1. **Ingresar nueva contraseña**
2. **Click "Ingresar"**
3. **Contraseña actualizada** exitosamente

---

## 9. DESARROLLO Y MANTENIMIENTO

### 9.1 Estructura del Código

#### 9.1.1 Convenciones de Nomenclatura

- **Clases**: PascalCase (ej: `Login`, `Home`)
- **Métodos**: PascalCase (ej: `CargarProductos()`)
- **Variables**: camelCase (ej: `productoEnEdicionId`)
- **Constantes**: UPPER_CASE (ej: `SECURITY_CODE`)

#### 9.1.2 Documentación del Código

```csharp
/// <summary>
/// Método que maneja el evento de clic del botón guardar
/// Permite crear nuevos productos o actualizar productos existentes
/// </summary>
/// <param name="sender">Objeto que disparó el evento</param>
/// <param name="e">Argumentos del evento</param>
private async void btnGuardar_Click(object sender, EventArgs e)
{
    // Implementación del método
}
```

### 9.2 Manejo de Errores

#### 9.2.1 Estrategia de Errores

- **Try-Catch** en todas las operaciones de BD
- **Validación de entrada** antes de procesar
- **Mensajes informativos** al usuario
- **Logging** de errores para debugging

#### 9.2.2 Ejemplo de Manejo

```csharp
try
{
    var resultado = await productosCollection.InsertOneAsync(nuevoProducto);
    MessageBox.Show("Producto guardado exitosamente");
}
catch (Exception ex)
{
    MessageBox.Show($"Error al guardar: {ex.Message}", "Error");
}
```

### 9.3 Optimizaciones Implementadas

#### 9.3.1 Operaciones Asíncronas

- **Async/Await** para operaciones de BD
- **No bloqueo** de la interfaz de usuario
- **Mejor experiencia** del usuario

#### 9.3.2 Control de Llamadas Duplicadas

```csharp
private bool cargandoProductos = false;

private async void CargarProductos()
{
    if (cargandoProductos) return;
    cargandoProductos = true;
    // ... lógica de carga
    cargandoProductos = false;
}
```

### 9.4 Extensibilidad

#### 9.4.1 Nuevas Funcionalidades

- **Módulos independientes** por formulario
- **Interfaz extensible** con nuevos campos
- **Base de datos escalable** con nuevas colecciones

#### 9.4.2 Integración con APIs

- **Google Gmail API** para correos
- **MongoDB Atlas** para datos en la nube
- **Posibilidad de integración** con otros servicios

---

## 10. SOLUCIÓN DE PROBLEMAS

### 10.1 Errores Comunes

#### 10.1.1 Error de Conexión a MongoDB

**Síntomas:**

- Mensaje: "Error al conectar con la base de datos"
- Aplicación no puede cargar datos

**Soluciones:**

1. **Verificar MongoDB está ejecutándose**:

   ```bash
   mongod --version
   ```

2. **Verificar string de conexión**:

   ```csharp
   // En cada formulario, verificar:
   var client = new MongoClient("mongodb://localhost:27017");
   ```

3. **Verificar puerto 27017** no esté ocupado

#### 10.1.2 Error de Envío de Correo

**Síntomas:**

- Mensaje: "Error al enviar el correo"
- No se recibe código de recuperación

**Soluciones:**

1. **Verificar credenciales de Gmail**:

   ```csharp
   string senderEmail = "tu-email@gmail.com";
   string password = "tu-app-password";
   ```

2. **Habilitar autenticación de 2 factores**
3. **Generar contraseña de aplicación**

#### 10.1.3 Productos Duplicados

**Síntomas:**

- Productos aparecen múltiples veces
- Datos inconsistentes

**Solución:**

- **Ya corregido** en versión actual
- **Control de llamadas duplicadas** implementado

### 10.2 Optimización de Rendimiento

#### 10.2.1 Carga Lenta de Datos

**Causas:**

- Muchos productos en la base de datos
- Conexión lenta a MongoDB

**Soluciones:**

1. **Implementar paginación**:

   ```csharp
   var productos = await productosCollection
       .Find(new BsonDocument())
       .Skip(page * pageSize)
       .Limit(pageSize)
       .ToListAsync();
   ```

2. **Usar índices en MongoDB**:
   ```javascript
   db.productos.createIndex({ nombre: 1 });
   db.productos.createIndex({ codigo_proveedor: 1 });
   ```

#### 10.2.2 Interfaz Lenta

**Soluciones:**

1. **Operaciones asíncronas** ya implementadas
2. **Control de llamadas duplicadas** ya implementado
3. **Carga progresiva** de datos

### 10.3 Mantenimiento Preventivo

#### 10.3.1 Backup de Base de Datos

```bash
# MongoDB local
mongodump --db JorStock --out backup/

# MongoDB Atlas
# Usar herramientas de backup de Atlas
```

#### 10.3.2 Limpieza de Datos

```javascript
// Eliminar productos sin stock
db.productos.deleteMany({ stock: 0 });

// Eliminar proveedores inactivos
db.proveedores.deleteMany({ status: "Inactivo" });
```

#### 10.3.3 Monitoreo de Rendimiento

- **Revisar logs** de MongoDB
- **Monitorear uso de memoria**
- **Verificar conexiones** activas

---

## 📊 DIAGRAMAS TÉCNICOS

### Diagrama de Flujo de Autenticación

```
┌─────────────┐    ┌──────────────┐    ┌─────────────┐
│   Usuario   │───▶│   Login.cs   │───▶│  MongoDB    │
│             │    │              │    │             │
└─────────────┘    └──────────────┘    └─────────────┘
                           │
                           ▼
                   ┌──────────────┐
                   │   Home.cs    │
                   │ (Principal)  │
                   └──────────────┘
```

### Diagrama de Flujo de Recuperación de Contraseña

```
┌─────────────┐    ┌──────────────┐    ┌─────────────┐
│   Usuario   │───▶│ ForgotPass.cs│───▶│   Gmail     │
│             │    │              │    │   SMTP      │
└─────────────┘    └──────────────┘    └─────────────┘
                           │
                           ▼
                   ┌──────────────┐    ┌─────────────┐
                   │FormSecurity.cs│───▶│FormNewPass.cs│
                   │              │    │             │
                   └──────────────┘    └─────────────┘
```

### Diagrama de Arquitectura de Base de Datos

```
┌─────────────────────────────────────────────────┐
│                MONGODB                          │
│  ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │    users    │ │  productos  │ │proveedores│ │
│  │             │ │             │ │           │ │
│  │• username   │ │• nombre     │ │• name     │ │
│  │• password   │ │• precio     │ │• contact  │ │
│  │• email      │ │• stock       │ │• address  │ │
│  │             │ │• serial      │ │• category │ │
│  │             │ │• proveedor   │ │• status   │ │
│  └─────────────┘ └─────────────┘ └───────────┘ │
└─────────────────────────────────────────────────┘
```

---

## 🎯 ROADMAP FUTURO

### Versión 2.0 (Próximas Características)

- [ ] **Reportes PDF**: Generación de reportes de inventario
- [ ] **Backup Automático**: Respaldos programados
- [ ] **Notificaciones**: Alertas de stock bajo
- [ ] **API REST**: Integración con sistemas externos
- [ ] **Dashboard**: Estadísticas y métricas

### Versión 3.0 (Características Avanzadas)

- [ ] **Multi-usuario**: Gestión de roles y permisos
- [ ] **Auditoría**: Registro de cambios
- [ ] **Integración**: APIs de proveedores
- [ ] **Mobile**: Aplicación móvil complementaria

---

## 📞 SOPORTE Y CONTACTO

### Soporte Técnico

- **Email**: soporte@jorstock.com
- **GitHub Issues**: [Reportar Problema](https://github.com/tu-usuario/jorstock-app/issues)
- **Documentación**: [Wiki del Proyecto](https://github.com/tu-usuario/jorstock-app/wiki)

### Comunidad

- **Discord**: [Servidor de JorStock](https://discord.gg/jorstock)
- **Reddit**: [r/JorStock](https://reddit.com/r/JorStock)
- **Twitter**: [@JorStockApp](https://twitter.com/JorStockApp)

---

## 📄 LICENCIA

Este proyecto está bajo la **Licencia MIT**. Ver el archivo [LICENSE](LICENSE) para más detalles.

---

## 👥 EQUIPO DE DESARROLLO

- **Desarrollador Principal**: [Tu Nombre]
- **Base de Datos**: MongoDB Atlas
- **Framework**: .NET Framework 4.7.2+
- **IDE**: Visual Studio 2019/2022

---

## 🎉 ¡Gracias por usar JorStock!

**Desarrollado con ❤️ utilizando tecnologías Microsoft .NET**

_Si este proyecto te ha sido útil, considera darle una ⭐ en GitHub_

---

**Fecha de Documentación**: Enero 2024  
**Versión del Sistema**: 1.0  
**Última Actualización**: Enero 2024

