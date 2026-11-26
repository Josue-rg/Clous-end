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
  tercera persona, mira con ratón y disparo hitscan.
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
| Desplazarse lateralmente | `A` / `D` o flechas izquierda/derecha |
| Subir / bajar (vuelo) | `Espacio` / `Ctrl izquierdo` o `Q` |
| Apuntar | Mover el ratón (la retícula sigue al cursor) |
| Disparar | Clic izquierdo |
| Girar la cámara | Clic derecho + arrastrar |
| Zoom | Rueda del ratón |
| Terminar la partida | Llegar a la línea del Refugio (z = 1450) |

Valores ajustables en el Inspector:

- `Player` → `VehicleMotor`: velocidades, aceleración, frenada, giro y modo vuelo
  (`flying`, `verticalSpeed`, `verticalAcceleration`).
- `Player Camera` → `ThirdPersonCamera`: distancia, alturas, sensibilidad, pitch, zoom y
  colisión de cámara.
- `CharacterPivot` → `CharacterAim`: velocidad de giro de la mira y alcance.
- `CharacterPivot` → `HitscanWeapon`: alcance, cadencia, automático y máscara de impacto.

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
| `MapPlayerBuilder.cs` | Cubo, cilindro, cámara, efectos de disparo y nieve. |
| `MapUIBuilder.cs` | HUD, cronómetro, aviso de checkpoint, paneles de resultados y game over. |
| `MapAssetFactory.cs` | Materiales, perfil de postproceso y sprite de retícula. |
| `MapBuildMenu.cs` | Entradas del menú `Clous-End > Mapa`. |

### Runtime (solo lee datos del Inspector)

| Archivo | Responsabilidad |
|---|---|
| `Assets/Scripts/Game/Input/PlayerInputReader.cs` | Única lectura de teclado y ratón. |
| `Assets/Scripts/Game/Player/VehicleMotor.cs` | Movimiento del vehículo con CharacterController. |
| `Assets/Scripts/Game/Player/CharacterAim.cs` | Giro del personaje y del arma hacia el ratón. |
| `Assets/Scripts/Game/Player/HitscanWeapon.cs` | Raycast, trazador e impacto. |
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

Todo el jugador de pruebas está en `Game > Player`:

| Objeto | Qué es | Sustituir por |
|---|---|---|
| `Player/PH_CarBody` (+ faros) | Cubo rojo del vehículo | `SM_Sleigh` de Blender |
| `Player/CharacterPivot/PH_Character` | Cilindro del personaje | `SK_Santa` de Blender |
| `Player/CharacterPivot/AimPivot/PH_Gun` | Arma de prueba | `SM_Carbon` de Blender |
| `Player Camera` | Cámara en tercera persona | Se conserva; ajustar distancia |
| `Effects/Tracer`, `Effects/Impact` | VFX de disparo | Materiales/VFX finales |
| `UI` | HUD de prueba | HUD final |

Al reemplazar los modelos, mantener los pivotes (`CharacterPivot`, `AimPivot`, `Muzzle`) y las
referencias del Inspector. El `CharacterController` y los scripts no cambian.

Los edificios se llaman `SM_Bldg_*`, los props `SM_*` y los materiales `M_Map_*`, siguiendo el
naming del documento 11.

---

## Limitaciones conocidas del blockout

- La geometría es de primitivas: no es arte final. El documento 11 prohíbe primitivas en la
  entrega final, no en el blockout.
- El vehículo de pruebas es un cubo que vuela; no es un trineo. La cámara y los controles
  están pensados para validar el mapa, no para el combate final.
- La misión completa (combate, renos, armas y boss) no está implementada. La demo sí tiene
  cronómetro, checkpoint, meta, resultados y game over.
- La retícula se oculta si el cursor sale de la ventana del juego; en ventana completa no pasa.
- El suelo es plano; el terreno definitivo (nieve, desniveles, río) llegará con el kit de
  Blender.
- La escena no tiene occlusion culling horneado. El streaming de zonas y los LODs ya mantienen
  el coste bajo; hornear oclusión es una optimización posterior.

---

## Siguiente paso

1. Sustituir el blockout por el primer kit de Blender (calle, edificio, esquina y puente),
   empezando por el centro histórico.
2. Montar el combate, los renos y las armas sobre `MissionManager` y el mapa actual.
3. Repetir la medición de draw calls con los modelos finales y comparar con este presupuesto.
4. Actualizar la demo web cuando haya una versión estable (ver doc 13).
