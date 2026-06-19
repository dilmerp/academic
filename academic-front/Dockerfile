# ====================================================
# Etapa 1: Construcción de la aplicación (SPA Angular)
# ====================================================
FROM node:20-alpine AS build-stage
WORKDIR /app

# Copiar los archivos de gestión de dependencias
COPY package.json package-lock.json ./

# Instalación limpia, rápida y determinista de los módulos de Node
RUN npm ci

# Copiar la totalidad del código fuente del frontend
COPY . .

# Compilar la aplicación Angular optimizada para producción
RUN npm run build --configuration=production

# ====================================================
# Etapa 2: Servidor Web de alta eficiencia (Nginx)
# ====================================================
FROM nginx:1.25-alpine AS production-stage

# Eliminar los archivos por defecto que trae Nginx para evitar conflictos
RUN rm -rf /usr/share/nginx/html/*

# Copiar los archivos estáticos compilados desde la etapa de Node
# NOTA: Verifica que en tu angular.json el outputpath sea exactamente este.
COPY --from=build-stage /app/dist/academic-front/browser /usr/share/nginx/html

# Copiar el archivo de configuración personalizado de Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Exponer el puerto estándar de HTTP
EXPOSE 80

# Arrancar Nginx en primer plano para asegurar que el contenedor permanezca activo
CMD ["nginx", "-g", "daemon off;"]