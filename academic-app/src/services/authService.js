import * as SecureStore from 'expo-secure-store';

// Utilizando la URL de tu Dev Tunnel activo para las pruebas
const API_URL = 'https://4fg4xs9d-7241.brs.devtunnels.ms/api/Auth';

export const authService = {
  login: async (credentials) => {
    // Limpiamos espacios en blanco accidentales al inicio o final del usuario
    const cleanLogin = credentials.login.trim();

    const response = await fetch(`${API_URL}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        login: cleanLogin,
        clave: credentials.clave
      }),
    });

    // Manejo inteligente de errores HTTP en lugar de un error genérico
    if (!response.ok) {
      let errorMessage = 'Error al intentar iniciar sesión.';
      
      try {
        // Intentamos extraer el mensaje de error exacto que envía .NET (si existe)
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.title || errorData.detail || errorMessage;
      } catch (e) {
        // Si no hay JSON (ej. caída del servidor), usamos los códigos de estado
        if (response.status === 401) {
          errorMessage = 'Usuario o contraseña incorrectos.';
        } else if (response.status === 404) {
          errorMessage = 'Las credenciales ingresadas no existen.';
        } else if (response.status >= 500) {
          errorMessage = 'Problemas internos en el servidor. Intente más tarde.';
        } else {
          errorMessage = `Error inesperado (Código: ${response.status}).`;
        }
      }
      
      throw new Error(errorMessage);
    }

    const data = await response.json();

    if (data && data.token) {
      // Guardado seguro en la bóveda del celular
      await SecureStore.setItemAsync('token', data.token);
      await SecureStore.setItemAsync('nombreCompleto', data.nombreCompleto);
      await SecureStore.setItemAsync('login', data.login);
    }

    return data;
  },

  actualizarClave: async (data) => {
    const payload = {
      Login: data.login,
      NuevaClave: data.nuevaClave,
    };

    const response = await fetch(`${API_URL}/actualizar-clave`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });

    // Aplicamos el mismo blindaje de errores para la actualización de clave
    if (!response.ok) {
      let errorMessage = 'Hubo un problema al procesar el cambio de clave institucional.';
      
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.title || errorData.detail || errorMessage;
      } catch (e) {
        if (response.status === 400) {
          errorMessage = 'Datos inválidos o la contraseña no cumple con las políticas de seguridad.';
        }
      }

      throw new Error(errorMessage);
    }

    return await response.json();
  },

  logout: async () => {
    await SecureStore.deleteItemAsync('token');
    await SecureStore.deleteItemAsync('nombreCompleto');
    await SecureStore.deleteItemAsync('login');
  },

  isAuthenticated: async () => {
    const token = await SecureStore.getItemAsync('token');
    return !!token;
  }
};