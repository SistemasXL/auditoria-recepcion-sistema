# Correcciones Pendientes - 218 Errores de Compilación

## Resumen de Problemas

Los servicios y repositorios usan nombres de propiedades que no existen en los modelos reales.

## Mapeo de Propiedades INCORRECTAS → CORRECTAS

### Modelo: Producto
| Usado (Incorrecto) | Real (Correcto) |
|-------------------|-----------------|
| `Nombre` | `Descripcion` |
| `SKU` | `CodigoInterno` |
| `PesoUnitario` | *NO EXISTE* (eliminar referencias) |

### Modelo: Incidencia
| Usado (Incorrecto) | Real (Correcto) |
|-------------------|-----------------|
| `FechaDeteccion` | `FechaReporte` |
| `EstadoResolucion` | `EstadoIncidencia` |
| `Prioridad` | `Severidad` |
| `UsuarioReportoID` | `UsuarioReportaID` |
| `UsuarioAsignadoID` | `UsuarioResponsableID` |
| `Producto` (navigation) | *NO EXISTE* (no hay ProductoID) |
| `ProductoID` | *NO EXISTE* |
| `DetalleAuditoria` (navigation) | *NO EXISTE* |
| `DetalleAuditoriaID` | *NO EXISTE* |
| `AccionCorrectiva` | `ComentariosResolucion` |
| `UsuarioReporto` (navigation) | `UsuarioReporta` |
| `UsuarioAsignado` (navigation) | `UsuarioResponsable` |

### Modelo: Proveedor
| Usado (Incorrecto) | Real (Correcto) |
|-------------------|-----------------|
| `RazonSocial` | `NombreProveedor` |
| `NombreFantasia` | `NombreProveedor` |
| `Provincia` | *NO EXISTE* |
| `Ciudad` | *NO EXISTE* |
| `CodigoPostal` | *NO EXISTE* |
| `PersonaContacto` | `Contacto` |

### Modelo: Evidencia
| Usado (Incorrecto) | Real (Correcto) |
|-------------------|-----------------|
| `TamanoBytes` | `TamañoKB` (convertir a KB) |
| `UsuarioCargaID` | *NO EXISTE* |
| `UsuarioCarga` (navigation) | *NO EXISTE* |
| `Descripcion` | *NO EXISTE* |
| `TipoEvidencia` | *NO EXISTE* (usar `TipoArchivo`) |
| `FechaCarga` | `FechaSubida` |
| `IncidenciaID` | *NO EXISTE* |

### Modelo: AuditoriaRecepcion
| Usado (Incorrecto) | Real (Correcto) |
|-------------------|-----------------|
| `FechaRecepcion` | `FechaInicio` |
| `Estado` | `EstadoAuditoria` |
| `NumeroOrdenCompra` | `OrdenCompraID` (es int?) |
| `UsuarioCreacion` (navigation) | `UsuarioAuditor` |
| `UsuarioCreacionID` | `UsuarioAuditorID` |

### Modelo: Usuario
| Usado (Incorrecto) | Real (Correcto) |
|-------------------|-----------------|
| `FechaInicio` | `FechaCreacion` |
| `Usuario` (property) | `NombreUsuario` |
| `NombreUsuarioID` | `UsuarioID` |

### Modelo: AuditoriaDetalle
| Usado (Incorrecto) | Real (Correcto) |
|-------------------|-----------------|
| `EstadoProducto` | *NO EXISTE* (ver modelo real) |

### DbContext: AuditoriaRecepcionContext
| Usado (Incorrecto) | Real (Correcto) |
|-------------------|-----------------|
| `Auditorias` | `AuditoriasRecepcion` |
| `DetallesAuditoria` | `DetallesAuditorias` (verificar) |
| `AuditoriasAcciones` | `AuditoriaLogs` |
| `AuditoriasRecepcionAcciones` | `AuditoriaLogs` |

## Archivos a Corregir (con prioridad)

### Alta Prioridad
1. `/backend/Services/Implementation/IncidenciaService.cs` - 32 errores
2. `/backend/Services/Implementation/ProductoService.cs` - 20 errores
3. `/backend/Services/Implementation/DashboardService.cs` - 20+ errores
4. `/backend/Services/Implementation/FileStorageService.cs` - 15 errores
5. `/backend/Services/Implementation/ProveedorService.cs` - 15 errores
6. `/backend/Services/Implementation/ReporteService.cs` - 15 errores

### Media Prioridad
7. `/backend/Repositories/Implementation/IncidenciaRepository.cs` - 12 errores
8. `/backend/Repositories/Implementation/ProductoRepository.cs` - 5 errores
9. `/backend/Repositories/Implementation/ProveedorRepository.cs` - 5 errores
10. `/backend/Services/Implementation/NotificationService.cs` - 8 errores
11. `/backend/Services/Implementation/AuditoriaService.cs` - 5 errores
12. `/backend/Services/Implementation/AuthService.cs` - 3 errores
13. `/backend/Services/Implementation/UsuarioService.cs` - 2 errores

## Acción Recomendada

Necesito ver los modelos completos de:
- AuditoriaDetalle
- AuditoriaRecepcion

Y confirmar los nombres exactos de los DbSets en AuditoriaRecepcionContext.
