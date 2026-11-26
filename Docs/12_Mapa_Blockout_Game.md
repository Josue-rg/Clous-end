# 12 · Mapa blockout: escena Game

> **Estado:** blockout jugable (fase F1). La geometría usa primitivas de Unity a propósito:
> es el blockout temporal que servirá para medir la ruta, la cámara y el rendimiento antes
> del kit modular de Blender. La escena final sustituirá estas piezas por módulos de Blender.

---

## Qué hay hecho

- Escena **`Assets/Scenes/Game.unity`**, con el Distrito Aurora completo en blockout.
- Ruta continua de **1,5 km** dividida en las 4 zonas del documento 11: afueras, centro
  histórico, distrito financiero y refugio.
- Vehículo y personaje de pruebas (un cubo y un cilindro) con movimiento, vuelo, cámara en
  tercera persona, mira con ratón y disparo hitscan. Si el paquete VAT está importado, el
  cubo y el cilindro se sustituyen por el trineo, Santa y los cuatro renos.
- **Combate completo** (doc 15): 4 armas, 5 enemigos, 9 oleadas, rescate de renos,
  pickups, Nicolás Cero en 3 fases, barrera de arena, HUD y pausa.
- **Checkpoint** en el puente que repara el casco; game over por casco, renos o caída;
  resultados con tiempo, renos, casco y boss.
- HUD de pruebas con retícula, controles, nombre de zona, cronómetro y paneles de misión.
- Checkpoint en el puente, meta en el Refugio y game over con reintento.
- Sistema de rendimiento por capas: LODs con crossfade, apagado de zonas lejanas, niebla de
  distancia y props fusionados por material.
- **Demo web publicada:** https://josue-rg.github.io/Clous-end-web/ (ver doc 13).

---

## Cómo abrir y reconstruir

| Acción | Menú |
|---|---|
| Construir o reconstruir el mapa | `Clous-End > Mapa > Construir escena Game (blockout)` |
| Abrir la escena | `Clous-End > Mapa > Abrir escena Game` |
| Probar directamente | `Clous-End > Mapa > Entrar en Play Mode` |
| Importar los modelos de Blender | `Clous-End > VAT > Importar todo (paquete + prefabs)` |
| Construir la escena con combate y jefe | `Clous-End > Mapa > Construir escena Game (blockout)` |
| Integrar solo los modelos en Game sin rehacer el mapa | `Clous-End > VAT > 4 · Integrar modelos en la escena Game` |

La escena se guarda en disco. Una vez construida se puede editar a mano (mover edificios,
cambiar materiales, añadir props) y los cambios se conservan. La reconstrucción desde el menú
**reemplaza** la escena, así que conviene duplicarla o hacer commit antes de reconstruir.

`Game.unity` ya está en Build Settings, después de `Login`. El botón **JUGAR** del lobby carga
`Game`. La demo web usa estas dos escenas; el proceso de publicación está en
[13 · Demo web](13_Web_Demo_GitHub_Pages.md).

---

## Controles de la prueba

| Acción | Tecla / ratón |
|---|---|
| Avanzar / retroceder | `W` / `S` o flechas arriba/abajo |
| Esquivar / desplazarse lateralmente | `A` / `D` o flechas izquierda/derecha |
| Subir / bajar (vuelo) | `Espacio` / `Ctrl izquierdo` o `Q` |
| Apuntar | Mover el ratón (la retícula sigue al cursor) |
| Disparar | Clic izquierdo |
| Cambiar de arma | `1` Carbón · `2` Nogal · `3` Estrella · `4` Cascabel |
| Recargar | `R` |
| Girar la cámara | Clic derecho + arrastrar |
| Zoom | Rueda del ratón |
| Pausa | `Esc` |
| Terminar la partida | Derrotar a Nicolás Cero (z≈1420) |

Valores ajustables en el Inspector:

- `Player` → `VehicleMotor`: velocidades, aceleración, frenada, giro y modo vuelo
  (`flying`, `verticalSpeed`, `verticalAcceleration`).
- `Player Camera` → `ThirdPersonCamera`: distancia **7 m**, offset **1,8**, pitch **12°**,
  sensibilidad, zoom y colisión de cámara.
- `CharacterPivot` → `CharacterAim`: velocidad de giro de la mira, alcance y **límite de
  giro del cuerpo (40°)**.
- `CharacterPivot` → `HitscanWeapon`: alcance, cadencia, automático y máscara de impacto.

---

## Combate y misión (resumen)

El detalle completo está en [15 · Combate, misión y jefe](15_Combate_Mision_y_Jefe.md).

- **Armas:** Carbón (automática y calor), Nogal (escopeta), Estrella (perfora) y
  Cascabel (cadena). Se cambian con 1–4; R recarga.
- **Enemigos:** Sabueso, Garfio, Espora, Manto y Torre, más las naves Caza y
  Recolector y los renos de obsidiana del jefe. 9 oleadas precolocadas.
- **Renos:** son las 4 vidas. El Garfio puede agarrar uno; hay 5 s para matarlo y
  rescatarlo. El escudo de reno revive o protege.
- **Casco:** 100 de integridad; a 0 se pierde. El checkpoint del puente lo repara.
- **Pickups:** munición, kits, escudos y las armas Estrella (z≈900) y Cascabel
  (z≈1160).
- **Jefe:** Nicolás Cero en z≈1420, con barrera en z=1440. Fase 1 (renos de
  obsidiana), fase 2 (bombas y voleas) y fase 3 (al 40 %).
- **Final:** al morir el jefe, se abre la barrera y aparecen los resultados.

---

## Layout del mapa

```
Z = 0                        Z = 340               Z = 820          Z = 1180        Z = 1500
│  AFUERAS / AUTOPISTA        │  CENTRO HISTÓRICO    │  FINANCIERO     │  REFUGIO AURORA  │
│  naves industriales         │  plaza + iglesia     │  torres, cables  │  cúpula, faro    │
│  camiones, pinos            │  árbol de Navidad    │  checkpoint      │  campamento      │
```

- **Calzada:** 26 m de ancho, 3 carriles por sentido, con aceras de 6 m a cada lado.
- **Línea de fachada:** x = ±19 m.
- **Suelo jugable:** 1.400 × 2.500 m, con bancos de nieve y una cordillera lateral.
- **Inicio:** el jugador aparece en `(0, 9, 8)`, flotando sobre la calle, y cruza la puerta
  de entrada en z = 26. Puede subir por encima de los tejados o bajar a la calzada.

### Hitos visuales

| Zona | Hito | Dónde |
|---|---|---|
| Afueras | Puerta de entrada + camión volcado + espina Yerma | z = 26 / 206 / 196 |
| Centro | Árbol de Navidad de la plaza + iglesia con torre y rosetón | (-30, 556) / (-56, 690) |
| Financiero | Pasarelas, cartel alienígena, grúa y **checkpoint del puente** | z = 900 / 1072 / 1000 |
| Refugio | Puerta de la cúpula, anillo luminoso, faro y agujas Yermas | z = 1620 / 1500 / 1760 |

---

## Sistema de rendimiento

El mapa usa cinco capas para que no se renderice lo que no se ve:

1. **Fog de distancia.** Niebla exponencial (densidad 0,0024). A 760 m un objeto conserva
   menos del 4 % de visibilidad, así que su desaparición no se nota.
2. **ZoneStreamer.** Cada zona es un root independiente con unos límites serializados. Si el
   jugador se aleja más de `cullDistance` (760 m por defecto) del *bounds* de la zona, se
   desactiva entera: renderers, colliders y LODs. Se vuelve a activar al acercarse.
3. **LOD Groups por edificio.** LOD0 = volumen + cubierta + detalles; LOD1 = solo volumen;
   LOD2 = culled. `fadeMode = CrossFade` para que el cambio no sea un salto. Hay **137 LOD
   Groups** en la escena.
4. **Props fusionados.** Todos los props estáticos de una zona se combinan por material en el
   editor (`MapPropCombiner`), conservando sus colliders. Cientos de renderers pasan a ~10
   draw calls por zona.
5. **Ventanas.** Las ventanas encendidas de toda una zona se combinan en **una sola malla** por
   material (`WindowMeshBuilder`), así que toda la iluminación de fachadas cuesta 1–2 draw
   calls por zona.

### Medición del blockout

Con el jugador al inicio y dos zonas activas (medición del blockout, antes de añadir los
paneles de misión):

| Métrica | Valor |
|---|---|
| Draw calls | **624** |
| Triángulos | **523.038** |
| SetPass calls | 73 |
| Luces en tiempo real | 5 (1 direccional + 4 puntuales) |

Objetivos del documento 11: <1.200 draw calls y <1,5 M de triángulos. El blockout está dentro
del presupuesto, con margen para los modelos finales de Blender.

---

## Arquitectura

### Editor (construcción una sola vez)

| Archivo | Responsabilidad |
|---|---|
| `Assets/Editor/Map/MapSceneBuilder.cs` | Orquesta la escena completa y la guarda. |
| `MapZoneAfueras.cs`, `MapZoneCentro.cs`, `MapZoneFinanciero.cs`, `MapZoneRefugio.cs` | Contenido de cada zona. |
| `MapBuildingFactory.cs` | Edificios con LODs, cubiertas, ventanas y chimeneas. |
| `MapPropsFactory.cs` | Farolas, coches, camiones, vallas, árboles, contenedores, cables... |
| `MapZoneCommon.cs` | Hileras de edificios, farolas y nieve repetidas. |
| `MapPropCombiner.cs` | Fusión de props por material y conservación de colliders. |
| `MapMeshFactory.cs` | Mallas reutilizables y mallas de ventanas por zona. |
| `MapGroundBuilder.cs` | Suelo, calzada, aceras, líneas y bancos de nieve. |
| `MapBackdropBuilder.cs` | Montañas y skyline lejano. |
| `MapLightingBuilder.cs` | Luna, luces prácticas, niebla y postproceso. |
| `MapPlayerBuilder.cs` | Rig del jugador (modelos VAT o placeholders), cámara, efectos de disparo y nieve. |
| `GameplayBuilder.cs` | Armas, renos, 9 oleadas, pickups, jefe, barrera y wiring. |
| `GameHudBuilder.cs` | HUD de combate (casco, renos, armas, jefe, rescate). |
| `MapUIBuilder.cs` | HUD base, cronómetro, resultados, game over y pausa. |
| `MapAssetFactory.cs` | Materiales, perfil de postproceso y sprite de retícula. |
| `MapBuildMenu.cs` | Entradas del menú `Clous-End > Mapa`. |

### Runtime (solo lee datos del Inspector)

| Archivo | Responsabilidad |
|---|---|
| `Assets/Scripts/Game/Input/PlayerInputReader.cs` | Única lectura de teclado y ratón. |
| `Assets/Scripts/Game/Player/VehicleMotor.cs` | Movimiento del vehículo con CharacterController. |
| `Assets/Scripts/Game/Player/CharacterAim.cs` | Giro del personaje y del arma hacia el ratón. |
| `Assets/Scripts/Game/Player/WeaponController.cs` | Las 4 armas: calor, munición, recarga, perforación y cadena. |
| `Assets/Scripts/Game/Player/SleighHealth.cs` | Casco, daño y reparación. |
| `Assets/Scripts/Game/Player/ReindeerSquad.cs` | 4 renos, agarre, rescate y pérdida. |
| `Assets/Scripts/Game/Combat/EnemyAgent.cs` | IA de los enemigos y naves. |
| `Assets/Scripts/Game/Combat/BossController.cs` | Nicolás Cero y sus fases. |
| `Assets/Scripts/Game/World/EnemyDirector.cs` | Activación de oleadas por Z. |
| `Assets/Scripts/Game/World/PickupCollector.cs` | Recogida de pickups. |
| `Assets/Scripts/Game/UI/GameHud.cs` | HUD de combate. |
| `Assets/Scripts/Game/UI/PauseMenu.cs` | Pausa y reanudación. |
| `Assets/Scripts/Game/Camera/ThirdPersonCamera.cs` | Órbita, zoom y colisión de cámara. |
| `Assets/Scripts/Game/FX/TracerBeam.cs` | Trazador reutilizable (LineRenderer de escena). |
| `Assets/Scripts/Game/FX/ImpactFlash.cs` | Impacto reutilizable (malla de escena). |
| `Assets/Scripts/Game/FX/SnowFollower.cs` | La nieve sigue al jugador sin mover sus partículas. |
| `Assets/Scripts/Game/World/ZoneStreamer.cs` | Enciende y apaga zonas por distancia. |
| `Assets/Scripts/Game/World/MissionManager.cs` | Cronómetro, checkpoint, meta, game over y reintento. |
| `Assets/Scripts/Game/UI/PointerReticle.cs` | La retícula sigue al ratón. |
| `Assets/Scripts/Game/UI/ZoneTitleHud.cs` | Nombre de zona con desvanecido. |

No hay `new GameObject()` en runtime, ni `FindObjectOfType`, ni instanciación de proyectiles:
el trazador y el impacto ya existen en la escena y se reutilizan.

---

## Colisiones

- Los edificios tienen collider en el volumen y, los históricos, también en la pirámide de la
  cubierta. La iglesia tiene collider propio para la nave y el tejado.
- Las pasarelas, la viga del checkpoint, la puerta de entrada, el cartel, la grúa, el faro y el
  arco de meta tienen colliders. Los props fusionados conservan los suyos.
- Los pinos tienen un collider de tronco; la nieve y los bancos son decorativos.
- El `CharacterController` usa radio 1,9 m para que el cubo de pruebas no se meta en las paredes.

## Meta, checkpoint y fin de partida

- **Checkpoint:** al cruzar el puente (z = 1000) se guarda el punto de reaparición en el carril,
  justo después del puente. Un aviso **CHECKPOINT** aparece y se desvanece.
- **Meta:** la línea **z = 1450**, marcada con un arco y una franja luminosa en el Refugio.
- `MissionManager` compara la posición del jugador (funciona a pie, volando o a gran velocidad).
  Lleva el cronómetro visible arriba a la derecha.
- **Al cruzar la meta:** se muestran **MISIÓN COMPLETADA**, el tiempo, los renos vivos
  (4 / 4 como base) y los botones **REINTENTAR** (nueva partida) y **LOBBY**.
- **Game over:** si el trineo cae por debajo del mapa, aparece **PARTIDA TERMINADA** con
  **REINTENTAR** (vuelve al último checkpoint) y **LOBBY**. Los botones usan el EventSystem
  del juego y el cursor se libera solo en esos paneles.

## Dónde se sustituyen los placeholders

El jugador de pruebas está en `Game > Player`. Desde que existe el paquete VAT,
la reconstrucción del mapa ya monta el arte de Blender si los prefabs existen; y
`Clous-End > VAT > 4 · Integrar modelos en la escena Game` hace el cambio en una
escena ya construida, sin regenerar el mapa.

| Objeto | Qué era | Estado |
|---|---|---|
| `Player/Sleigh` | `PH_CarBody` (cubo rojo) | Sustituido por `PF_Sleigh` (VAT Flight) |
| `Player/CharacterPivot/Santa` | `PH_Character` (cilindro) | Sustituido por `PF_Santa` (VAT Idle) |
| `Player/ReindeerSquad` | No existía | Cuatro renos: Rodolfo, Donner, Vixen y Cometa |
| `Player/CharacterPivot/AimPivot/PH_Gun` | Arma de prueba | **Sustituido** por un `WeaponRack` con los 4 modelos; `WeaponVisualSwitcher` muestra la equipada |
| `Player Camera` | Cámara en tercera persona | Se conserva; ajustar distancia |
| `Effects/Tracer`, `Effects/Impact` | VFX de disparo | Materiales/VFX finales |
| `UI` | HUD de prueba | HUD final |

El rig incluye también `Player/VATProps` (arnés, cascabeles y collar) y
`Game/VATProps` (meteoritos caídos como decoración del recorrido).

Al reemplazar los modelos se mantienen los pivotes (`CharacterPivot`, `AimPivot`,
`Muzzle`), el `CharacterController`, la cámara y las referencias del Inspector.
Los offsets del rig (altura del trineo y hueco de Santa) son configuraciones del
editor en `VATRigBuilder` y se pueden retocar a mano en la escena.

Los edificios se llaman `SM_Bldg_*`, los props `SM_*` y los materiales `M_Map_*`, siguiendo el
naming del documento 11.

---

## Limitaciones conocidas del blockout

- La geometría es de primitivas: no es arte final. El documento 11 prohíbe primitivas en la
  entrega final, no en el blockout.
- El vehículo de pruebas es un cubo que vuela; no es un trineo. Cuando el paquete
  VAT está importado, la escena usa el trineo, Santa y los cuatro renos reales.
- La misión completa (combate, oleadas, renos, armas y boss) **está implementada**;
  falta probarla de principio a fin y ajustar dificultad, cámara y tiempos.
- No hay audio real, intro cinematográfica ni pantalla de créditos completa.
- Los edificios y props siguen siendo blockout; el kit modular de Blender está pendiente.
- La retícula se oculta si el cursor sale de la ventana del juego; en ventana completa no pasa.
- El suelo es plano; el terreno definitivo (nieve, desniveles, río) llegará con el kit de
  Blender.
- La escena no tiene occlusion culling horneado. El streaming de zonas y los LODs ya mantienen
  el coste bajo; hornear oclusión es una optimización posterior.

---

## Siguiente paso

1. **Probar la misión completa** desde `Login > JUGAR` con los modelos reales y ajustar dificultad,
   oleadas, daño y tiempos con los assets de `Assets/Config`.
2. Sustituir el blockout por el kit modular de Blender (calle, edificio, esquina y
   puente), empezando por el centro histórico.
3. Añadir audio, intro/final cinematográficos y créditos completos.
4. Hornear Albedo/Normal/ORM para que los modelos se vean como en Blender.
5. Repetir la medición de draw calls con el arte final y el combate activo.
