# Plan de trabajo — PartyBot

Complementa a `roadmap.md` (la visión). Este archivo es el **día a día**: tareas pequeñas, quién hace qué y cómo sabemos que está hecho.

## Objetivo

**Meta inmediata (Hito 1):** un robot montado por código que se conduce por una arena sin temblar ni romperse.
**Meta v1.0:** 2-4 amigos en un mismo PC, 5 rondas de "montar robot en 60 s → minijuego", 15+ piezas, 3+ minijuegos.

## Estado actual (8 oct 2026)

- Proyecto Unity 6.6 (6000.6.5f1) con plantilla 2D URP en `My project/`. Input System ya instalado.
- Sin Git, sin carpetas `_Project`, sin código propio.
- Fase 0 a medias.

## Reparto

| Claude (yo) | Hugo (tú) |
|---|---|
| Escribir scripts C#, ScriptableObjects, asmdefs, tests | Abrir Unity, darle a Play y decir qué se siente mal |
| `.gitignore`, estructura de carpetas, commits | Crear el repo remoto (GitHub) y hacer push |
| Scripts de editor que generen prefabs/escenas de prueba | Montar escenas/prefabs a mano cuando sea más rápido en el editor |
| Proponer valores de física y ajustarlos según tu feedback | Decidir el "feel": velocidad, peso, cuánto caos |
| Mantener este archivo y el roadmap al día | Arte, sonido, nombre, playtests (ver "En paralelo") |

**Regla de oro:** una tarea = una sesión corta = un commit. Después de cada tarea tú pruebas en Unity y me dices "ok" o qué falla.

---

## Hito 0 — Cimientos (1-2 días)

| # | Tarea | Quién | Hecho cuando |
|---|---|---|---|
| 0.1 ✅ | Renombrar `My project` → `PartyBot` (con Unity cerrado) | Tú | Unity abre el proyecto con el nuevo nombre |
| 0.2 ✅ | `git init`, `.gitignore` de Unity, `.gitattributes` para LFS | Yo | `git status` no muestra `Library/` ni `Temp/` |
| 0.3 ✅ | Instalar Git LFS y crear repo en GitHub, primer push | Tú | El repo está en GitHub |
| 0.4 | Carpetas `Assets/_Project/...` + asmdefs (`Core`, `Robot`, `Tests`) | Yo | Unity compila sin errores |
| 0.5 | Limpiar plantilla: carpeta `Welcome`, Visual Scripting (opcional) | Yo propongo, tú confirmas | Proyecto limpio, compila |
| 0.6 | Escena `Sandbox` con cámara cenital y arena con paredes | Yo (script de editor) / tú revisas | Abres la escena y ves la arena |

## Hito 1 — Robot conducible (1-2 semanas)

Empezamos por el **mayor riesgo técnico**: la física del robot.

| # | Tarea | Quién | Hecho cuando |
|---|---|---|---|
| 1.1 | **Prueba de física**: mismo robot con (A) joints y (B) un Rigidbody2D con colliders hijos | Yo | Hay dos robots en Sandbox |
| 1.2 | Probar ambos, empujarlos, chocarlos, y elegir | **Tú decides** | Elegimos A o B (recomiendo B) |
| 1.3 | `PartDefinition` (ScriptableObject) + `PartBehaviour` base | Yo | Puedes crear piezas desde el menú Create |
| 1.4 | `RobotData` / `PlacedPart` + serialización JSON + tests | Yo | Tests en verde en Test Runner |
| 1.5 | `RobotAssembler`: `RobotData` → robot en escena | Yo | Un robot hardcodeado aparece montado |
| 1.6 | 4 piezas: núcleo, bloque, rueda, propulsor (sprites = cuadrados de colores) | Yo | Se ven y tienen física |
| 1.7 | Control con teclado y mando (mover, girar, activar) | Yo | Lo conduces |
| 1.8 | Ajuste de feel (velocidad, fricción, masa) | Tú pruebas, yo ajusto | Te parece divertido moverlo |

## Hito 2 — Editor de montaje (2 semanas)

| # | Tarea | Quién |
|---|---|---|
| 2.1 | Rejilla 9x9 y colocar pieza con clic | Yo |
| 2.2 | Rotar / borrar / mover | Yo |
| 2.3 | Validación en vivo (núcleo único, conectividad, solapamiento) + colores rojo/verde | Yo |
| 2.4 | Inventario de piezas (UI) | Yo, tú das opinión de la UX |
| 2.5 | Guardar/cargar JSON | Yo |
| 2.6 | Botón "Probar" → lanza el robot a Sandbox | Yo |
| 2.7 | Montar 5 robots distintos y anotar qué molesta | **Tú** |

## Hitos siguientes (se detallarán al llegar)

- **Hito 3** — Piezas de combate y daño (pala, martillo, sierra, cañón; piezas que se desprenden).
- **Hito 4** — Bucle de partida local 2-4 jugadores → **primer playtest con amigos**.
- **Hito 5** — 3 minijuegos (Rey de la colina, Fútbol, Supervivencia).
- **Hito 6** — Contenido y pulido. Después: progresión, online y lanzamiento (opcionales).

---

## Lo que puedes hacer tú en paralelo (sin bloquearme)

**Ya, esta semana**
- [x] Tareas 0.1 y 0.3 (renombrar carpeta, GitHub + Git LFS).
- [ ] Repasar lo básico de Unity si no lo dominas: Rigidbody2D, prefabs, ScriptableObjects, Input System (1-2 h de vídeos bastan).
- [ ] Conseguir un mando (Xbox/PS) para probar multijugador desde el principio.

**Diseño (papel o un doc)**
- [ ] Lista de 20 piezas soñadas: nombre, qué hace, tamaño en celdas. Marca las 8 favoritas.
- [ ] Bocetos de 3 robots "graciosos" que te gustaría que se pudieran montar.
- [ ] Reglas de 3 minijuegos: cómo se gana, cuánto dura, cuántos puntos da.

**Arte y sonido** (no hace falta hasta el Hito 3, pero conviene decidir pronto)
- [ ] Elegir estilo: formas simples de colores vs. pack de assets (Kenney.nl es gratis y encaja).
- [ ] Buscar sonidos de choques, motores, explosiones (freesound.org, Kenney).

**Gente**
- [ ] Apuntar 3-4 amigos dispuestos a probar al acabar el Hito 4.

## Decisiones pendientes

| Decisión | Cuándo | Recomendación |
|---|---|---|
| Física: joints vs. cuerpo compuesto | Tarea 1.2 | Cuerpo compuesto (más estable) |
| UI: uGUI vs. UI Toolkit | Antes del Hito 2 | uGUI (drag & drop más sencillo, más tutoriales) |
| Nombre de la carpeta del proyecto | Ahora | `PartyBot` (sin espacios) |
