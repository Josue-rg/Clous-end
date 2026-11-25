# 06 · Personajes y vehículos

> **Alcance final v1.0:** Santa, trineo, 4 renos, 4 armas y todos los props 3D se modelan y animan en **Blender**. No hay mejoras ni niveles de arma. Fuera de v1: Regalo, Aurora y los sistemas de progresión. Ver [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md).

## Santa Claus

### Concepto
Un artesano anciano, no un superhéroe. Espalda ancha de cargar sacos, barba descuidada por la noche, botas gastadas, guantes de cuero. El traje rojo está descolorido y tiene parches; la única prenda nueva es un chaleco térmico del taller.

### Silueta
- Abrigo largo hasta la rodilla, abierto, con forro de pelo.
- Gorro sin borla (se la arrancó en el despegue) o con borla deshilachada.
- Cinturón con herramientas, no solo hebilla.
- Botas con crampones improvisados.

### Materiales
- Lana: roughness 0.85, sheen bajo.
- Cuero: roughness 0.55, desgaste en codos y rodillas.
- Piel: SSS ligera, venas y manchas de edad.
- Nieve acumulada en hombros y barba (shader de acumulación simple).

### Rig y animación
- Rig humanoide estándar (Unity Humanoid) + huesos extra para abrigo y barba.
- Animaciones clave: pilotar, girar el torso para apuntar, agarrar la bolsa, mirar atrás, cubrirse, respiración en reposo.
- La barba y el abrigo usan *cloth* o huesos secundarios; nunca animación rígida.
- En el trineo, Santa **no** está sentado como un maniquí: se inclina con las curvas, corrige el rumbo y reacciona a los impactos.

### Modelo
| Campo | Objetivo |
|---|---|
| Triángulos | 60–80 k (LOD0), 25 k (LOD1), 8 k (LOD2) |
| Texturas | 4K cuerpo/traje, 2K cabeza, 1K detalles |
| Huesos | 80–100 |
| Blend shapes | 12 (expresión, respiración, frío) |

---

## Los cuatro renos

### Base común
- Reno realista (*Rangifer tarandus*), no cartoon. Pelaje denso, hocico claro, astas asimétricas.
- Cada uno tiene una **marca de arnés** distinta y un cascabel con un tono propio.
- Tamaño: 1,8–2,1 m de longitud, 1,4 m de altura a la cruz.
- Rig: 45–60 huesos, control de cola, orejas, cuello y patas. Galope de cuatro tiempos con suspensión.
- El pelaje usa tarjetas de pelo en cuello y vientre; el resto, textura con *depth*.
- Respiración visible (vapor) a baja temperatura.

### Fichas

#### Rodolfo — el guía
- **Marca:** arnés rojo, cascabel de latón.
- **Rasgo:** nariz ligeramente luminosa (no es un faro de dibujos: un brillo cálido tenue).
- **Pasiva:** resalta amenazas ocultas y +10 % de precisión.
- **Animación:** mira hacia el peligro antes que los demás; tira del grupo.
- **Muerte:** su luz se apaga. Es la pérdida más dura.

#### Donner — el fuerte
- **Marca:** arnés de cuero grueso, placas de refuerzo.
- **Rasgo:** el más grande, cuello cicatrizado.
- **Pasiva:** −15 % de daño al casco.
- **Animación:** pisada más pesada; sujeta el trineo en las curvas.

#### Vixen — la ágil
- **Marca:** arnés ligero con cintas azules.
- **Rasgo:** la más pequeña y rápida; orejas siempre alerta.
- **Pasiva:** +1 carga de dash y recuperación más rápida.
- **Animación:** cambia de dirección antes; salta obstáculos.

#### Cometa — la veloz
- **Marca:** arnés con una estrella cosida.
- **Rasgo:** patas largas, galope amplio.
- **Pasiva:** +8 % velocidad base y turbo más eficiente.
- **Animación:** lidera en las rectas; se impacienta en los giros.

### Estados de daño
| Estado | Visual | Mecánica |
|---|---|---|
| Sano | Galope firme, cascabel claro | Pasiva activa |
| Herido | Cojea, sangre en el flanco, cascabel irregular | Pasiva desactivada hasta el punto de control |
| Enganchado | Garfio tenso, el reno se eleva | Ventana de rescate de 5 s |
| Perdido | El arnés queda colgando; silencio | Vida perdida para siempre |

La muerte de un reno se escenifica con cámara lenta, caída del arnés y un plano del resto de la yunta. Nunca explota ni desaparece.

---

## El trineo

### Concepto
Un trineo de madera y hierro, construido a mano, con refuerzos de guerra improvisados. No es un vehículo militar: es una reliquia familiar adaptada para sobrevivir.

- **Largo:** 6,5 m.
- **Ancho:** 2,2 m.
- **Peso:** 900 kg en vacío; 1,4 t con la bolsa.
- **Materiales:** madera de fresno, hierro forjado, cuero, latón, cristal, nieve.

### Partes
| Parte | Función |
|---|---|
| Patines | Suspensión; chispas al aterrizar; se dañan con impactos. |
| Caja | Espacio para Santa, la bolsa y los módulos de arma. |
| Bolsa | Munición y armas; se puede perder. |
| Corazón de Estrella | Núcleo luminoso; energía del trineo y del refugio. |
| Arnés | Conecta a los cuatro renos; se puede cortar. |
| Farol | Luz cálida delantera; se apaga en los túneles. |
| Campanas | Estado del trineo; suenan distinto al recibir daño. |

### Daño visual
1. **75 %:** madera astillada, farol parpadeante.
2. **50 %:** humo, un patín desalineado, la bolsa se abre.
3. **25 %:** fuego, cascabeles rotos, Santa sangra.
4. **0 %:** caída; el trineo se desmonta en el aire.

### Animación
- Suspensión: rebote al pasar baches, inclinación en curvas, derrape en hielo.
- La bolsa se balancea con inercia.
- Las riendas se tensan y aflojan; nunca son una línea rígida.
- Los renos y el trineo están conectados por cuerdas con simulación (Obi Rope o similar, o huesos).

### Modelo
| Campo | Objetivo |
|---|---|
| Triángulos | 80–120 k (LOD0) |
| Materiales | 6–8 (madera, hierro, cuero, cristal, emisivo, nieve) |
| Texturas | 4K madera/hierro, 2K detalles |
| Huesos | 30 (suspensión, patines, bolsa, riendas) |

---

## Armas

### Diseño
Todas parecen objetos navideños o herramientas del taller. La lectura debe ser inmediata: el jugador sabe qué arma lleva sin mirar el HUD.

| Arma | Modelo | Mecánica | Uso |
|---|---|---|---|
| **Carbón** | Fusil de latón con tolva de carbón | Automático, calor, infinito | Presión constante |
| **Nogal** | Escopeta de dos cañones con culata de nogal | 8 cartuchos, cono, retroceso | Corta distancia / escudos |
| **Estrella** | Rifle largo con bobinas y un farol en la mira | 3 cargas, perfora, electrocuta | Precisión / élites |
| **Cascabel** | Lanzador de cascabeles con tambor | Cadena a 4 objetivos | Grupos |

**Fuera de v1:** Regalo (torreta/mina) y Aurora (ultimate). No hay niveles de arma ni mejoras.

### Reglas de modelado
- Escala coherente con las manos de Santa.
- Desgaste y pintura desconchada; nada de cromo limpio.
- Piezas móviles animadas: cerrojo, tambor, bisagras.
- Sonido propio por arma, con cola de radio.

---

## Sin mejoras en v1

No hay mejoras de armas, trineo ni renos. Lo que el jugador tiene al empezar es lo que tiene al terminar; la única progresión es la historia y las armas que recoge en el mapa.

Las antiguas líneas de mejora (Arsenal, Trineo, Yunta) quedan como idea para una secuela, no para esta versión.

---

## Lista de animaciones de la misión

### Santa
- Idle en el trineo (3 variantes).
- Inclinación izquierda/derecha.
- Disparo con cada arma.
- Recarga.
- Impacto / sacudida.
- Mirar atrás.
- Agarrar la bolsa.
- Victoria / derrota.

### Renos
- Galope (4 variantes de velocidad).
- Trote.
- Giro.
- Salto.
- Herido.
- Enganchado / forcejeo.
- Caída.
- Muerte.

### Trineo
- Vuelo recto.
- Inclinación.
- Derrape.
- Turbo.
- Impacto leve / grave.
- Caída.
- Aterrizaje.

---

## Presupuesto de personajes

| Elemento | LOD0 | LOD1 | LOD2 |
|---|---|---|---|
| Santa | 70 k tris | 25 k | 8 k |
| Reno | 35 k tris | 14 k | 5 k |
| Trineo | 100 k tris | 35 k | 12 k |
| Arma | 8–15 k tris | 5 k | — |

En pantalla simultánea: Santa + 4 renos + trineo + 2 armas visibles. Nunca más de 6 personajes animados a la vez.
