# 08 · Dirección de arte, animación y audio

> **Alcance final v1.0:** un solo mapa, sin niveles ni progresión. **Todo el 3D se modela y anima en Blender**: mapa, Santa, trineo, renos, naves, enemigos, armas y props. La UI/2D y el audio se hacen fuera de Blender. Ver [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md).

## Norte visual

CLOUS-END debe sentirse como una **película de Navidad en tiempos de guerra**: hermosa, fría y dolorosa. El jugador tiene que querer quedarse a mirar el paisaje y, al mismo tiempo, sentir que no puede bajar la guardia.

La referencia no es el realismo de un simulador. Es el **realismo cinematográfico**: materiales creíbles, luz física, cámara con intención, animación con peso. Un plano puede ser estilizado; lo que no puede ser es genérico.

---

## Paleta emocional

| Momento | Color | Sensación |
|---|---|---|
| Navidad viva | Ámbar, rojo, verde abeto, oro | Calor, hogar, memoria |
| Noche polar | Azul profundo, cian, blanco | Soledad, escala |
| Los Yermos | Obsidiana, blanco estéril, violeta frío | Invasión, ausencia |
| Aurora | Verde menta, magenta, cian | Esperanza, milagro |
| Sangre / pérdida | Rojo oscuro, gris | Consecuencia |

**Regla:** el rojo saturado se reserva para Santa, la sangre y los avisos de peligro. Si todo es rojo, nada es rojo.

---

## Luz

- **Key light:** luna fría, azulada, con sombras suaves y largas.
- **Fill:** rebote de nieve, muy frío y débil.
- **Warm practicals:** ventanas, faroles, árbol de Navidad, hogueras. Son la vida del plano.
- **Los Yermos:** luz blanca sin sombra cálida, casi quirúrgica. Sus naves no iluminan: recortan.
- **Aurora:** luz de ambiente, no un foco. Se mueve lento.
- **Contraste:** cada capítulo debe tener una fuente de calor visible a la que el jugador quiera llegar.

### Postproceso (URP)
- Bloom suave, threshold alto para que solo brillen luces y emisivos.
- Tonemapping ACES.
- Viñeta contenida.
- Grano de película muy leve.
- Color grading por capítulo: más frío en el bosque, más ámbar en el pueblo, más negro en el Distrito Aurora.
- Sin *chromatic aberration* agresiva ni *lens dirt*: el juego no es una cámara encontrada.

---

## Materiales

| Superficie | Tratamiento |
|---|---|
| Nieve | Albedo ligeramente azul, roughness 0.35–0.6, micro-normal, chispas puntuales |
| Hielo | Roughness 0.05–0.2, transmisión simulada, grietas |
| Madera | Vetas, barniz gastado, humedad en los bordes |
| Cuero | Roughness variable, costuras, sudor |
| Metal | Óxido, pintura desconchada, huellas |
| Pelaje de reno | Tarjetas de pelo, variación de tono, nieve acumulada |
| Traje de Santa | Lana desgastada, parches, polvo de carbón |
| Yermos | Obsidiana húmeda, vetas emisivas, sin reflejos difusos |

**Regla:** si un material no cuenta una historia de uso, no está terminado.

---

## Cámara

- **FOV base:** 55–65° en gameplay; 40–45° en cinemáticas.
- **Altura:** detrás y por encima del trineo, con el horizonte en el tercio superior.
- **Movimiento:** la cámara sigue con retardo, anticipa curvas y se sacude con impactos.
- **Bosses:** planos más cerrados, cámara baja para dar escala.
- **Pérdida de reno:** cámara lenta, foco en el arnés, sin música.
- **Login:** deriva lenta, composición fija, sin cortes.

---

## Animación

### Principios
1. **Peso:** el trineo pesa; los renos tiran; Santa se agarra.
2. **Anticipación:** cada ataque enemigo se lee antes de ocurrir.
3. **Solapamiento:** abrigo, barba, riendas, bolsa y cascabeles se mueven con retardo.
4. **Silueta:** cualquier pose debe leerse en negro.
5. **Micro-movimiento:** respiración, parpadeo, nieve en la cara, vapor.

### Santa
- Rig humanoid + 8 huesos secundarios para abrigo y barba.
- En el trineo: inclinación de torso, corrección de rumbo, mirada al peligro.
- Disparo: retroceso de hombro, no de muñeca.
- Impacto: sacudida contenida; nunca animación de dibujo animado.

### Renos
- Galope de cuatro tiempos; el ritmo cambia con la velocidad.
- Las patas delanteras lideran; las traseras empujan.
- Cabeza y orejas reaccionan antes que el cuerpo.
- Herido: cojera asimétrica, respiración agitada.
- Enganchado: fuerza contra el arnés, patas en el aire.
- Muerte: caída, no explosión. El arnés queda colgando.

### Trineo
- Suspensión con rebote y amortiguación.
- Derrape en hielo con inclinación de patines.
- La bolsa se balancea con inercia.
- Las riendas se tensan y aflojan; nunca son una línea recta.
- Daño progresivo visible por estados (75/50/25/0 %).

### Enemigos
- Flotan con calma; atacan con un latigazo seco.
- Cada ataque tiene una pose de anticipación de al menos 0.8 s.
- Los Yermos no tienen respiración humana: su movimiento es mecánico y continuo.

---

## VFX

| Efecto | Uso |
|---|---|
| Nieve | Partículas de fondo + copos cercanos + nieve levantada por el trineo |
| Vapor | Respiración de renos y Santa |
| Chispas | Patines, impactos, cables |
| Sangre | Contenida, oscura, sobre nieve |
| Fuego | Ventanas, escombros, árboles |
| Aurora | Cielo y reflejos en la nieve |
| Distorsión Yerma | Aire alrededor de naves y élites |
| Polvo de regalo | Armas y pickups |
| Huellas | Patines y pezuñas en la nieve |

**Regla:** ningún VFX debe tapar la retícula ni la marca roja de un reno.

---

## UI

- Tipografía display para títulos; texto humanista para lectura.
- Cristal oscuro, bordes finos, mucho aire.
- Rojo solo para la acción principal y el peligro.
- Animaciones de 150–250 ms; nunca bloquean la entrada.
- El HUD vive en los bordes; el centro es para volar.
- La pantalla de login es la referencia de tono para el resto del juego.

---

## Audio

### Música
- Híbrido orquestal + coro + sintetizador analógico.
- Villancicos deformados: la melodía existe, pero está desafinada, rota o lejana.
- Los Yermos: coro grave, vidrio, radio.
- La aurora: drones luminosos, campanas invertidas.
- Silencio como recurso: la muerte de un reno no lleva música.

### Diseño sonoro
- **Cascabeles:** estado de los renos. Suenan distinto si están sanos, heridos o perdidos.
- **Trineo:** madera, cuero, metal, nieve. Cada impacto tiene capas.
- **Armas:** cada una con una firma sonora y una cola de radio.
- **Voz de Voss:** radio comprimida, entrecortada, humana.
- **Santa:** pocas líneas, voz cansada, nunca grandilocuente.
- **Ambiente:** viento, crujidos, ladridos lejanos, campanas.

### Mezcla
- Prioridad: diálogo > avisos de reno > armas > música > ambiente.
- Ducking de música en radio y pérdidas.
- Subtítulos por defecto.
- Sin audio essential para completar el juego.

---

## Presupuesto visual

| Elemento | Objetivo |
|---|---|
| Personajes en pantalla | 6 animados máx. |
| Enemigos comunes | 18 máx. |
| Luces en tiempo real | 1 direccional + 4 puntuales |
| Triángulos visibles | < 1,5 M |
| Draw calls | < 1.200 |
| Texturas | 2K héroes, 1K arquitectura, 512 props |
| Resolución | 1080p / 60 FPS en PC medio |
| Escalado | FSR / DLSS opcional |

El realismo se consigue con **composición, luz y animación**, no con más polígonos.

---

## Pipeline de assets

1. Blockout en Unity con primitivas y medidas reales.
2. Modelo en Blender con escala y pivotes correctos.
3. UVs y texturizado PBR (Substance o similar).
4. Import a Unity con presets por tipo.
5. LODs y colliders simples.
6. Prueba en escena con la luz final.
7. Aprobación de silueta en negro y en miniatura.

**Nada entra al juego si no se ha visto en la escena real, con la cámara real y la luz real.**
