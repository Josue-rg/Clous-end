# 11 · Alcance final: juego terminado

> **Este documento manda.** Si otro documento contradice el alcance de aquí, gana este. Los documentos 01–10 conservan ideas y detalle de diseño; todo lo que hable de múltiples capítulos, mapas, niveles, monedas o mejoras queda como **idea futura**, no como parte de esta versión.

---

## Decisión de alcance

CLOUS-END será un **juego corto, lineal y completo**, no una campaña grande.

- **Un solo mapa.**
- **Una sola misión** de principio a fin.
- **Un solo boss final.**
- **Sin sistema de niveles, XP, monedas, mejoras, inventario ni árbol de habilidades.**
- **Todos los assets 3D se modelan y animan en Blender**: mapa, Santa, trineo, renos, naves, enemigos, armas, props y pickups.
- La UI/2D y el audio se hacen fuera de Blender (Figma/Photoshop, DAW/librerías), porque no son 3D.

La prioridad no es la cantidad: es que la historia se entienda, que volar y disparar se sienta bien y que el jugador llegue al final con ganas de repetirlo.

---

## Ficha final

| Campo | Decisión |
|---|---|
| Nombre | CLOUS-END |
| Género | Shooter 3D de defensa sobre trineo, on-rails con movimiento lateral |
| Cámara | Tercera persona detrás del trineo |
| Jugadores | 1 |
| Duración | 20–30 minutos |
| Estructura | Login → intro → misión única → boss → final → créditos |
| Mapas | 1: Distrito Aurora (ciudad y refugio) |
| Renos | 4, cada uno una vida |
| Armas | 4 |
| Enemigos | 5 tipos |
| Boss | 1: Nicolás Cero |
| Naves alienígenas | 3 tipos (caza, recolector, nave nodriza de fondo) |
| Pantallas | Login, HUD, pausa, resultados/créditos |
| Plataforma | Windows x64, teclado/ratón y mando |
| Motor | Unity 6.6 · URP |
| Arte 3D | 100 % Blender |
| Arte 2D/UI | Figma/Photoshop |
| Audio | Música, SFX y voz de radio |
| Guardado | Solo perfil local; la misión no se guarda entre sesiones |
| Checkpoint | Uno, en el puente de la ciudad; se pierde al cerrar el juego |

---

## Estado de partida

| Entregable | Estado |
|---|---|
| Documentación y alcance congelado | Hecho |
| Login con botón JUGAR y diorama (sin credenciales) | Hecho |
| Reglas de trabajo (`AGENTS.md`) | Hecho |
| Escena `Game` y blockout del mapa | Hecho (blockout jugable, ver doc 12) |
| Kit del mapa en Blender | Pendiente |
| Santa, trineo y 4 renos en Blender | **Hecho** (modelo + rig + animación, ver doc 14) |
| Armas, enemigos, naves y boss en Blender | **Hecho** (ver doc 14) |
| Rig y clips de todos los assets animados | **Hecho** (VAT, ver doc 14) |
| LODs y mallas de colisión | **Hecho** (14 assets animados) |
| Paquete exportado y listo para importar a Unity | **Hecho** (`BlenderSource/VAT/`, doc 14) |
| Importador a Unity: materiales, prefabs, LODs y colliders | **Hecho y ejecutado en el proyecto principal** (28 prefabs, 375 materiales) |
| Revisión del paquete en escena (`VAT_Preview`) | **Hecho**; la escena se genera desde el menú |
| Texturas VAT float para Unity | **Hecho** (assets `RGBAHalf` generados; el importador EXR de Unity corrompe los valores) |
| Texturas PBR horneadas (Albedo/Normal/ORM) | Pendiente |
| Combate, oleadas y rescate de renos | **Hecho** (doc 15) |
| Checkpoint del puente, cronómetro, resultados y game over de la demo | **Hecho** |
| HUD final, pausa y créditos | **Hecho** (HUD y pausa; créditos básicos) |
| Audio, VFX y voz | VFX básicos hechos (VAT, impactos, trazadores, nieve); audio y voz pendientes |
| Demo web en GitHub Pages | Hecho (ver doc 13) |
| QA, rendimiento y build de Windows | Pendiente |

---

## Qué significa “juego terminado”

El juego está terminado cuando una persona puede:

1. Abrir el juego.
2. Pulsar **JUGAR** en el lobby.
3. Ver una intro corta que explica la invasión y el objetivo.
4. Volar por el único mapa durante 20–30 minutos.
5. Disparar con 4 armas, esquivar y proteger a los 4 renos.
6. Llegar al Refugio Aurora.
7. Derrotar a Nicolás Cero.
8. Ver un final y unos créditos.
9. Volver al lobby o cerrar el juego sin errores.

Y todo eso debe funcionar **sin placeholders visibles**, a 60 FPS en un PC medio, con el mapa y los personajes hechos en Blender.

---

## Bucle de juego final

```
Lobby (Login)
  ↓
Intro de radio + despegue (cinemática corta, saltables)
  ↓
Misión única: Distrito Aurora
  ├─ Acto 1: afueras / autopista helada (tutorial de vuelo y disparo)
  ├─ Acto 2: centro histórico (primer robo de reno, civiles, Garfios)
  ├─ Acto 3: distrito financiero (presión, naves, mini-set piece del puente)
  └─ Acto 4: Refugio Aurora (boss Nicolás Cero)
  ↓
Final + créditos
```

No hay selección de nivel. No hay pantalla de mapa. No hay “capítulo 2”. Los actos son **beats narrativos dentro del mismo mapa**, no escenas separadas.

### Reglas del bucle
- El trineo avanza solo; el jugador controla lateral/vertical, apunta y dispara.
- El jugador protege dos cosas: el casco del trineo y los 4 renos.
- Los renos son las 4 vidas. Si se pierden los 4, el trineo cae.
- Hay un checkpoint en el puente para no repetir 20 minutos.
- No hay monedas, tienda ni mejoras. Las armas se obtienen en momentos de la historia.
- El final no depende de tener mejoras: depende de llegar vivo y derrotar al boss.

---

## Contenido final (lista cerrada)

| Contenido | Cantidad | Nota |
|---|---|---|
| Mapas | 1 | Distrito Aurora, continuo, sin cargas |
| Zonas del mapa | 4 | Afueras, centro, financiero, refugio |
| Santa | 1 | Modelo, rig y animaciones en Blender |
| Trineo | 1 | Modelo, rig de suspensión y animaciones en Blender |
| Renos | 4 | Rodolfo, Donner, Vixen, Cometa; rig compartido + variantes |
| Arnés / cuerdas | 1 set | Blender |
| Armas | 4 | Carbón, Nogal, Estrella, Cascabel |
| Enemigos comunes | 5 | Sabueso, Garfio, Espora, Manto, Torre |
| Boss | 1 | Nicolás Cero + 2 renos de obsidiana |
| Naves alienígenas | 3 | Caza, Recolector, Aguja/Ojo (fondo) |
| Pickups | 4 | Munición, kit de casco, escudo de reno, arma de historia |
| Pantallas | 4 | Login, HUD, pausa, resultados/créditos |
| Intro / final | 2 | Cinemáticas cortas con radio y cámara |
| Voces | 2 | Santa (pocas líneas) y Dra. Voss (radio) |
| Música | 4 temas | Menú, vuelo, boss, final |
| SFX | 1 librería | Armas, trineo, renos, aliens, entorno |

### Las 4 armas finales

| Arma | Rol | Munición | Se obtiene |
|---|---|---|---|
| **Carbón** | Fusil automático, calor | Infinita | Inicio |
| **Nogal** | Escopeta, rompe escudos | 8 cartuchos | Inicio |
| **Estrella** | Railgun, perfora | 3 cargas | Acto 3, en un camión caído |
| **Cascabel** | Cadena eléctrica | 6 cargas | Antes del refugio |

No hay mejoras de armas. No hay niveles de arma. No hay tienda.

### Los 5 enemigos finales

| Enemigo | Función | Ataque |
|---|---|---|
| **Sabueso** | Presión básica | Embiste al trineo |
| **Garfio** | Roba renos | Engancha y tira; ventana de rescate |
| **Espora** | Kamikaze | Se pega y explota |
| **Manto** | Francotirador | Láser de aviso y disparo |
| **Torre** | Escudo | Protege a otros y embiste |

El resto del bestiario (Coro, Tejedor, Ojo, El Recolector, La Catedral, El Convoy) queda como idea para una secuela. **No se modela en v1.**

### El boss final

**Nicolás Cero**, el anti-Santa fabricado por los Yermos. Un modelo único en Blender + 2 renos de obsidiana. Tres fases:

1. **Procesión:** los renos de obsidiana atacan; hay que destruirlos.
2. **Reparto:** lanza regalos-bomba; esquivar y devolver.
3. **Cuerpo a cuerpo:** en la cúpula del refugio; solo queda el Carbón y lo que quede en la bolsa.

No hay bosses intermedios. El Recolector no aparece en v1.

---

## El único mapa: Distrito Aurora

### Concepto
Una sola ruta continua desde las afueras de la ciudad hasta el Refugio Aurora. El jugador nunca ve una pantalla de carga: el mapa se construye como un corredor cinematográfico con cuatro zonas que se encadenan.

### Zonas
| Zona | Fantasía | Set pieces |
|---|---|---|
| **Afueras / autopista** | Salir del Polo Norte y ver la ciudad arder | Camiones, señales, primera nave |
| **Centro histórico** | La Navidad que aún respira | Plaza, iglesia, árbol, civiles, Garfio |
| **Distrito financiero** | La invasión a escala | Rascacielos, puente, cables, naves |
| **Refugio Aurora** | La última luz | Cúpula, faro, boss, final |

### Cómo se hace el mapa
El mapa se modela en **Blender** como un **kit modular**:

- Módulos de calle, acera, edificios, tejados, puente, túnel, plaza, refugio.
- Props: farolas, coches, vallas, árboles, bancos, carteles, escombros, cables, nieve.
- Un `.blend` maestro por zona y exports FBX por módulo.
- El montaje final se hace en **una sola escena de Unity** usando esos módulos.
- No se usan primitivas de Unity en el mapa final. Las primitivas solo se permiten en el blockout temporal.
- No se usan assets 3D de Asset Store. Todo el 3D es Blender.

Esto **sigue cumpliendo “el mapa se hace en Blender”**: toda la geometría, los edificios, el terreno y los props nacen en Blender; Unity solo coloca esos módulos en la escena. No se modela nada del mapa final con primitivas de Unity.

**Recomendación:** no exportar todo el mapa como un único FBX gigante. Un kit modular hecho en Blender se ve igual de “hecho en Blender”, pero permite LODs, occlusion, mejor rendimiento y arreglos sin reexportar 3 km de ciudad. Si se prefiere un FBX único, se puede, pero el coste de rendimiento y de iteración es mucho mayor.

### Medidas y rendimiento
| Elemento | Objetivo |
|---|---|
| Longitud de ruta | 2,5–3,5 km de vuelo |
| Duración de la misión | 20–30 min con combate |
| Módulos de edificio | 12–20 piezas reutilizables |
| Props únicos | 30–40 |
| Triángulos visibles | < 1,5 M |
| Draw calls | < 1.200 |
| Texturas | 2K héroes, 1K arquitectura, 512 props |
| Luces en tiempo real | 1 direccional + 4 puntuales |
| Colisiones | Corredor de vuelo y obstáculos grandes; no física completa |
| Carga | Una sola escena; sin streaming |

### Colisiones
- El trineo vuela dentro de un corredor invisible.
- Los edificios y obstáculos tienen colliders simples (cajas/cápsulas), no mallas complejas.
- Los elementos decorativos no colisionan.
- El suelo es un plano de colisión bajo el corredor.

---

## Todos los assets 3D en Blender

> **Regla:** ningún modelo 3D final se compra, se descarga ni se modela con primitivas de Unity. Todo se modela, se texturiza y se anima en Blender. La única excepción son los placeholders temporales de blockout, que se eliminan antes de la entrega.

### Personajes y vehículos

| Asset | Tipo | Rig | Animaciones mínimas | Presupuesto |
|---|---|---|---|---|
| **Santa** | Personaje | Humanoid + huesos de abrigo/barba | Idle, inclinación, disparo, recarga, impacto, mirar atrás, agarrar bolsa, victoria, derrota | 60–80 k tris |
| **Trineo** | Vehículo | Suspensión, patines, bolsa, riendas | Vuelo, inclinación, derrape, turbo, impacto, caída | 80–120 k tris |
| **Reno base** | Personaje | Cuadrúpedo | Galope, trote, giro, salto, herido, enganchado, caída, muerte | 35 k tris |
| **Rodolfo** | Variante | Comparte rig | Ídem + nariz | 35 k tris |
| **Donner** | Variante | Comparte rig | Ídem | 35 k tris |
| **Vixen** | Variante | Comparte rig | Ídem | 35 k tris |
| **Cometa** | Variante | Comparte rig | Ídem | 35 k tris |
| **Arnés y cuerdas** | Props | Huesos/rieles | Tensión, enganche, rotura | 5–10 k tris |
| **Nicolás Cero** | Boss | Rig único | Idle, ataque, daño, fase, muerte | 100–150 k tris |
| **Reno de obsidiana** | Enemigo | Rig simple | Vuelo, ataque, muerte | 20 k tris |

Los 4 renos comparten esqueleto y animaciones base; se diferencian por proporciones, astas, materiales, marcas de arnés y detalles. Así se mantiene la calidad sin multiplicar el trabajo por cuatro.

### Enemigos y naves

| Asset | Tipo | Rig | Animaciones | Presupuesto |
|---|---|---|---|---|
| **Sabueso** | Enemigo | Simple | Idle, correr, atacar, muerte | 8 k tris |
| **Garfio** | Enemigo | Brazo-cable | Idle, enganchar, tirar, soltar, muerte | 12 k tris |
| **Espora** | Enemigo | Ninguno | Flotar, hincharse, explotar | 4 k tris |
| **Manto** | Enemigo | Simple | Idle, apuntar, disparar, muerte | 10 k tris |
| **Torre** | Enemigo | Ninguno | Idle, embestir, escudo, muerte | 10 k tris |
| **Nave caza** | Nave | Ninguno | Vuelo, ataque, explosión | 15 k tris |
| **Nave recolector** | Nave | Ninguno | Vuelo, soltar Garfios, daño | 25 k tris |
| **Aguja / Ojo de Yermo** | Fondo | Ninguno | Deriva, pulso | 30 k tris |

### Armas y props

| Asset | Tipo | Animaciones | Presupuesto |
|---|---|---|---|
| **Carbón** | Arma | Idle, disparo, recarga | 10 k tris |
| **Nogal** | Arma | Idle, disparo, recarga | 8 k tris |
| **Estrella** | Arma | Idle, carga, disparo | 12 k tris |
| **Cascabel** | Arma | Idle, disparo | 8 k tris |
| **Kit de casco** | Pickup | Flotar | 2 k tris |
| **Munición** | Pickup | Flotar | 1 k tris |
| **Escudo de reno** | Pickup | Flotar / girar | 2 k tris |
| **Collares** | Coleccionable | — | 1 k tris |

### Definición de terminado por asset

Un asset de Blender **no está terminado** solo por estar modelado. Está terminado cuando:

- [ ] Tiene escala correcta y transforms aplicados.
- [ ] Tiene UVs sin solapamientos raros.
- [ ] Tiene texturas PBR (Albedo, Normal, ORM).
- [ ] Tiene rig y peso limpio (si es personaje).
- [ ] Tiene todas las animaciones de su lista.
- [ ] Tiene LODs y colisión simple.
- [ ] Está exportado a FBX con el naming acordado.
- [ ] Está importado en Unity y probado con la cámara y la luz reales.
- [ ] Está dentro del presupuesto de triángulos y texturas.
- [ ] No tiene placeholders ni materiales provisionales.

### Kit del mapa (Blender)

| Categoría | Piezas |
|---|---|
| Calles y aceras | Rectas, curvas, cruces, rampas, nieve |
| Edificios | 12–20 módulos: vivienda, tienda, oficina, esquina, tejado, azotea |
| Zona histórica | Iglesia, plaza, fuente, árbol grande, mercado |
| Distrito financiero | Torres, pasarelas, cristaleras, puente atirantado |
| Refugio | Cúpula, faro, compuertas, antenas, explanada |
| Naturaleza | Pinos, abetos, nieve, rocas, montañas de fondo |
| Props | Farolas, coches, autobuses, vallas, bancos, cabinas, cables, carteles, escombros |
| Destructibles | Chimeneas, coches, farolas, muros de hielo |

---

## Pipeline Blender → Unity

### Unidades y ejes
- 1 unidad de Blender = **1 metro**.
- Aplicar escala y rotación antes de exportar.
- Eje de exportación: **Forward -Z, Up Y**.
- Personajes mirando hacia **-Y en Blender** (que se convierte en +Z en Unity) o el eje que se acuerde; probar con un cubo de referencia.
- Pivote en la base (pies, patines, base del edificio).
- Sin transforms negativos ni escalas no aplicadas.

### Nombres
| Prefijo | Uso |
|---|---|
| `SM_` | Static Mesh (edificios, props) |
| `SK_` | Skeletal Mesh (Santa, renos, boss) |
| `M_` | Material |
| `T_` | Textura |
| `AN_` | Animación |
| `_LOD0/1/2` | Niveles de detalle |
| `_COL` | Colisión |

Ejemplos: `SK_Santa`, `SK_Reindeer_Rodolfo`, `SM_Sleigh`, `SM_Bldg_Office_A`, `AN_Reindeer_Gallop`, `T_Snow_Albedo`.

### Exportación FBX
- Formato FBX 7.4 binario.
- Aplicar modificadores.
- Incluir armaduras y animaciones.
- Sin luces ni cámaras en los FBX de personajes.
- Un FBX por personaje/prop; el kit del mapa por módulos.
- Probar cada FBX en una escena limpia de Unity antes de integrarlo.

### Materiales y texturas
- Materiales PBR compatibles con URP Lit.
- Un set de texturas por familia: Albedo, Normal, ORM/MetallicSmoothness.
- 2K para héroes, 1K arquitectura, 512 props.
- Emisivos para ventanas, ojos y luces.
- No usar shaders procedurales de Blender que no exporten; hornear si hace falta.
- Nada de materiales gigantes con 20 texturas distintas.

### LODs y colisiones
- LOD0: héroe; LOD1: 40–50 %; LOD2: 15–20 %.
- Crear LODs en Blender o en Unity; documentar cuál.
- Colisiones: mallas simples aparte (`_COL`) o colliders de Unity.
- Los edificios pueden compartir colliders de caja.

### Carpetas
```
BlenderSource/              .blend maestros (fuera de Assets, no se importan)
Assets/
  Art/
    Models/
      Characters/
      Enemies/
      Ships/
      Weapons/
      Map/
      Props/
    Materials/
    Textures/
    Animations/
  Prefabs/
    Characters/
    Enemies/
    Weapons/
    Map/
```

Los `.blend` se guardan en `BlenderSource/` en la raíz del proyecto, no dentro de `Assets`, para que Unity no los importe ni los duplique. Los FBX y texturas sí viven en `Assets`.

---

## Qué falta para terminar el juego

### 1. Blender: assets y animación

- [x] Blockout de Santa, trineo, reno y kit de mapa.
- [x] Modelo final de Santa.
- [x] Modelo final del trineo y suspensión.
- [x] Modelo base de reno + 4 variantes.
- [x] Arnés, cuerdas y cascabeles.
- [x] 4 armas.
- [x] Enemigos: Sabueso, Garfio y Espora + Manto y Torre rediseñados como robots de 4 brazos.
- [x] 2 naves (Caza y Recolector). La Aguja/Ojo de fondo queda pendiente.
- [x] Nicolás Cero. Los 2 renos de obsidiana quedan como variante de material pendiente.
- [ ] Kit del mapa (calles, edificios, iglesia, puente, refugio, props).
- [~] UVs hechas; **falta hornear Albedo/Normal/ORM** (el FBX lleva color plano por material).
- [x] Rigs de Santa, renos, trineo, enemigos y boss (guardados en `SK_*_vat.blend`).
- [x] Animaciones de los clips principales (idle, galope, muerte, ataque, sacada de arma…).
- [x] LODs y colisiones.
- [x] Export FBX (7.4, dos capas UV). **Importado en el proyecto principal**: 28 prefabs, 375 materiales, texturas float y escena `Game` reconstruida.

### 2. Unity: sistemas de juego

- [x] Escena `Game` con el mapa montado.
- [x] Jugador con arte de Blender: trineo, Santa, renos y **las 4 armas en la mano** integrados.
- [x] `SleighController` funcional sobre `VehicleMotor`: vuelo, avance, lateral y vertical.
- [x] Cámara de gameplay.
- [x] `ReindeerSquad`: 4 vidas, herido, enganche y rescate.
- [x] `SleighHealth`: casco, daño, reparación y caída.
- [x] 4 armas: disparo, calor, munición y cambio.
- [x] 5 enemigos con ataques y telegraph (más naves y renos de obsidiana).
- [x] Director de oleadas por Z (9 oleadas precolocadas).
- [x] Boss Nicolás Cero con 3 fases y barrera de arena.
- [x] Pickups: munición, kit, escudo y armas de historia.
- [x] Misión: actos, checkpoint del puente, game over y reintento.
- [~] Intro, final y créditos: textos y paneles; falta la puesta en escena cinematográfica.
- [~] Pausa hecha; faltan opciones (volumen, sensibilidad, subtítulos).
- [x] HUD de combate.
- [x] Pantalla de resultados.
- [x] Guardado de perfil (ya hecho en login).
- [ ] Build de Windows.
### 3. UI / 2D

- [ ] Logo final (si se quiere reemplazar el texto).
- [ ] Iconos de armas, renos y estados.
- [x] HUD de combate (casco, renos, arma, calor/munición, jefe, objetivo y rescate).
- [x] Pantalla de pausa.
- [~] Pantalla de resultados hecha; créditos básicos.
- [ ] Subtítulos.
- [x] Indicadores de daño, rescate y objetivo.
- [ ] Cursores y estados de foco.
- [ ] Pruebas a 1280×720, 1920×1080 y 2560×1440.

### 4. Narrativa y voz

- [x] Guion de la intro: Voss pide ayuda, Santa ve la ciudad y despega. (Texto de radio implementado.)
- [~] 6–8 líneas de radio durante la misión: avisos de HUD y fases; falta grabación.
- [ ] Línea(s) de Nicolás Cero antes y durante el boss.
- [~] Final base: panel de resultados; falta la puesta en escena.
- [ ] Variante secreta si llegan los 4 renos vivos (mismas escena, distintas líneas).
- [~] Créditos: línea en el panel de resultados; falta pantalla completa.
- [ ] Subtítulos en español para todas las voces.

### 5. Audio

- [ ] Tema de menú.
- [ ] Tema de vuelo.
- [ ] Tema de boss.
- [ ] Tema de final.
- [ ] Ambiente de viento y ciudad.
- [ ] SFX de trineo, cascabeles, armas, enemigos y boss.
- [ ] Voz de Voss (radio).
- [ ] Pocas líneas de Santa.
- [ ] Mezcla y ducking.
- [ ] Subtítulos.

### 6. VFX

- [ ] Nieve ambiental y nieve levantada.
- [ ] Chispas de patines.
- [ ] Impactos y explosiones.
- [ ] Humo y fuego.
- [ ] Sangre contenida.
- [ ] Distorsión de los Yermos.
- [ ] Aurora y luces.
- [ ] Estelas de proyectiles.
- [ ] Polvo de regalo.

### 7. QA y release

- [ ] Prueba completa de 20–30 min.
- [ ] Prueba de perder los 4 renos.
- [ ] Prueba de checkpoint.
- [ ] Prueba de boss.
- [ ] Prueba de pausa, opciones y salida.
- [ ] Prueba de rendimiento (60 FPS).
- [ ] Prueba de mando y teclado/ratón.
- [ ] Prueba de resolución.
- [ ] Build limpio.
- [ ] Página de itch.io o Steam (opcional).
- [ ] Capturas y tráiler corto (opcional).

---

## Lo que NO se hace en esta versión

| Fuera de alcance | Motivo |
|---|---|
| Sistema de niveles / XP | El juego es una misión corta; no aporta. |
| Monedas / Luz | No hay tienda ni mejoras. |
| Mejoras de armas o trineo | Las armas se obtienen en la historia; no se suben. |
| Árbol de habilidades | No hay progresión. |
| Inventario | No hay objetos que gestionar. |
| Varios mapas / capítulos | Un solo mapa y una sola misión. |
| Selección de nivel | El juego empieza siempre desde el login. |
| Mundo abierto | Es un corredor cinematográfico. |
| Multijugador / cooperativo | Fuera del alcance de v1. |
| Personalización del trineo | No aporta a la historia. |
| Misiones secundarias | La misión principal es el juego. |
| Tienda / pases de batalla | No hay monetización. |
| Guardado de progreso de misión | Solo perfil y checkpoint en memoria. |
| Doblaje completo | Solo Voss y frases de Santa. |
| Localización a otros idiomas | Español primero; inglés después si se quiere. |
| Logros | No son necesarios para terminar. |
| Móvil / consola / VR | PC primero. |
| Assets 3D de Asset Store | Todo el 3D es Blender. |

Si alguna de estas ideas vuelve, se apunta para una secuela o actualización. No entra en la versión que hay que terminar.

---

## Plan de trabajo restante

### Fase F0 · Alcance congelado
**Estado:** hecha con este documento.

- Un mapa, una misión, un boss, 4 armas, 5 enemigos, sin niveles.
- Todos los 3D en Blender.

### Fase F1 · Blockout jugable (2 semanas)
- Escena `Game` con primitivas.
- Trineo, cámara, 4 renos, 2 armas, 1 enemigo.
- Corredor de prueba de 2–3 minutos.
- HUD mínimo.

**Salida:** se puede volar, disparar y perder un reno. Todavía con placeholders.

### Fase F2 · Kit de Blender v1 (4–6 semanas)
- Santa, trineo, reno base + variantes.
- 4 armas, 5 enemigos, 3 naves.
- Kit del mapa: calles, edificios, iglesia, puente, refugio.
- Rig y animaciones base.
- Export e integración de prueba.

**Salida:** el mapa y los personajes se ven como el juego final, aunque falten animaciones y pulido.

### Fase F3 · Misión completa (3–4 semanas)
- Los 4 actos dentro del mapa.
- Oleadas, Garfio, checkpoint, pickups.
- Boss Nicolás Cero.
- Intro, final y créditos.
- HUD y resultados.

**Salida:** se puede jugar de principio a fin, con arte final y sonido provisional.

### Fase F4 · Audio, UI y pulido (2–3 semanas)
- Música y SFX.
- Voz de Voss.
- Subtítulos y accesibilidad.
- VFX finales.
- Ajuste de dificultad.

**Salida:** experiencia completa y legible.

### Fase F5 · QA y build (1–2 semanas)
- Pruebas completas.
- Optimización.
- Build de Windows.
- Corrección de bugs.
- Capturas/tráiler.

**Salida:** juego terminado.

### Estimación
| Equipo | Tiempo estimado |
|---|---|
| 1 programador + 1 artista 3D + apoyo audio/UI | 12–16 semanas |
| 1 sola persona (todo) | 5–7 meses |
| Equipo de 3–4 con dedicación parcial | 4–6 meses |

La ruta crítica son los assets de Blender. Si el arte se retrasa, el juego se retrasa; el código puede avanzar con placeholders.

---

## Orden recomendado para empezar

1. **Congelar el alcance** (este documento).
2. **Blockout del mapa** en Unity con primitivas, solo para medir la ruta y la cámara.
3. **Blockout de Santa, trineo y reno** en Blender, con proporciones reales.
4. **Kit modular del mapa** en Blender: una calle, un edificio, una esquina, un puente, el refugio.
5. **Integrar el kit** en Unity y comprobar escala, colisiones y rendimiento.
6. **Rig y animación del reno base** (es el asset más repetido).
7. **Santa y trineo**.
8. **Armas y enemigos**.
9. **Boss**.
10. **Misión, UI, audio y QA**.

No empezar por el boss ni por detalles de UI. Primero el vuelo y el mapa, que es lo que el jugador va a sentir durante 25 minutos.

---

## Criterios de aceptación del juego terminado

### Historia
- [ ] Se entiende que es Nochebuena, que hay una invasión y que el refugio es el destino.
- [ ] Se entiende por qué Santa lucha: los renos y la gente.
- [ ] El final cierra la historia (base o secreto).
- [ ] No hace falta leer la documentación para entender el juego.

### Jugabilidad
- [ ] Volar se siente bien en los primeros 30 segundos.
- [ ] Disparar se siente bien antes del primer enemigo difícil.
- [ ] Perder un reno duele y se entiende.
- [ ] El rescate con Garfio es claro.
- [ ] El boss se puede aprender y vencer.
- [ ] El checkpoint funciona.
- [ ] No hay bloqueos ni estados imposibles.

### Arte
- [ ] Todos los 3D finales son de Blender.
- [ ] No quedan primitivas de Unity en la escena final.
- [ ] El mapa tiene 4 zonas legibles.
- [ ] Los renos se distinguen entre sí.
- [ ] Los Yermos se leen como amenaza.
- [ ] La iluminación cuenta la historia.

### Técnica
- [ ] 60 FPS a 1080p en PC medio.
- [ ] Sin errores en consola.
- [ ] Sin creación de escena/UI en runtime.
- [ ] Todos los valores de gameplay en Inspector/ScriptableObject.
- [ ] Build de Windows que arranca y se cierra bien.
- [ ] Pausa, opciones y salida funcionan.

### UX
- [ ] Login claro y con 2 botones.
- [ ] HUD legible.
- [ ] Subtítulos.
- [ ] Remapeo o al menos teclado/ratón y mando.
- [ ] Resoluciones 1280×720, 1920×1080 y 2560×1440.
- [ ] El juego se puede terminar en 20–30 minutos.

---

## Riesgos del alcance final

| Riesgo | Impacto | Mitigación |
|---|---|---|
| El kit de Blender tarda más de lo previsto | Alto | Empezar por módulos reutilizables; no modelar 100 edificios únicos. |
| Los 4 renos requieren demasiada animación | Alto | Rig y animaciones compartidas; variantes por materiales y astas. |
| El mapa modular se ve repetitivo | Medio | 3–4 landmarks fuertes y variación de materiales/props. |
| El boss es demasiado complejo | Medio | 3 fases claras, reutilizar enemigos y naves. |
| El juego se alarga más de 30 min | Medio | Medir la ruta y cortar tramos; la duración es parte del diseño. |
| El realismo no llega | Medio | Priorizar silueta, luz y animación sobre detalle microscópico. |
| Se cuela una idea de “nivel” o “mejora” | Alto | Volver a este documento; si no es historia o combate, no entra. |

---

## Resumen de una frase

**CLOUS-END v1 es una misión corta y terminada: un mapa de ciudad hecho en Blender, Santa y su trineo, cuatro renos que son cuatro vidas, cuatro armas, cinco enemigos y un boss final; sin niveles, sin monedas y sin mejoras.**
