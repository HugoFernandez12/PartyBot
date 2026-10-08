# PartyBot — Hoja de ruta

## 1. Visión

Juego 2D cenital para 2-4 amigos. Partidas de 15-20 min, 5 rondas. En cada ronda los jugadores reciben piezas al azar, montan un robot **libremente en una rejilla** durante 60 s y compiten en un minijuego de arena. Gana quien acumule más puntos.

**Pilares de diseño**
1. Caos divertido: los robots torpes y las derrotas absurdas generan las risas.
2. Partidas cortas y rejugables.
3. Montaje libre pero legible: se entiende de un vistazo qué hace cada pieza.
4. Alcance pequeño: terminable por una sola persona.

## 2. Stack técnico

- Unity (LTS reciente), 2D, URP 2D.
- Física: Rigidbody2D + joints (FixedJoint2D / HingeJoint2D). Ver nota de riesgo en sección 6.
- Input: Input System nuevo, multijugador local (PlayerInput + varios mandos/teclado).
- UI: UI Toolkit o uGUI (decidir en Fase 1).
- Datos: ScriptableObjects para piezas y minijuegos.
- Guardado de robots: JSON (lista de celdas con pieza, posición y rotación).
- Control de versiones: Git + .gitignore de Unity, Git LFS para assets pesados.
- Online (opcional, al final): Netcode for GameObjects o Mirror.

## 3. Estructura de carpetas sugerida

```
Assets/
  _Project/
    Art/
    Audio/
    Data/            # ScriptableObjects (PartDefinition, MinigameDefinition)
    Prefabs/
      Parts/
      Robots/
    Scenes/
      Boot.unity
      Menu.unity
      Builder.unity
      Arena_*.unity
    Scripts/
      Core/          # GameManager, flujo de rondas, puntuación
      Parts/         # Part, PartDefinition, comportamientos
      Builder/       # rejilla, drag & drop, validación
      Robot/         # RobotAssembler, RobotController, serialización
      Minigames/
      UI/
      Input/
```

## 4. Modelo de datos clave

**PartDefinition (ScriptableObject)**: id, nombre, sprite, tamaño en celdas (forma), masa, vida, coste, puntos de conexión, tipo de comportamiento y parámetros (potencia, daño, alcance...).

**RobotData (serializable)**: lista de `PlacedPart { partId, cell(x,y), rotation }`. Es lo que se guarda, se valida y se instancia.

**Reglas de montaje (validación)**
- Debe haber exactamente un núcleo/chasis base.
- Todas las piezas deben estar conectadas por caras adyacentes al núcleo (flood fill).
- No hay solapamiento de celdas.
- Límite de tamaño de rejilla (ej. 9x9) y/o presupuesto de coste.

## 5. Fases

### Fase 0 — Preparación (1-2 días)
- [ ] Crear proyecto Unity 2D (URP), repo Git y .gitignore.
- [ ] Configurar estructura de carpetas, Input System y Assembly Definitions.
- [ ] Escena de pruebas con cámara cenital y una arena vacía.
- **Hecho cuando:** el proyecto abre, compila y está subido al repo.

### Fase 1 — Prototipo del robot (1-2 semanas)
- [ ] `PartDefinition` y prefab base de pieza.
- [ ] `RobotAssembler`: dado un `RobotData`, instancia piezas y las une con joints.
- [ ] Piezas mínimas: núcleo, bloque estructural, rueda motriz, propulsor.
- [ ] Control de un robot con un mando/teclado (cada pieza reacciona a un input).
- [ ] Robot hardcodeado conducible en una arena con paredes.
- **Hecho cuando:** conduces un robot montado por código sin que se descomponga ni tiemble.

### Fase 2 — Editor de montaje (2 semanas)
- [ ] Rejilla con celdas, inventario de piezas y drag & drop (o clic para colocar).
- [ ] Rotar, borrar y mover piezas.
- [ ] Validación en vivo (conectividad, solapamiento, límites) con feedback visual.
- [ ] Guardar/cargar `RobotData` en JSON.
- [ ] Botón "Probar" que lanza el robot a una arena de test.
- **Hecho cuando:** montas un robot desde cero, lo pruebas y lo guardas.

### Fase 3 — Piezas de combate y utilidad (1-2 semanas)
- [ ] Pala empujadora, brazo/martillo, sierra, cañón (proyectil).
- [ ] Sistema de vida y daño por pieza: las piezas se rompen y se desprenden.
- [ ] Efectos básicos (partículas, sonido, screen shake).
- [ ] Ajustar físicas y masas para que sea divertido, no realista.
- **Hecho cuando:** dos robots pueden pelearse y se rompen de forma legible.

### Fase 4 — Bucle de partida con multijugador local (2 semanas)
- [ ] Menú, lobby local (2-4 jugadores con PlayerInput).
- [ ] Flujo de ronda: reparto aleatorio de piezas, fase de montaje con temporizador, minijuego, puntuación.
- [ ] Reparto de piezas con reglas básicas (ej. 8 piezas por jugador, núcleo garantizado).
- [ ] Pantalla de resultados por ronda y final.
- **Hecho cuando:** 2+ amigos juegan una partida completa de 5 rondas en un mismo PC.

### Fase 5 — Minijuegos (2-3 semanas)
Un minijuego por vez, cada uno como `MinigameDefinition` con su escena, reglas y puntuación.
- [ ] Rey de la colina.
- [ ] Fútbol de robots.
- [ ] Supervivencia (arena que se encoge o con plataformas que caen).
- [ ] Carrera de obstáculos.
- [ ] Oleadas cooperativas contra enemigos.
- **Hecho cuando:** hay al menos 3 minijuegos jugables y rotan entre rondas.

### Fase 6 — Contenido y pulido (2-3 semanas)
- [ ] Ampliar piezas hasta 15-20 con variedad real.
- [ ] Arte coherente (pack o propio), audio y música.
- [ ] Tutorial rápido y UX del montaje (atajos, deshacer, tooltips).
- [ ] Balance con playtests reales (ver sección 7).
- [ ] Opciones: volumen, controles, resolución.

### Fase 7 — Progresión ligera (opcional, 1-2 semanas)
- [ ] Desbloqueo de piezas/cosméticos con el uso (guiño a Pokémon/Risk of Rain).
- [ ] Galería de robots guardados.

### Fase 8 — Online (opcional, solo si el juego ya es divertido)
- [ ] Elegir Netcode for GameObjects o Mirror.
- [ ] Sincronizar solo lo necesario: robots (RobotData) y estado de ronda; simular física en el host.
- [ ] Lobby con código de sala.

### Fase 9 — Lanzamiento
- [ ] Build para Windows (y Steam Deck si se quiere).
- [ ] Página en itch.io (o Steam), trailer corto y capturas.
- [ ] Recoger feedback y parchear.

## 6. Riesgos y mitigaciones

| Riesgo | Mitigación |
|---|---|
| Joints inestables (robots que tiemblan o explotan) | Probar pronto en Fase 1. Alternativa: un único Rigidbody2D con múltiples colliders hijos (compuesto) y las piezas "rotas" se separan creando un Rigidbody nuevo. Suele ser mucho más estable. |
| Montaje libre difícil de balancear | Presupuesto de coste, límite de rejilla, repartos aleatorios de piezas. |
| Alcance excesivo | Congelar el alcance tras Fase 5; todo lo demás es opcional. |
| Online rompe la física | Dejarlo para el final y simular en host. |
| Falta de arte | Pack de assets o formas simples con colores; estilo minimalista. |

## 7. Playtesting

- Primer playtest con amigos al acabar la Fase 4 (aunque sea feo).
- Preguntas: ¿se entiende el montaje?, ¿te ríes?, ¿cuánto dura cada ronda?, ¿qué pieza rompe el juego?
- Anotar todo en un documento de feedback y priorizarlo antes de añadir contenido.

## 8. Orden recomendado para trabajar con Claude Code

1. Fase 0: crear el proyecto y la estructura.
2. Fase 1, en este orden: `PartDefinition` -> `RobotData` -> `RobotAssembler` -> controlador de piezas.
3. Hacer commits pequeños por tarea y probar en el editor tras cada paso.
4. Mantener este archivo actualizado marcando las casillas completadas.

## 9. Definición de "terminado" (v1.0)

- 2-4 jugadores locales, partida de 5 rondas.
- Montaje libre en rejilla con 15+ piezas.
- 3+ minijuegos.
- Menú, ajustes, audio y build estable en Windows.
