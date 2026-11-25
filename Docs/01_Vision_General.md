# 01 · Visión general

> **Alcance final v1.0:** un solo mapa, una sola misión, un solo boss, **sin niveles, XP, monedas ni mejoras**. Todos los assets 3D se hacen en Blender. El documento rector es [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md); este texto describe la fantasía y el tono.

## Pitch

> La última Nochebuena no se celebra. Se defiende.

Santa Claus despega del Polo Norte con cuatro renos, una bolsa de regalos incompleta y una radio que no deja de repetir que el mundo está cayendo. La ciudad está siendo devorada por una invasión alienígena. El único refugio que queda es **Aurora-9**. Para llegar, Santa convertirá su trineo en un arma y cruzará una ciudad helada mientras protege lo único que le queda: sus renos.

CLOUS-END es un shooter de defensa sobre un trineo en movimiento: rápido, cinematográfico y corto. El jugador dispara, esquiva, prioriza amenazas y toma decisiones desesperadas para que la yunta llegue viva al refugio.

---

## Ficha

| Campo | Decisión |
|---|---|
| Nombre | CLOUS-END (Operación Clous-End) |
| Género | Shooter 3D de defensa vehicular, on-rails con movimiento lateral |
| Perspectiva | Tercera persona detrás del trineo, cámara cinematográfica |
| Modo | Un jugador, misión única lineal |
| Duración | 20–30 min |
| Mapas | 1: Distrito Aurora |
| Arte 3D | 100 % Blender |
| Plataforma | PC primero; mando y teclado/ratón |
| Motor | Unity 6.6 · URP |
| Clasificación | Teen: violencia fantástica, tensión, muerte de animales |
| Referencias | *Star Fox*, *Panzer Dragoon*, *Metro Exodus*, *God of War* (escala), *Klaus* (calidez), *Blade Runner 2049* (los Yermos) |

---

## Pilares de diseño

### 1. Navidad bajo asedio
El contraste es el corazón visual y emocional. Luces cálidas, villancicos deformados por la radio, nieve sucia, sangre sobre escarcha. La Navidad no es un decorado: es la razón por la que se lucha.

### 2. El trineo es el personaje
No es una nave genérica. Pesa, derrapa, cruje, se inclina en las curvas y arrastra bolsas de regalo. Cada impacto se siente en la suspensión y en los renos. El jugador debe notar que está pilotando un objeto querido, no un caza.

### 3. Cada reno es una vida
Los cuatro renos son las cuatro vidas. No hay número abstracto: cuando muere uno, se pierde su nombre, su silueta y su habilidad pasiva. La pérdida se escenifica y cambia el juego.

### 4. Espectáculo realista
Modelos PBR, animación con peso, iluminación física y mapas con composición cinematográfica. El realismo no es un adjetivo de marketing: es una lista de comprobación en cada asset.

### 5. Corto y completo
Una sola misión de 20–30 minutos, sin relleno. No hay niveles ni mejoras: la rejugabilidad viene de dominar el vuelo, el combate y el rescate de renos, no de subir números.

---

## Bucle de juego

```
Login
  ↓
Intro de radio + despegue
  ↓
Volar por el único mapa (riel + desplazamiento lateral)
  ↓
Detectar amenazas y priorizar
  ├─ enemigos que atacan al trineo  → disparar / esquivar
  ├─ enemigos que atacan a un reno  → interceptar / rescatar
  └─ peligros del entorno           → maniobrar
  ↓
Recoger suministros y armas de historia
  ↓
Checkpoint del puente
  ↓
Refugio Aurora
  ↓
Boss final: Nicolás Cero
  ↓
Final + créditos
```

Una partida completa dura 20–30 minutos: cuatro actos dentro del mismo mapa y un boss de 3–5 minutos. No hay pantalla de selección de nivel.

---

## Fantasía del jugador

- "Soy Santa Claus, pero ya no reparto regalos: reparto plomo."
- "Mi trineo es mi casa y mis renos son mi familia; no voy a dejar que ninguno caiga."
- "Cruzo una ciudad en ruinas a toda velocidad, con la aurora detrás y una flota alienígena encima."

Lo que el jugador debe sentir en los primeros 60 segundos: **peso, velocidad y peligro**. Lo que debe sentir al perder un reno: **silencio**.

---

## Público

- Jugadores de 16–40 años que disfrutan experiencias cortas y espectaculares.
- Fans de shooters arcade y de experiencias narrativas lineales.
- Público navideño que quiere algo distinto: no un juego infantil, no una parodia.

No es un juego para sesiones largas ni para multijugador. Es una experiencia de una tarde, memorable y rejugable.

---

## Diferenciador

La mayoría de juegos navideños son cozy, de gestión o de plataformas. CLOUS-END apuesta por lo contrario: **Navidad épica y melancólica**, con un protagonista vulnerable y un vehículo querido. La combinación de trineo + escolta + cuatro vidas con nombre propio no se ve en el mercado.

---

## Alcance cerrado (MVP)

| Contenido | Cantidad |
|---|---|
| Misiones | 1 |
| Mapas | 1: Distrito Aurora |
| Renos | 4 (4 vidas) |
| Armas | 4 (sin mejoras) |
| Enemigos comunes | 5 |
| Boss | 1: Nicolás Cero |
| Pantallas | Login, HUD, pausa, resultados/créditos |
| Idiomas | Español (base), inglés (post) |

**Fuera de alcance:** niveles, XP, monedas, mejoras, inventario, árbol de habilidades, múltiples mapas o capítulos, selección de nivel, mundo abierto, multijugador, cooperativo, personalización de trineo, misiones secundarias, doblaje completo, realidad virtual, assets 3D de Asset Store.

**Regla de arte:** todo el 3D final (mapa, Santa, trineo, renos, naves, enemigos, armas y props) se modela y anima en Blender.

---

## Métricas de éxito del juego terminado

1. Un jugador nuevo entiende el control en menos de 30 segundos.
2. La primera muerte de un reno se recuerda y se puede explicar.
3. La misión completa de 20–30 minutos se puede jugar del tirón.
4. 60 FPS estables en un PC medio a 1080p.
5. El login y el primer minuto de vuelo ya transmiten la fantasía del juego.
6. El jugador llega al final y entiende qué ha pasado.
7. No queda ningún placeholder ni primitiva de Unity en la escena final.
