# Sistema de Gestión Académica - API (Cloud Version)

Este repositorio contiene el backend del Sistema de Gestión Académica, construido bajo los principios de **Clean Architecture** utilizando **.NET 8**. 

Esta rama principal (`main`) está configurada específicamente para entornos de **Producción en la Nube (Cloud-Native)** y se integra con una aplicación móvil desarrollada en React Native / Expo.

## 🏗️ Arquitectura de la Nube (Cloud Stack)

El sistema ha sido migrado de una infraestructura local a servicios gestionados en la nube para garantizar alta disponibilidad y acceso global:

* **Hosting de la API:** [Render](https://render.com/) (Despliegue automatizado mediante contenedores Docker).
* **Base de Datos Principal:** [Supabase](https://supabase.com/) (PostgreSQL con Connection Pooling configurado).
* **Caché Distribuida:** [Upstash](https://upstash.com/) (Redis Serverless).

## 🛠️ Tecnologías y Patrones

* **Framework:** .NET 8.0 (C#)
* **Arquitectura:** Clean Architecture (Api, Domain, Application, Persistence)
* **ORM / Acceso a Datos:** Dapper combinado con `Npgsql` para PostgreSQL.
* **Patrones de Diseño:** CQRS (vía MediatR), Repository Pattern, Decorator (para CachingBehavior con SemaphoreSlim).
* **Contenedores:** Docker (Multi-stage build).

## 🔀 Estrategia de Ramas (Version Control)

Para mantener la estabilidad entre los entornos de prueba locales y la versión expuesta a internet, el repositorio se divide en:

* `main`: **(Actual)** Versión Cloud. Apunta a PostgreSQL (Supabase) y Redis en la nube (Upstash). Lista para el despliegue continuo en Render.
* `academic_local`: Versión On-Premise. Apunta a SQL Server local y contenedores de Redis en Docker Desktop.

## 🚀 Ejecución Local de la Versión Cloud

Para ejecutar esta versión en tu máquina conectándote a los servicios de la nube, **NO modifiques** el `appsettings.json`. Las credenciales se manejan de forma segura a través de **User Secrets** en .NET.

1. Configura la cadena de conexión de PostgreSQL (Supabase):
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=TU_HOST;Port=6543;Database=postgres;Username=postgres;Password=TU_PASSWORD;Include Error Detail=true;"