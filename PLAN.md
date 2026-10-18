# Plan de trabajo — PartyBot

Complementa a `roadmap.md` (la visión). Este archivo es el **día a día**: qué está hecho, qué falta, quién hace qué y cómo retomar el trabajo en una sesión nueva.

## Objetivo

**Meta v1.0:** 2-4 amigos en un mismo PC, 5 rondas de "montar robot en 60 s → minijuego", 15+ piezas, 3+ minijuegos.
**Meta actual (Hito 2):** montar un robot con el ratón en una pantalla de taller, guardarlo y probarlo en la arena.

---

## ▶ Cómo retomar (leer primero)

**Última sesión: 18 oct 2026.**

1. **Hay trabajo SIN COMMIT y SIN PROBAR**: la pantalla del taller (tareas 2.1-2.4). Ver "Pendiente de verificar" abajo.
2. Antes de seguir con 2.5, Hugo tiene que hacer esos pasos en Unity y confirmar que funciona. Entonces se hace el commit.
3. Después: tarea 2.5 (guardar/cargar) y 2.6 (botón Probar).

### Pendiente de verificar (Hito 2, primera tanda)

Pasos en Unity:
1. **Window > TextMeshPro > Import TMP Essential Resources** → Import (solo una vez; sin esto no se ve texto).
2. Compila sin errores rojos. Test Runner > EditMode > Run All → **52 tests en verde** (14 nuevos en `BuilderModelTests`).
3. Menú **PartyBot > Crear escena Taller** → crea `Scenes/Builder.unity`, `Data/UiTheme.asset` y `Art/UI/RoundedRect.png`.
4. Abrir escena **Builder**, Game view en 1920x1080, **Play**. Debe verse: barra superior "PARTYBOT · TALLER", panel PIEZAS a la izquierda, rejilla 9x9 con núcleo en el centro, panel ESTADO a la derecha.
5. Probar: elegir pieza, colocar (clic izq.), recoger para mover (clic izq. sobre pieza), borrar (clic der.), girar (R / rueda / Shift+R), soltar (Esc), pieza suelta → borde rojo y problema listado.

Archivos sin commit: `Scripts/Builder/*`, `Scripts/UI/*`, `Scripts/Editor/BuilderSceneBuilder.cs`, `Tests/EditMode/BuilderModelTests.cs`, `Art/Fonts/*` (Lilita One + licencia OFL), cambios en `PartyBot.Runtime.asmdef` (refs a `UnityEngine.UI` y `Unity.TextMeshPro`), `SandboxSceneBuilder.cs`, `StarterContentMenu.cs`, y los valores de `RobotTuning.asset` que ajustó Hugo.

---

## Reparto

| Claude | Hugo |
|---|---|
| Escribir scripts C#, ScriptableObjects, asmdefs, tests | Abrir Unity, darle a Play y decir qué se siente mal |
| Git: commits y push (el remoto ya existe) | Ejecutar los menús de PartyBot y guardar escenas (Ctrl+S) |
| Scripts de editor que generan escenas y assets | Decidir el "feel" y el aspecto: velocidad, colores, cuánto caos |
| Mantener este archivo y el roadmap al día | Arte, sonido, diseño de piezas y minijuegos, playtests |

**Regla de oro:** una tarea = un commit. Después de cada tarea Hugo prueba en Unity y dice "ok" o qué falla. Claude no puede compilar fuera de Unity: la compilación y los tests los confirma Hugo.

---

## Hito 0 — Cimientos ✅

| # | Tarea | Estado |
|---|---|---|
| 0.1 | Renombrar `My project` → `PartyBot` | ✅ |
| 0.2 | `git init`, `.gitignore`, `.gitattributes` con LFS | ✅ |
| 0.3 | Repo en GitHub y primer push | ✅ |
| 0.4 | Carpetas `Assets/_Project/...` + asmdefs | ✅ |
| 0.5 | Limpiar plantilla (Welcome, tutoriales, Visual Scripting) | ✅ |
| 0.6 | Escena `Sandbox` con cámara cenital y arena | ✅ |

## Hito 1 — Robot conducible ✅

| # | Tarea | Estado |
|---|---|---|
| 1.1 | Prueba de física: joints vs. cuerpo compuesto | ✅ (código en el commit `4abee0c`) |
| 1.2 | Elegir | ✅ **Cuerpo compuesto** |
| 1.3 | `PartDefinition` + `PartCatalog` | ✅ |
| 1.4 | `RobotData` / `PlacedPart` + JSON + `RobotValidator` + tests | ✅ |
| 1.5 | `RobotAssembler`: `RobotData` → robot en escena | ✅ |
| 1.6 | 4 piezas: núcleo, bloque, rueda, propulsor | ✅ |
| 1.7 | Controles: ruedas tipo tanque + propulsor, teclado y mando | ✅ |
| 1.8 | Ajustes de conducción en vivo (`RobotTuning`) | ✅ |

## Hito 2 — Editor de montaje (en curso)

| # | Tarea | Estado |
|---|---|---|
| 2.1 | Rejilla 9x9 y colocar pieza con clic | 🟡 Escrito, falta verificar |
| 2.2 | Rotar / borrar / mover | 🟡 Escrito, falta verificar |
| 2.3 | Validación en vivo + borde rojo en piezas con error | 🟡 Escrito, falta verificar |
| 2.4 | Panel de piezas (tarjetas) y panel de estado | 🟡 Escrito, falta verificar · Hugo opina del diseño |
| 2.5 | Guardar/cargar robots en JSON | ⬜ Siguiente |
| 2.6 | Botón "Probar" → lleva el robot a la Sandbox | ⬜ |
| 2.7 | Montar 5 robots distintos y anotar qué molesta | ⬜ **Hugo** |

**Ideas para 2.5/2.6:** botones GUARDAR / CARGAR / PROBAR en la barra superior (ya hay hueco a la derecha, junto a "Empezar de nuevo"); nombre del robot editable; guardar en `Application.persistentDataPath/robots/*.json`; pasar el `RobotData` a la Sandbox con un objeto estático o `DontDestroyOnLoad` y montarlo allí con `RobotSpawner`. Botón "Volver al taller" en la Sandbox.

## Hitos siguientes (se detallarán al llegar)

- **Hito 3** — Piezas de combate y daño (pala, martillo, sierra, cañón; piezas que se desprenden). La separación de piezas ya se probó en el commit `4abee0c` (`CompoundRobot.DetachNext`).
- **Hito 4** — Bucle de partida local 2-4 jugadores (`PlayerInput` por jugador en lugar de `LocalRobotInput`) → **primer playtest con amigos**.
- **Hito 5** — 3 minijuegos (Rey de la colina, Fútbol, Supervivencia).
- **Hito 6** — Contenido y pulido. Después: progresión, online y lanzamiento (opcionales).

---

## Mapa del proyecto

Proyecto Unity 6.6 (6000.6.5f1), 2D URP, en `PartyBot/`. Todo lo nuestro está en `PartyBot/Assets/_Project/`.

| Carpeta | Qué hay |
|---|---|
| `Scripts/Core` | `GridMath` (rotaciones de 90º y vecinos) |
| `Scripts/Parts` | `PartDefinition`, `PartCatalog`, `PartCategory`, `PartBehaviourKind`; `Behaviours/` con `PartBehaviour`, `WheelBehaviour`, `ThrusterBehaviour` |
| `Scripts/Robot` | `RobotData`/`PlacedPart`, `RobotValidator`, `RobotAssembler`, `RobotBody`, `RobotPart`, `RobotController` (+ `RobotCommand`), `RobotTuning`, `RobotSpawner` |
| `Scripts/Input` | `LocalRobotInput` (namespace `PartyBot.Controls`, provisional hasta el Hito 4) |
| `Scripts/Builder` | `BuilderModel` (lógica del taller, testeable), `BuilderScreen` (pantalla), `GridView`/`GridCell`, `PaletteView`, `StatusView` |
| `Scripts/UI` | `UiTheme` (colores, fuente, sprite redondeado), `UiFactory` (atajos para crear uGUI desde código) |
| `Scripts/Editor` | Menús: `SandboxSceneBuilder`, `StarterContentMenu`, `BuilderSceneBuilder` |
| `Tests/EditMode` | Tests NUnit de todo lo anterior |
| `Data/` | `Parts/*.asset` (Core, Block, Wheel, Thruster), `PartCatalog`, `RobotTuning`, `UiTheme` |
| `Scenes/` | `Sandbox` (arena con `RobotSpawner`), `Builder` (taller) |
| `Art/` | `WhiteSquare.png` (placeholder), `UI/RoundedRect.png`, `Fonts/LilitaOne` |

**Assemblies:** `PartyBot.Runtime` (todo `Scripts/` menos Editor; refs Input System, uGUI, TextMeshPro), `PartyBot.Editor`, `PartyBot.Tests.EditMode` (ve los `internal` del runtime vía `AssemblyInfo.cs`).

**Menús del editor (barra PartyBot):**
- *Crear escena Sandbox* — regenera la arena.
- *Crear piezas iniciales* — crea las 4 piezas y el catálogo si faltan (no pisa ajustes).
- *Añadir robot de prueba a la escena* — `RobotSpawner` en la escena abierta (o completa el existente).
- *Crear escena Taller* — escena Builder + tema + sprite redondeado.

### Decisiones técnicas tomadas

| Tema | Decisión |
|---|---|
| Física del robot | **Un Rigidbody2D por robot** con un `BoxCollider2D` por celda. Masa = suma de piezas; centro de masas ponderado. Nada de joints. |
| Juego cenital | Gravedad 2D a 0 en todo el proyecto. |
| Rejilla | 9x9, celda (0,0) abajo-izquierda, rotación en cuartos de vuelta antihorarios (0-3). Conexión solo por caras. |
| Piezas | `PartDefinition` (ScriptableObject) con id estable (se guarda en JSON), forma en celdas, masa, vida, coste, `Behaviour` y `Power`. |
| Control | El robot recibe un `RobotCommand` (Move + Boost); quién lo rellene (teclado, mando, IA, red) da igual. Ruedas: tracción tipo tanque por lado del centro de masas + agarre lateral. |
| UI | **uGUI construida desde código** con `UiTheme`/`UiFactory`; `InputSystemUIInputModule`. Resolución de referencia 1920x1080. |
| Estilo visual | Fondo azul marino oscuro, paneles redondeados, acentos amarillo/turquesa/morado, verde = válido, coral = error. Títulos en Lilita One, texto en Liberation Sans (TMP). |
| Taller | No deja colocar piezas que se salgan, se solapen o dupliquen el núcleo; sí deja piezas sueltas (se marcan en rojo). Clic en pieza colocada = recogerla. |

### Valores de conducción actuales (`Data/RobotTuning`, ajustados por Hugo)

Linear Damping **3** · Angular Damping **1** · Wheel Power ×**2** · Wheel Grip **10** · Thruster Power ×**1**.
Potencia por pieza: rueda 25, propulsor 80.

---

## Lo que puede hacer Hugo en paralelo

**Diseño (papel o un doc)** — lo más útil ahora
- [ ] Opinión sobre la pantalla del taller (captura + qué cambiarías).
- [ ] Lista de 20 piezas soñadas: nombre, qué hace, tamaño en celdas. Marca las 8 favoritas. *(Se necesita en el Hito 3.)*
- [ ] Reglas de 3 minijuegos: cómo se gana, cuánto dura, cuántos puntos da. *(Hito 5.)*
- [ ] Bocetos de 3 robots "graciosos".

**Arte y sonido**
- [ ] Elegir estilo: formas simples de colores vs. pack de assets (Kenney.nl es gratis y encaja).
- [ ] Buscar sonidos de choques, motores, explosiones (freesound.org, Kenney).

**Otros**
- [x] Renombrar carpeta, GitHub + Git LFS.
- [ ] Conseguir un mando (Xbox/PS) para probar multijugador.
- [ ] Apuntar 3-4 amigos dispuestos a probar al acabar el Hito 4.

## Historial de commits

| Commit | Qué |
|---|---|
| `c7794fe` | Proyecto inicial, roadmap y plan |
| `4ebae22` | Estructura de carpetas, asmdefs, limpieza de plantilla |
| `69c0686` | Escena Sandbox |
| `4abee0c` | Prueba de física (joints vs. compuesto) — referencia |
| `efa0eeb` | Decisión: cuerpo compuesto; prueba borrada |
| `e6b68c1` | Modelo de datos y validador (32 tests) |
| `7fd9690` | RobotAssembler y piezas iniciales |
| `2c7ccb2` | Controles: ruedas y propulsor |
| `c757319` | RobotTuning — cierra Hito 1 (38 tests) |
| *(pendiente)* | Taller: tareas 2.1-2.4 (52 tests) |
