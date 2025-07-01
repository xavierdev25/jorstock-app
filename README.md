# JorStock

JorStock es una aplicación de escritorio desarrollada en C# utilizando Windows Forms que proporciona un sistema de gestión con autenticación de usuarios y características de seguridad.

## Características

- **Sistema de Autenticación Seguro**

  - Inicio de sesión con usuario y contraseña
  - Sistema de recuperación de contraseña mediante correo electrónico
  - Verificación de código de seguridad
  - Interfaz de cambio de contraseña

- **Interfaz de Usuario Moderna**

  - Diseño limpio y profesional
  - Ventanas con bordes redondeados
  - Navegación intuitiva entre formularios
  - Botones de minimizar y cerrar personalizados

- **Base de Datos**
  - Integración con MongoDB para almacenamiento de datos
  - Gestión segura de credenciales de usuario
  - Operaciones asíncronas para mejor rendimiento

## Tecnologías Utilizadas

- **Lenguaje:** C# (.NET Framework)
- **Framework UI:** Windows Forms
- **Base de Datos:** MongoDB
- **Paquetes Principales:**
  - MongoDB.Driver
  - System.Net.Mail (para envío de correos)
  - Google.Apis.Gmail.v1 (para servicios de correo)

## Requisitos del Sistema

- Windows OS
- .NET Framework
- MongoDB Server instalado y ejecutándose localmente
- Conexión a Internet (para la funcionalidad de recuperación de contraseña)

## Estructura del Proyecto

El proyecto está organizado en varios formularios que manejan diferentes aspectos de la aplicación:

- **Login.cs:** Formulario principal de inicio de sesión
- **ForgotPass.cs:** Manejo de recuperación de contraseña
- **FormSecurity.cs:** Verificación de código de seguridad
- **FormNewPass.cs:** Cambio de contraseña
- **Home.cs:** Interfaz principal de la aplicación

## Seguridad

- Autenticación de usuarios contra base de datos MongoDB
- Sistema de recuperación de contraseña mediante correo electrónico
- Códigos de verificación generados aleatoriamente
- Validación de entradas de usuario
- Manejo seguro de sesiones

## Configuración

1. Asegúrate de tener MongoDB instalado y ejecutándose en `localhost:27017`
2. La base de datos debe llamarse `JorStock`
3. Debe existir una colección `users` con los campos:
   - username
   - password
   - email

## Desarrollo

Este proyecto fue desarrollado en Visual Studio utilizando C# y .NET Framework. Utiliza una arquitectura basada en formularios Windows Forms con un diseño moderno y funcional.

---

Desarrollado con ❤️ utilizando tecnologías Microsoft .NET
