import { fetchUsersFromApi } from '../data/userRepository';

export const executeGetUsers = async () => {
  try {
    const users = await fetchUsersFromApi();
    
    // Regla de negocio: ordenar alfabéticamente antes de entregar los datos a la UI
    return users.sort((a, b) => a.name.localeCompare(b.name));
    
  } catch (error) {
    console.error("Error en el caso de uso al obtener usuarios:", error);
    throw new Error("No se pudieron cargar los datos del sistema.");
  }
};