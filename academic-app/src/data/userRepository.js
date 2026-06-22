// export const fetchUsersFromApi = async () => {
// const API_URL = 'https://7p7s35xz-7241.brs.devtunnels.ms/api/Alumnos';
// const API_URL = 'https://cloudacademic.onrender.com/api/alumnos';

  export const fetchUsersFromApi = async () => {
  const API_URL = 'https://4fg4xs9d-7241.brs.devtunnels.ms/api/Alumnos';

  try {
    const response = await fetch(API_URL, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`Error HTTP del servidor: ${response.status}`);
    }

    const data = await response.json();

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
    throw new Error("No se pudo conectar con el servidor de base de datos.");
  }
};