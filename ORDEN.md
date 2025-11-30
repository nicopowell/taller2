
--- 
# 🏁 FASE 0: Preparación (5-10 min)
El objetivo es que el proyecto compile y tenga las librerías necesarias.

1. **Crear el Proyecto:** `dotnet new mvc -n ParcialApellidoNombre`.
2. **Instalar Paquetes:** Instala `Microsoft.Data.Sqlite` (o el que te pidan).
3. **Copiar Estructura de Carpetas:** Crea ya las carpetas `Interfaces`, `Repositorios`, `Services`, `ViewModels`, `DB`.
4. **Copiar `Program.cs`:** Copia la configuración que te pasé (DI, Session, Cookies). Aunque dé error porque faltan clases, déjalo comentado o listo para descomentar.

--- 

# 🏗️ FASE 1: El Núcleo de Datos (15-20 min)
Sin esto, la aplicación no tiene "sangre".

1. **Modelos (`Models/*.cs`):**
    - Crea las clases simples (`Producto`, `Presupuesto`, `Usuario`).
    - Son fáciles y rápidas. Copia y pega propiedades.

2. **Interfaces (`Interfaces/*.cs`):**
    - Define los contratos (`IProductoRepository`, etc.).
    - Esto te permite escribir el controlador después aunque no hayas terminado el repositorio.

3. **Repositorios (`Repositorios/*.cs`):**
    - Implementa primero el CRUD básico (ej: `ProductoRepository`).
    - Deja la lógica compleja (Relaciones N:M) para después si estás corto de tiempo.
    - Prioridad: Que el `GetAll` y `Add` funcionen.

--- 

# 🔐 FASE 2: Seguridad y Lógica (15-20 min)
Necesario para que el Login funcione y puedas entrar al sistema.

1. **Repositorio de Usuario:** Implementa `GetUser` en `UsuarioRepository`.

2. **Servicio de Autenticación:** Copia el `AuthenticationService.cs` que te pasé. Es código "boilerplate" (casi siempre igual).

3. **ViewModel de Login:** Crea `LoginViewModel`.

4. **Controlador de Login:** Implementa `LoginController`.

5. **Prueba Rápida:** Intenta loguearte. Si guarda la cookie, ¡ya tienes el 50% del examen asegurado!

--- 

# 🎮 FASE 3: Funcionalidad Principal (30-40 min)
Aquí es donde sumas los puntos gordos.

1. **ViewModels de Negocio:** Crea `ProductoViewModel` y `PresupuestoViewModel` con las validaciones (`[Required]`).

2. **Controladores:**
    - Empieza por el **CRUD Simple** (Productos).
    - Copia la estructura: Inyección en constructor -> Acción GET -> Acción POST.
    - **No olvides:** Validar `ModelState.IsValid` en los POST.

3. **Vistas CRUD:**
    - Copia las vistas `Index`, `Create`, `Edit`.
    - Usa los Tag Helpers (`asp-for`).

--- 

# 🚀 FASE 4: Lógica Compleja (El "10")
*Solo si te sobra tiempo o si el CRUD básico ya funciona.*
1. **Relaciones N:M:** Implementa `AgregarProducto` en el Controlador de Presupuestos y su Vista correspondiente.
    - **Recuerda el truco:** Si el `ModelState` falla, recarga el SelectList antes de devolver la vista.

--- 

## 💡 Consejos de Supervivencia para el Examen
- **Compila constantemente:** No escribas todo el código de una sola vez. Haz *Interfaz -> Repositorio -> Build*. Si hay error, arréglalo ahí.

- **Usa tu Guía:** Tienes el código comentado. Si te piden "Alta de Clientes", ve a tu guía, busca "Alta de Productos", copia, pega y cambia "Producto" por "Cliente". Ctrl+H (Reemplazar) es tu mejor amigo.

- **Prioriza:** Un ABM (Alta-Baja-Modificación) completo y funcionando vale más que 3 ABMs a medias. Asegura primero Productos o Usuarios.

- **El error más común:** Olvidarse de registrar algo en `Program.cs.` Si te dice "Unable to resolve service...", ve directo a `Program.cs.`