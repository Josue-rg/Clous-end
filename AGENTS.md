# Reglas estrictas para trabajar en este proyecto de Unity

1. NUNCA generes objetos de escena, niveles ni interfaces (UI) mediante código en tiempo de ejecución (Start/Awake). Todo debe existir físicamente en la Jerarquía o como Prefabs editables antes de dar Play.
2. Si necesitas automatizar la creación de objetos o UI, crea un Editor Script (`[MenuItem]`) para que yo pueda ejecutarlo en el editor, ver el resultado y modificarlo visualmente.
3. PROHIBIDO hardcodear valores de jugabilidad (velocidades, vida, daño, tiempos). Expón absolutamente todas las variables configurables con `[SerializeField]` o usa `ScriptableObjects` para poder ajustarlas en el Inspector.
4. Mantén los scripts cortos, modulares y desacoplados (un script por responsabilidad), pensando en que el juego escalará a multijugador.

---

## Cómo se aplica esto en la práctica

- **Escenas y prefabs:** todo lo que se ve en pantalla debe estar guardado en `Assets/Scenes` o `Assets/Prefabs` y visible en la Jerarquía. Nada de `new GameObject()` en `Awake`, `Start` o `Update`.
- **Automatización permitida:** un Editor Script con `[MenuItem("Clous-End/...")]` puede construir una escena, un prefab o una UI una sola vez. Después queda guardado y se edita con el mouse en el Inspector, la Jerarquía o el Scene View.
- **Valores configurables:** velocidades, vida, daño, tiempos, probabilidades, textos y referencias van en `[SerializeField]` o en `ScriptableObject`. El código solo lee esos datos.
- **Scripts pequeños:** un archivo = una responsabilidad. Separar movimiento, cámara, disparo, vida, entrada, audio y UI. Evitar archivos de 500+ líneas.
- **Sin dependencias ocultas:** los sistemas se comunican por referencias serializadas o eventos; nada de `FindObjectOfType` en tiempo de ejecución.
- **Pensado para multijugador:** la lógica de dominio no debe depender de la UI ni de la cámara. Los datos deben poder ejecutarse en servidor y cliente.

## Convenciones del proyecto

- Carpetas: `Assets/Art`, `Assets/Audio`, `Assets/Prefabs`, `Assets/Scenes`, `Assets/Scripts`, `Assets/Settings`, `Assets/UI`, `Docs`.
- Escenas: `Login`, `Game` y una escena por capítulo.
- Nombres de clases en PascalCase; archivos igual que la clase.
- Los cambios de escena se guardan en disco antes de terminar una tarea.
- Toda pantalla debe poder revisarse sin dar Play, abriendo la escena en el editor.

## Documentación

La documentación vive en `Docs/`. Antes de implementar una pantalla o sistema, revisar el documento correspondiente. Si el código y el documento se contradicen, actualizar el documento en el mismo cambio.

## Alcance final v1

- El documento rector es `Docs/11_Alcance_Final_Juego_Terminado.md`.
- **Un solo mapa, una sola misión, un solo boss, 4 armas, 5 enemigos, sin niveles, XP, monedas, mejoras ni inventario.**
- **Todos los assets 3D se modelan y animan en Blender.** La UI/2D y el audio se hacen fuera de Blender.
- No añadir capítulos, mapas adicionales, progresión, mejoras, assets 3D de Asset Store ni sistemas de nivel sin aprobación explícita.
- Si una idea no mejora volar, defender o perder a un reno, no entra en v1: se apunta para una secuela.

---

## Control de versiones

- La única rama del proyecto es `main`. **Todos los cambios se commitean y se suben directamente a `main`.**
- No se crean ramas de trabajo, ramas `gh-pages` ni repositorios aparte.
- Al terminar cualquier tarea: `git add -A`, commit con un mensaje claro y `git push origin main`.
- La demo WebGL se publica desde `main` (raíz del repo): `index.html`, `WebGL/` y `TemplateData/`.
- No reescribir el historial ni hacer `push --force` sobre `main`.
