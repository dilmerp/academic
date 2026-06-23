// export const fetchUsersFromApi = async () => {
// const API_URL = 'https://7p7s35xz-7241.brs.devtunnels.ms/api/Alumnos';
// const API_URL = 'https://cloudacademic.onrender.com/api/alumnos';

import * as SecureStore from 'expo-secure-store';

export const fetchUsersFromApi = async () => {
  const API_URL = 'https://4fg4xs9d-7241.brs.devtunnels.ms/api/Alumnos';

  try {
    // 1. Recuperamos el token de la bóveda de seguridad del celular
    const token = await SecureStore.getItemAsync('token');

    // 2. Realizamos la petición adjuntando el Token
    const response = await fetch(API_URL, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}` // Inyección del JWT
      }
    });

    if (!response.ok) {
      // Capturamos específicamente el error 401 por si el token expiró
      if (response.status === 401) {
        throw new Error('Sesión expirada o no autorizada. Por favor, inicie sesión nuevamente.');
      }
      throw new Error(`Error HTTP del servidor: ${response.status}`);
    }

    const data = await response.json();

    // 3. Mantenemos intacto tu mapeo de datos para la UI
    const mappedUsers = data.map((alumno) => {
      return {
        id: alumno.codigo ? alumno.codigo.toString() : Math.random().toString(),
        name: alumno.nombreCompleto || 'Alumno sin nombre',
        role: `${alumno.emailPrincipal || 'Sin correo registrado'} - ${alumno.estado || 'Desconocido'}`
      };
    });

    return mappedUsers;

  } catch (error) {
    console.error("Error de conexión:", error);
    // Devolvemos el mensaje específico si es un 401, o un mensaje genérico si es fallo de red
    throw new Error(error.message || "No se pudo conectar con el servidor de base de datos.");
  }
};