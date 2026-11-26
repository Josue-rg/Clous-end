# 15 · Combate, misión y jefe Nicolás Cero

> **Estado:** implementado en la escena `Game`: el jugador vuela, dispara con las
> 4 armas, defiende a los 4 renos, atraviesa 11 oleadas de los 5 enemigos, recoge
> los pickups y derrota a Nicolás Cero en 3 fases. La versión no incluye audio.
>
> Documentos relacionados: [09 Arquitectura](09_Arquitectura_Tecnica.md) ·
> [11 Alcance](11_Alcance_Final_Juego_Terminado.md) ·
> [12 Mapa](12_Mapa_Blockout_Game.md) · [14 Assets 3D](14_Assets_3D_y_Pipeline_VAT.md)

---

## 1. Flujo de una partida

```
Login · JUGAR
  ↓
Game · intro de radio + despegue (texto de Voss)
  ↓
Acto 1 · afueras (oleadas de Sabuesos y Esporas)
  ↓
Acto 2 · centro histórico (primer Garfio, rescate de reno)
  ↓
Checkpoint del puente · z=1000 · repara el casco
  ↓
Acto 3 · distrito financiero (Mantos, Torres, naves, Estrella)
  ↓
Acto 4 · Refugio Aurora · barrera, Nicolás Cero y renos de obsidiana
  ↓
Final · resultados, tiempo, renos, casco, boss + créditos
```

Un solo mapa, una sola misión. La demo recorre 1,5 km y se completa en pocos minutos.

---

## 2. Controles

| Acción | Tecla / ratón |
|---|---|
| Avanzar / retroceder | `W` / `S` o flechas arriba/abajo |
| Esquivar / desplazarse | `A` / `D` o flechas izquierda/derecha |
| Subir / bajar | `Espacio` / `Ctrl izquierdo` o `Q` |
| Apuntar | mover el ratón (la retícula es el puntero) |
| Disparar | clic izquierdo |
| Cambiar de arma | `1` Carbón · `2` Nogal · `3` Estrella · `4` Cascabel |
| Recargar | `R` |
| Girar la cámara | clic derecho + arrastrar |
| Zoom | rueda del ratón |
| Pausa | `Esc` |

El disparo sale por el **rayo que pasa por el puntero** (`ScreenPointToRay`), no por el
centro de la cámara. Se puede apuntar a los lados sin girar la cámara; el clic derecho
solo sirve para orbitar la vista.

---

## 3. Armas

Todas salen de la bolsa. No hay mejoras ni niveles. Los datos viven en
`Assets/Config/Weapons/W_*.asset` y se pueden ajustar en el Inspector.

| Arma | Tipo | Daño | Cadencia | Munición | Especial |
|---|---|---|---|---|---|
| **Carbón** | Fusil automático | 6 | 0,125 s (8/s) | Infinita (calor) | Se sobrecalienta si se mantiene el gatillo; 1,2 s de bloqueo. |
| **Nogal** | Escopeta | 11 × 6 perdigones | 0,7 s | 8 cartuchos | Dispersión de 6,5°; rompe escudos y enjambres. |
| **Estrella** | Railgun | 80 | 1,5 s | 3 cargas | Perfora hasta 5 enemigos en línea. |
| **Cascabel** | Cadena eléctrica | 22 + 55 % en cadena | 0,6 s | 6 cargas | Rebota a 4 enemigos en 18 m. |

- **Carbón** se obtiene al empezar; **Nogal** también.
- **Estrella** se recoge en el distrito financiero (z≈900).
- **Cascabel** se recoge antes del refugio (z≈1160).
- El HUD muestra calor o munición y las 4 ranuras.

---

## 4. Enemigos

Los 5 enemigos de la v1, más las naves y los renos de obsidiana del jefe.
Los datos viven en `Assets/Config/Enemies/E_*.asset`.

| Enemigo | Vida | Velocidad | Ataque | Comportamiento |
|---|---|---|---|---|
| **Sabueso** | 30 | 26 | 10 al trineo | Embiste tras 0,45 s de aviso; muere al impactar. |
| **Garfio** | 40 | 20 | Secuestra un reno | A 55 m engancha un reno; 5 s para matarlo y rescatarlo. |
| **Espora** | 18 | 22 | 16 al trineo | Kamikaze; explota al tocar el trineo. |
| **Manto** | 40 | 16 | 12 al trineo | Francotirador a 75 m con láser de aviso de 1,2 s. |
| **Torre** | 80 | 14 | 14 al trineo | Tanque lento que embiste; aguanta el fuego. |
| **Nave Caza** | 50 | 20 | 12 al trineo | Hover a 90 m; dispara tras telegraph. |
| **Nave Recolector** | 90 | 16 | 14 al trineo | Hover pesado a 95 m. |
| **Reno de obsidiana** | 65 | 26 | 18 al trineo | Embiste; solo aparece en la fase 1 del jefe. |

La escala visual se configura en `Assets/Config/Enemies/E_*.asset` (`modelScale`) y en
`BossController.modelScale`: Sabueso **4,2**; Espora **4,0**; Garfio/Manto/Torre **3,4**;
naves **3,6/3,2**; jefe **3,2**; reno de obsidiana **3,0**. Los materiales VAT aclaran el
color base (los enemigos venían casi negros) y suben `_Ambient`/`_Rim`; el mapa usa
exposición **+0,65** y saturación **+18**. La cámara del jugador está a **7 m** y Santa
gira como máximo **40°** con el cuerpo; el resto del giro lo hace el pivote del arma.
Cada enemigo lleva una **barra de vida** sobre el modelo (`WorldHealthBar`).

- **Pool sin instanciación en runtime:** las oleadas están precolocadas en la
  escena. El `EnemyDirector` las activa al acercarse y **ya no las desactiva al
  pasar**: siguen al jugador, se recolocan según hacia dónde mira y hacen zigzag,
  así que hay que matarlas.
- **Persecución:** todos los enemigos vuelan a una velocidad similar a la del
  trineo (45–52 m/s) y mantienen su formación relativa a la orientación del
  jugador (`anchorOffset` rotado), con una evasión lateral sinusoidal.
- Los enemigos tienen colliders trigger para que el trineo no se quede
  atascado contra ellos; los disparos sí los alcanzan.
- La muerte reproduce el clip VAT `Death` cuando el asset lo tiene
  (Sabueso, Garfio, Espora y reno); Manto, Torre y naves se apagan.

### Oleadas

| Z | Nombre | Contenido |
|---|---|---|
| 90 | Primera presión | 2 Sabuesos + 1 Espora |
| 200 | La jauría | 3 Sabuesos + 2 Esporas |
| 320 | El primer Garfio | Garfio + 2 Sabuesos + Espora |
| 440 | Francotiradores | 2 Mantos + 2 Sabuesos + Espora |
| 560 | Torre de guardia | Torre + 2 Sabuesos + Espora |
| 680 | Enjambre | 3 Esporas + Sabueso + Garfio |
| 800 | Naves | Nave Caza + 2 Mantos + 2 Esporas |
| 920 | El asedio | 2 Torres + Garfio + 2 Esporas + Sabueso |
| 1040 | Cazadores | 2 Naves Caza + 2 Mantos + Torre |
| 1160 | El recolector | Nave Recolector + 2 Garfios + 2 Esporas |
| 1280 | La última escuela | Recolector + 2 Cazas + 2 Mantos + Torre |

---

## 5. Los 4 renos son las 4 vidas

- `ReindeerSquad` controla los 4 renos visibles: Rodolfo, Donner, Vixen y Cometa.
- El **Garfio** elige un reno sano, lo oculta y abre una ventana de **5 s**.
- Si el jugador mata al Garfio dentro de la ventana, el reno vuelve y reproduce
  `Hurt`. Si el Garfio escapa, el reno se pierde.
- El **escudo de reno** revive a un reno perdido o absorbe la siguiente pérdida.
- Si se pierden los 4, la partida termina.
- El casco es independiente: 100 de integridad; a 0 también se pierde.

---

## 6. Pickups

| Z | Objeto | Efecto |
|---|---|---|
| 250 | Munición | Recarga todas las armas. |
| 380 | Kit de casco | +25 de casco. |
| 520 | Escudo de reno | Revive un reno perdido o protege al siguiente. |
| 640 | Munición | Recarga. |
| 760 | Kit de casco | +25. |
| 900 | Regalo de arma | Desbloquea la **Estrella**. |
| 1050 | Munición | Recarga. |
| 1160 | Regalo de arma | Desbloquea el **Cascabel**. |
| 1260 | Kit de casco | +25. |
| 1320 | Escudo de reno | Revive/protege. |

Los pickups flotan y giran; el `PickupCollector` los recoge por cercanía
(sin triggers), así que no bloquean el vuelo ni los disparos.

---

## 7. Checkpoint, game over y resultados

- **Checkpoint:** al cruzar z=1000 se guarda la posición, se repara el casco y se
  muestra el aviso.
- **Game over:** casco a 0, los 4 renos perdidos o caer por debajo del mapa.
- **Reintentar (game over):** vuelve al checkpoint, restaura casco y los 4 renos,
  reinicia las oleadas y el jefe, y **devuelve la posición y la cámara al frente**. El
  reintento restaura el estado base del checkpoint.
- **Resultados:** tiempo, renos vivos, casco restante y si el boss cayó.
- Los botones **REINTENTAR** y **LOBBY** funcionan; el panel incluye una línea de
  créditos.

---

## 8. Jefe: Nicolás Cero

- Aparece al cruzar **z=1340**; la barrera de energía de z=1440 cierra la salida.
- El modelo está en `BossArena` (z=1420); tiene **850** de integridad.
- **Fase 1 · Procesión:** invulnerable; activa dos **renos de obsidiana** (Vixen y
  Cometa teñidos). Hay que destruirlos para bajar el escudo.
- **Fase 2 · Reparto:** lanza **regalos-bomba** desde un pool de 8 y dispara
  voleas con láser de aviso. Las bombas se pueden disparar para desactivarlas.
- **Fase 3 · Cuerpo a cuerpo:** al 40 % de vida acelera bombas y voleas.
- Al morir abre la barrera y, 2,5 s después, termina la misión.
- Los renos de obsidiana usan el clip `Gallop`, `Hurt` y `Death` del reno y se
  tiñen con `VATTint` sin duplicar materiales.

---

## 9. HUD y pausa

- **Arriba izquierda:** 4 renos con estado (sano / agarrado / perdido) y objetivo.
- **Abajo izquierda:** casco con barra.
- **Abajo derecha:** arma, munición o calor y las 4 ranuras.
- **Arriba centro:** barra del jefe cuando entra en combate.
- **Sobre cada enemigo:** barra de vida roja; el jefe usa la barra superior del HUD.
- **Centro:** aviso de rescate con cuenta atrás.
- **Avisos:** recogidas, checkpoint, fases del jefe y pérdida/rescate.
- **Pausa (Esc):** REANUDAR, REINICIAR y LOBBY; congela el tiempo y libera el cursor.
- **Resultados:** tiempo, renos, casco, boss y créditos.

---

## 10. Implementación (archivos)

### Runtime

| Archivo | Responsabilidad |
|---|---|
| `Assets/Scripts/Game/Player/WeaponController.cs` | Las 4 armas, calor, munición, recarga, perforación y cadena. |
| `Assets/Scripts/Game/Player/WeaponVisualSwitcher.cs` | Muestra el modelo del arma equipada entre los cuatro del rig. |
| `Assets/Scripts/Game/Player/SleighHealth.cs` | Integridad del casco. |
| `Assets/Scripts/Game/Player/ReindeerSquad.cs` | 4 renos, agarre, rescate, pérdida y escudo. |
| `Assets/Scripts/Game/Combat/EnemyAgent.cs` | IA de los 5 enemigos, naves y renos de obsidiana. |
| `Assets/Scripts/Game/Combat/EnemyDefinition.cs` | Datos de cada enemigo. |
| `Assets/Scripts/Game/Combat/BossController.cs` | Fases, movimiento y ataques de Nicolás Cero. |
| `Assets/Scripts/Game/Combat/GiftBomb.cs` | Regalo-bomba del jefe. |
| `Assets/Scripts/Game/Combat/VATTint.cs` | Tinte de los renos de obsidiana. |
| `Assets/Scripts/Game/Combat/Damageable.cs` | `IDamageable` y resolución de impactos. |
| `Assets/Scripts/Game/World/EnemyDirector.cs` | Activación de oleadas por Z. |
| `Assets/Scripts/Game/World/Pickup.cs` · `PickupCollector.cs` · `PickupBob.cs` | Recogibles y flotación. |
| `Assets/Scripts/Game/World/BossGate.cs` | Barrera de la arena. |
| `Assets/Scripts/Game/UI/GameHud.cs` | HUD de combate. |
| `Assets/Scripts/Game/UI/WorldHealthBar.cs` | Barra de vida sobre cada enemigo. |
| `Assets/Scripts/Game/UI/PauseMenu.cs` | Pausa. |
| `Assets/Scripts/Rendering/VATClipSet.cs` | Clips VAT por asset para reproducir `Death`/`Hurt`/`Attack`. |
| `Assets/Scripts/Game/World/MissionManager.cs` | Misión, casco, renos, boss, checkpoint y resultados. |

### Editor

| Archivo | Responsabilidad |
|---|---|
| `Assets/Editor/Game/GameplayBuilder.cs` | Coloca armas, renos, oleadas, pickups, jefe, barrera y wiring. |
| `Assets/Editor/Game/GameHudBuilder.cs` | Construye el HUD de combate. |
| `Assets/Editor/VAT/VATPropBuilder.cs` | Arnés, cascabeles, collar y meteoritos del recorrido. |
| `Assets/Editor/VAT/VATTextureBuilder.cs` | Genera las texturas VAT `RGBAHalf` desde el volcado de Blender. |
| `Assets/Editor/Map/MapUIBuilder.cs` | HUD base, resultados, game over y pausa. |
| `Assets/Editor/Map/MapPlayerBuilder.cs` | Jugador, cámara y `WeaponController`. |
| `Assets/Editor/Map/MapSceneBuilder.cs` | Orquesta la escena completa. |

Los datos de armas y enemigos se crean/actualizan como assets en
`Assets/Config/Weapons` y `Assets/Config/Enemies`.

---

## 11. Notas de la versión

- **Audio:** la versión actual no incluye música, SFX ni voz.
- **Intro y final:** se presentan con textos y paneles de HUD.
- **Créditos:** línea de créditos en el panel de resultados.
- **Materiales:** color plano por material; sin texturas Albedo/Normal/ORM horneadas.
- **Armas en mano:** los 4 modelos están en el rig y cambian con 1–4; no tienen clips
  propios de disparo/recarga.
- **Dificultad:** una única dificultad, sin modos alternativos, aim assist ni subtítulos.
- **Texturas VAT:** 124 MB de assets (65 MB en GPU); el build web pesa 63 MB.
- **Plataforma:** build WebGL publicado; el proyecto se edita y prueba en Unity 6.6.
