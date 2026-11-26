# 10 · Producción y roadmap

> **Alcance final v1.0:** un mapa, una misión, un boss, **sin niveles ni progresión**, y **todos los assets 3D hechos en Blender**. El desglose completo de contenido, assets y criterios está en [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md). Este documento es el plan de producción resumido.

## Estado actual

| Entregable | Estado |
|---|---|
| Documentación de visión, historia, gameplay, arte, técnica y alcance | Hecha |
| Lobby con botón JUGAR (sin credenciales) | Hecho |
| Alcance final congelado (1 mapa, sin niveles) | Hecho |
| Escena `Game` y blockout del Distrito Aurora | Hecho (doc 12) |
| Checkpoint, meta, resultados y game over de la demo | Hecho |
| Demo WebGL en GitHub Pages | Hecho (doc 13) |
| **Assets 3D de la v1 en Blender** (Santa, trineo, 4 renos, armas, enemigos, naves, boss, props) | **Hecho (doc 14)** |
| **Rigs, clips y horneado VAT** | **Hecho (doc 14)** |
| **LODs y colisiones** | **Hecho (doc 14)** |
| Horneado de Albedo/Normal/ORM | Pendiente |
| Import del paquete a Unity | Pendiente |
| Kit del mapa en Blender | Pendiente |
| Audio | Pendiente |
| Build de Windows | Pendiente |

---

## Alcance cerrado

| Contenido | Cantidad |
|---|---|
| Misiones | 1 |
| Mapas | 1: Distrito Aurora |
| Renos | 4 (4 vidas) |
| Armas | 4, sin mejoras |
| Enemigos | 5 tipos |
| Boss | 1: Nicolás Cero |
| Naves alienígenas | 3 tipos |
| Pantallas | Login, HUD, pausa, resultados/créditos |
| Duración | 20–30 minutos |
| Arte 3D | 100 % Blender |
| Progresión | Ninguna (solo perfil y checkpoint en memoria) |

**No entra:** niveles, XP, monedas, mejoras, inventario, árbol de habilidades, varios mapas, selección de nivel, mundo abierto, multijugador, cooperativo, personalización, misiones secundarias, tienda, pases de batalla, VR, móvil, assets 3D de Asset Store.

---

## Fases de producción

### F0 · Alcance congelado
**Estado:** hecha.

- Un mapa, una misión, un boss, 4 armas, 5 enemigos.
- Sin niveles ni mejoras.
- Todos los 3D en Blender.
- Documento rector: `11_Alcance_Final_Juego_Terminado.md`.

### F1 · Blockout jugable (2 semanas)
- Escena `Game` con primitivas.
- `SleighController`, cámara, 4 renos, 2 armas y Sabueso.
- Corredor de prueba de 2–3 minutos.
- HUD mínimo.

**Estado:** parcial. Ya están el mapa completo, la cámara, el vuelo, la mira, el disparo, el HUD,
el checkpoint, la meta y el game over. Faltan los renos, las armas y los enemigos de esta fase.

**Salida:** volar, disparar y perder un reno ya funciona con placeholders.

### F2 · Kit de Blender v1 (4–6 semanas)
- Santa, trineo, reno base + 4 variantes.
- 4 armas, 5 enemigos, 3 naves.
- Kit del mapa: calles, edificios, iglesia, puente, refugio, props.
- Rigs y animaciones base.
- Export FBX, integración y prueba de escala/rendimiento.

**Estado:** personajes, armas, enemigos, naves, boss, props, rigs, animaciones
(VAT), LODs y colisiones **hechos y exportados** (doc 14). Faltan el kit del mapa
y el horneado de texturas.

**Salida:** el juego se ve como el producto final, aunque falten pulido y audio.

### F3 · Misión completa (3–4 semanas)
- Los 4 actos dentro del mapa.
- Oleadas, Garfio, checkpoint, pickups.
- Boss Nicolás Cero.
- Intro, final y créditos.
- HUD y resultados.

**Salida:** se puede jugar de principio a fin.

### F4 · Audio, UI y pulido (2–3 semanas)
- Música, SFX, voz de Voss.
- Subtítulos, accesibilidad y opciones.
- VFX finales.
- Ajuste de dificultad y duración.

**Salida:** experiencia completa y legible.

### F5 · QA y build (1–2 semanas)
- Pruebas completas.
- Optimización.
- Build de Windows.
- Corrección de bugs.
- Capturas/tráiler (opcional).

**Salida:** juego terminado.

---

## Estimación

| Equipo | Tiempo |
|---|---|
| 1 programador + 1 artista 3D + apoyo audio/UI | 12–16 semanas |
| 1 sola persona (todo) | 5–7 meses |
| Equipo de 3–4 con dedicación parcial | 4–6 meses |

La ruta crítica es el arte de Blender. El código puede avanzar con placeholders, pero el juego no se termina sin los modelos y animaciones.

---

## Reparto sugerido

| Rol | Responsabilidad |
|---|---|
| Diseño / dirección | Alcance, historia, tuning, revisión. |
| Programación | Sistemas, herramientas de editor, rendimiento, build. |
| Arte 3D / Blender | Mapa, Santa, trineo, renos, enemigos, naves, armas, props. |
| Animación (Blender) | Renos, Santa, trineo, enemigos, boss. |
| Audio | Música, SFX, voz, mezcla. |
| UI / 2D | HUD, iconos, resultados, créditos. |
| QA | Flujo completo, dificultad, accesibilidad, build. |

En un equipo de 2–3 personas, los roles se combinan; el alcance no.

---

## Riesgos y mitigación

| Riesgo | Impacto | Mitigación |
|---|---|---|
| El kit de Blender tarda más de lo previsto | Alto | Módulos reutilizables; no modelar 100 edificios únicos. |
| Los 4 renos requieren demasiada animación | Alto | Rig y animaciones compartidas; variantes por materiales/astas. |
| El mapa modular se ve repetitivo | Medio | 3–4 landmarks fuertes y variación de props. |
| El boss es demasiado complejo | Medio | 3 fases claras; reutilizar enemigos y naves. |
| El juego se alarga más de 30 min | Medio | Medir la ruta y cortar; la duración es parte del diseño. |
| Se cuela una idea de “nivel” o “mejora” | Alto | Volver a `11`; si no es historia o combate, no entra. |
| El realismo no llega | Medio | Priorizar silueta, luz y animación sobre detalle microscópico. |

---

## Próximas 10 tareas

1. Importar el paquete VAT a Unity y crear los prefabs (doc 14).
2. Hornear Albedo/Normal/ORM de los assets de Blender.
3. Primer módulo de calle + edificio + esquina en Blender (kit del mapa).
4. Integrar ese módulo en Unity y comprobar escala, colisiones y FPS.
5. Implementar `SleighController` y cámara con los modelos finales.
6. Implementar `ReindeerSquad` y la pérdida de un reno.
7. Conectar la animación de muerte del reno y la sacada de arma al gameplay.
8. Añadir Sabueso y Carbón.
9. Montar el HUD como prefab editable.
10. Definir el guion de radio de la intro y el final.

---

## Definición de “juego terminado”

- [ ] Login → intro → misión única → boss → final → créditos.
- [ ] 20–30 minutos de duración.
- [ ] 4 renos como vidas; se puede perder y reintentar.
- [ ] 4 armas, 5 enemigos y 1 boss funcionando.
- [ ] Checkpoint del puente.
- [ ] Todos los 3D finales son de Blender; sin primitivas en la escena final.
- [ ] 60 FPS a 1080p en PC medio.
- [ ] Sin errores en consola.
- [ ] Sin creación de escena/UI en runtime.
- [ ] Todos los valores de gameplay en Inspector/ScriptableObject.
- [ ] Build de Windows que arranca, se juega y se cierra.
- [ ] Historia comprensible sin leer la documentación.

---

## Lo que no se hará

- Sistema de niveles, XP o monedas.
- Mejoras de armas, trineo o renos.
- Inventario o árbol de habilidades.
- Varios mapas o capítulos.
- Selección de nivel.
- Mundo abierto.
- Multijugador o cooperativo.
- Personalización del trineo.
- Misiones secundarias.
- Tienda, pases o monetización.
- Guardado de progreso de misión.
- Doblaje completo o localización a varios idiomas.
- Logros.
- Móvil, consola o VR.
- Assets 3D de Asset Store.

Si algo de esta lista vuelve, se apunta para una secuela. No entra en la versión que hay que terminar.
