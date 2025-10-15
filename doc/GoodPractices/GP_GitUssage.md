# Buenas prácticas para el uso de los commits
### ⚙️ Tipos de commit (convención estándar)

| Tipo      | Uso                                             | Ejemplo                                                  |
|------------|--------------------------------------------------|-----------------------------------------------------------|
| **feat**   | Nueva funcionalidad                             | `feat(seed): add sub-RNG for shop generation`             |
| **fix**    | Corrección de bug                               | `fix(save): resolve corrupted JSON loading`               |
| **refactor** | Mejora de código sin cambiar el comportamiento | `refactor(core): clean up input handling logic`           |
| **style**  | Cambios de formato, nombres, espacios, etc.     | `style: rename variables to match camelCase`              |
| **docs**   | Cambios en documentación o comentarios           | `docs(readme): update setup instructions`                 |
| **test**   | Añadir o modificar tests                        | `test(dice): add unit tests for face detection`           |
| **perf**   | Mejoras de rendimiento                          | `perf(physics): reduce rigidbody update overhead`         |
| **chore**  | Tareas menores, mantenimiento, dependencias     | `chore: update FMOD integration package`                  |
| **build**  | Cambios en el sistema de build o dependencias   | `build: add editor scripting define for debug mode`       |
| **ci**     | Cambios en CI/CD o pipelines                    | `ci: add Unity test runner to GitHub Actions`             |
