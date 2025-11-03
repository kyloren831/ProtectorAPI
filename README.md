# 🔐 SISTEMA DE SEGURIDAD E INVENTARIO

> _"Control, auditoría y continuidad para entornos empresariales seguros."_

![Oracle](https://img.shields.io/badge/Database-Oracle%20SQL-orange?style=for-the-badge&logo=oracle&logoColor=white)
![CSharp](https://img.shields.io/badge/Backend-C%23-512bd4?style=for-the-badge&logo=csharp&logoColor=white)
![JWT](https://img.shields.io/badge/Seguridad-JWT-green?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

---

## 📘 Descripción General

El **Sistema de Seguridad e Inventario** es un proyecto desarrollado como parte del curso **IF-5100 Administración de Bases de Datos** en la **Universidad de Costa Rica – Sede del Pacífico**.  
Su propósito es administrar usuarios, roles, permisos y sistemas mediante una arquitectura segura, auditada y fácilmente extensible.  
El módulo de inventario complementa la solución al permitir la gestión de productos y proveedores dentro del mismo entorno controlado.

---

## 🎯 Objetivos Principales

- Controlar accesos y permisos a nivel de sistema y pantalla.
- Mantener trazabilidad mediante **bitácoras automáticas (triggers)**.
- Implementar una estructura escalable para nuevos sistemas y módulos.
- Garantizar seguridad y auditoría mediante **JWT** y roles de base de datos.
- Asegurar continuidad mediante **planes de contingencia y replicación de datos**.

---

## 🧠 Arquitectura General

```
Frontend (Aplicación en C# o Web App)
   │
   ├──> Backend (API en C#)
   │       ├──> Seguridad JWT
   │       ├──> Validación de roles y permisos
   │       └──> Gestión de bitácoras
   │
   └──> Base de Datos (Oracle SQL)
            ├── Tablas funcionales (Usuarios, Roles, Permisos)
            ├── Bitácoras de acciones
            ├── Triggers automáticos
            ├── Roles y Tablespaces
            └── Índices optimizados
```

---

## 🧩 Componentes del Sistema

| Módulo | Descripción |
|--------|--------------|
| **Usuarios** | Gestión de credenciales, estados y roles asignados. |
| **Roles** | Agrupación de permisos con niveles de acceso definidos. |
| **Permisos** | Control detallado por pantalla y sistema. |
| **Sistemas y Pantallas** | Registro modular de aplicaciones y sus vistas. |
| **Bitácora** | Registro automático de cambios, accesos y auditorías. |
| **Inventario** | Gestión de productos y proveedores. |

---

## ⚙️ Estructura de la Base de Datos

### Tablas Clave (Sistema de Seguridad)

- **Usuarios** (`IdUsuario`, `Nombre`, `Correo`, `Contraseña`, `Estado`)
- **Roles**
- **Permisos**
- **Sistemas**
- **Pantallas**
- **Bitacora_Usuarios**
- **Usuarios_Roles**, **Rol_Permisos_Pantallas**, **Usuarios_Permisos_Pantallas**

### Tablas del Inventario

- **Producto** (`IdProducto`, `IdProveedor`, `Nombre`, `Precio`, `Cantidad`)

---

## 🧠 Automatización con Triggers

El sistema incluye **triggers inteligentes** para garantizar la trazabilidad y auditoría:

| Trigger | Propósito |
|----------|------------|
| `trg_bitacora_update_sistemas` | Registra modificaciones en la tabla de sistemas. |
| `autoincrementUsuarios` | Genera IDs automáticos para nuevos usuarios. |
| `trg_bitacora_update_usuarios` | Audita cambios en usuarios (antes y después). |
| `trg_bitacora_acceso_usuario` | Registra accesos de usuarios. |

---

## 🧮 Funciones y Procedimientos

| Nombre | Descripción |
|---------|-------------|
| `fn_TotalPermisosUsuario` | Calcula los permisos totales de un usuario. |
| `fn_EsAdministrador` | Determina si un usuario es administrador funcional. |
| `fn_ObtenerUsuarioBD` | Devuelve el usuario de base de datos actual. |
| `sp_InsertarBitacora` | Inserta movimientos en la bitácora. |
| `fn_CompararValores` | Detecta y formatea diferencias entre valores. |

---

## 🧱 Tablespaces y Roles de Base de Datos

| Tablespace | Uso |
|-------------|-----|
| `USERS_TS` | Tablas funcionales. |
| `INDEX_TS` | Índices. |
| `AUDI_TS` | Auditoría. |
| `TEMP_TS` | Datos temporales. |

| Rol | Usuario | Permisos |
|------|----------|----------|
| `ROL_OWNER` | `PROTECTOR_APP` | Control total de objetos. |
| `ROL_APP` | `APP_USER` | SELECT, INSERT, UPDATE, DELETE. |
| `ROL_AUDITOR` | `AUDITOR_USER` | Solo lectura en bitácoras. |

---

## 🔐 Seguridad JWT

El sistema utiliza **JSON Web Tokens (JWT)** para controlar accesos:

1. El usuario se autentica con su correo y contraseña.  
2. El backend genera un JWT con su `IdUsuario`, roles y permisos.  
3. Cada solicitud API incluye el token para validar autorización.  
4. Los permisos se verifican contra la base de datos en tiempo real.

Esto permite agregar nuevos módulos sin modificar la lógica de autenticación.

---

## ⚡ Índices y Optimización

Se aplicaron índices estratégicos para optimizar las consultas SQL:

```sql
CREATE INDEX idx_usuarios_correo ON Usuarios(correo);
CREATE INDEX idx_usuarios_estado ON Usuarios(estado);
CREATE INDEX idx_usuarios_roles_usuario ON Usuarios_Roles(id_usuario);
CREATE INDEX idx_permisos_idrol_iduser ON Permisos(id_rol, id_usuario);
CREATE INDEX IX_Pantallas_IdSistema ON PROTECTOR_APP.Pantallas(IdSistema);
```

Los índices reducen los tiempos de búsqueda y mejoran el rendimiento de joins frecuentes.

---

## 🧯 Plan de Contingencia y Replicación

### Estrategias Preventivas
- **Backups automáticos diarios.**
- **Bitácoras activas** para rastrear anomalías.
- **Consultas parametrizadas** (prevención de SQL Injection).
- **Entornos separados** (dev / test / prod).

### Replicación de Datos
- Replicación lógica entre servidor principal y standby.
- Restauraciones automáticas con respaldo `.bak` o `.dmp`.
- Pruebas semanales de recuperación.

### Plan de Respuesta ante Incidentes

| Tiempo | Acción |
|--------|--------|
| 0–5 min | Notificar al equipo y aislar el fallo. |
| 15–60 min | Validar integridad del standby y reiniciar servicios. |
| < 24h | Restaurar sincronización y documentar incidente. |


---

## 📚 Bibliografía Consultada

- IBM. (s.f.). [¿Qué es la replicación de datos?](https://www.ibm.com/mx-es/topics/data-replication)
- Oracle. (2024). [CREATE TRIGGER – Oracle Docs](https://docs.oracle.com/en/database/oracle/oracle-database/21/sqlrf/CREATE-TRIGGER.html)
- Google Cloud. (s.f.). [Recuperación ante desastres para SQL Server](https://cloud.google.com/compute/docs/instances/sql-server/disaster-recovery-for-microsoft-sql-server?hl=es-419)
- GeeksforGeeks. (n.d.). [Triggers in SQL](https://www.geeksforgeeks.org/triggers-in-sql/)
- Asana. (s.f.). [Plan de contingencia paso a paso](https://asana.com/es/resources/contingency-plan)

---

## 🧾 Licencia

Proyecto académico desarrollado para el curso **IF-5100 Administración de Bases de Datos**  
**Universidad de Costa Rica – Sede del Pacífico**  
Licencia: **Uso educativo y demostrativo.**

---

> 🧠 _La seguridad no es un producto, es un proceso continuo de control, auditoría y prevención._
