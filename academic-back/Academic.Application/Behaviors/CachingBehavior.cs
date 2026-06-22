using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Academic.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    IDistributedCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableQuery
{
    // 1. Diccionario concurrente para manejar bloqueos individuales por cada llave de caché.
    // Esto previene el "Cache Stampede" asegurando que solo un hilo a la vez consulte la base de datos por la misma llave.
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Semaphores = new();

    // 2. Opciones de serialización centralizadas para evitar excepciones con nulos y mejorar el rendimiento.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;

        // Intento 1: Obtenemos los datos de Redis directamente como un arreglo de bytes.
        // Esto evita alocar memoria innecesaria creando un string temporal gigante en la RAM.
        var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

        if (cachedBytes != null)
        {
            logger.LogInformation("✅ Caché Hit: Obteniendo datos desde Redis para la llave {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<TResponse>(cachedBytes, JsonOptions)!;
        }

        // Si hay Caché Miss, obtenemos o creamos un candado (SemaphoreSlim) exclusivo para esta llave específica.
        var semaphore = Semaphores.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

        // Obligamos a que las peticiones concurrentes por la misma llave esperen su turno.
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            // Intento 2 (Double-Check Locking): Verificamos nuevamente Redis por si otro hilo 
            // que entró antes que nosotros ya fue a la BD y pobló la caché mientras esperábamos en la línea anterior.
            cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

            if (cachedBytes != null)
            {
                logger.LogInformation("✅ Caché Hit (Post-Lock): Datos obtenidos desde Redis tras la espera para {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<TResponse>(cachedBytes, JsonOptions)!;
            }

            logger.LogInformation("⏳ Caché Miss: Consultando a la base de datos para la llave {CacheKey}", cacheKey);

            // 3. Ejecutamos el flujo normal (Consultamos a PostgreSQL a través del Handler)
            var response = await next();

            if (response != null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = request.Expiration
                };

                // Convertimos directamente el objeto a byte[] y lo guardamos en Redis de forma ultra rápida
                var serializedData = JsonSerializer.SerializeToUtf8Bytes(response, JsonOptions);
                await cache.SetAsync(cacheKey, serializedData, options, cancellationToken);

                logger.LogInformation("💾 Datos guardados en Redis para la llave {CacheKey}", cacheKey);
            }

            return response;
        }
        finally
        {
            // Liberamos el candado para que otros hilos puedan continuar
            semaphore.Release();
        }
    }
}

//using Academic.Application.Interfaces;
//using MediatR;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Logging;
//using System.Collections.Concurrent;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Academic.Application.Behaviors;

//public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheableQuery
//{
//    private readonly IDistributedCache cache;
//    private readonly ILogger<CachingBehavior<TRequest, TResponse>> logger;
//    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Semaphores = new();

//    private static readonly JsonSerializerOptions JsonOptions = new()
//    {
//        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//        PropertyNameCaseInsensitive = true
//    };

//    public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
//    {
//        this.cache = cache;
//        this.logger = logger;
//    }

//    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        var cacheKey = request.CacheKey;

//        // BYPASS: Forzamos que la caché sea nula para no llamar al contenedor de Redis bloqueado.
//        byte[]? cachedBytes = null;

//        if (cachedBytes != null)
//        {
//            logger.LogInformation("✅ Caché Hit: Obteniendo datos desde Redis");
//            return JsonSerializer.Deserialize<TResponse>(cachedBytes, JsonOptions)!;
//        }

//        var semaphore = Semaphores.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

//        // Obligamos a que las peticiones concurrentes esperen su turno
//        await semaphore.WaitAsync(cancellationToken);

//        try
//        {
//            logger.LogInformation($"⏳ Caché Miss (Bypass Activo): Consultando directo a la base de datos PostgreSQL en Docker para la llave {cacheKey}");

//            // Ejecutamos la consulta real a PostgreSQL
//            var response = await next();

//            logger.LogInformation("✅ Datos obtenidos de PostgreSQL con éxito. Omitiendo guardado en Redis temporalmente.");

//            return response;
//        }
//        finally
//        {
//            semaphore.Release();
//        }
//    }
//}

//using System.Collections.Concurrent;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Threading;
//using System.Threading.Tasks;
//using Academic.Application.Interfaces;
//using MediatR;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Logging;

//namespace Academic.Application.Behaviors;

//public class CachingBehavior<TRequest, TResponse>(
//    IDistributedCache cache,
//    ILogger<CachingBehavior<TRequest, TResponse>> logger)
//    : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : IRequest<TResponse>, ICacheableQuery
//{
//    // 1. Diccionario concurrente para manejar bloqueos individuales por cada llave de caché.
//    // Esto previene el "Cache Stampede" asegurando que solo un hilo a la vez consulte la base de datos por la misma llave.
//    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Semaphores = new();

//    // 2. Opciones de serialización centralizadas para evitar excepciones con nulos y mejorar el rendimiento.
//    private static readonly JsonSerializerOptions JsonOptions = new()
//    {
//        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//        PropertyNameCaseInsensitive = true
//    };

//    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        var cacheKey = request.CacheKey;

//        // Intento 1: Obtenemos los datos de Redis directamente como un arreglo de bytes.
//        // Esto evita alocar memoria innecesaria creando un string temporal gigante en la RAM.
//        //var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);
//        byte[]? cachedBytes = null;

//        if (cachedBytes != null)
//        {
//            logger.LogInformation("✅ Caché Hit: Obteniendo datos desde Redis para la llave {CacheKey}", cacheKey);
//            return JsonSerializer.Deserialize<TResponse>(cachedBytes, JsonOptions)!;
//        }

//        // Si hay Caché Miss, obtenemos o creamos un candado (SemaphoreSlim) exclusivo para esta llave específica.
//        var semaphore = Semaphores.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

//        // Obligamos a que las peticiones concurrentes por la misma llave esperen su turno.
//        await semaphore.WaitAsync(cancellationToken);

//        try
//        {
//            // Intento 2 (Double-Check Locking): Verificamos nuevamente Redis por si otro hilo 
//            // que entró antes que nosotros ya fue a la BD y pobló la caché mientras esperábamos en la línea anterior.
//            cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

//            if (cachedBytes != null)
//            {
//                logger.LogInformation("✅ Caché Hit (Post-Lock): Datos obtenidos desde Redis tras la espera para {CacheKey}", cacheKey);
//                return JsonSerializer.Deserialize<TResponse>(cachedBytes, JsonOptions)!;
//            }

//            logger.LogInformation("⏳ Caché Miss: Consultando a la base de datos para la llave {CacheKey}", cacheKey);

//            // 3. Ejecutamos el flujo normal (Consultamos a SQL Server a través del Handler)
//            var response = await next();

//            if (response != null)
//            {
//                var options = new DistributedCacheEntryOptions
//                {
//                    AbsoluteExpirationRelativeToNow = request.Expiration
//                };

//                // Convertimos directamente el objeto a byte[] y lo guardamos en Redis de forma ultra rápida
//                var serializedData = JsonSerializer.SerializeToUtf8Bytes(response, JsonOptions);
//                await cache.SetAsync(cacheKey, serializedData, options, cancellationToken);

//                logger.LogInformation("💾 Datos guardados en Redis para la llave {CacheKey}", cacheKey);
//            }

//            return response;
//        }
//        finally
//        {
//            // Liberamos el candado para que otros hilos puedan continuar
//            semaphore.Release();
//        }
//    }
//}